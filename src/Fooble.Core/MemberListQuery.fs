namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open Fooble.Presentation
open MediatR

/// Provides helpers for member list query.
[<RequireQualifiedAccess>]
module MemberListQuery =

    [<DefaultAugmentation(false)>]
    type private MemberListQueryImpl =
        | Query

        interface IMemberListQuery

    /// <summary>
    /// Constructs a member list query.
    /// </summary>
    /// <returns>Returns a member list query.</returns>
    [<CompiledName("Make")>]
    let make () = Query :> IMemberListQuery

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberListQueryResultImpl =
        | Success of readModel:IMemberListReadModel
        | NotFound

        interface IMemberListQueryResult with

            member this.ReadModel
                with get() =
                    match this with
                    | Success(readModel = x) -> x
                    | NotFound -> invalidOp "Result was not successful"

            member this.IsSuccess
                with get() =
                    match this with
                    | Success(readModel = _) -> true
                    | NotFound -> false

            member this.IsNotFound
                with get() =
                    match this with
                    | Success(readModel = _) -> false
                    | NotFound -> true

    let internal makeSuccessResult readModel = Success(readModel) :> IMemberListQueryResult
    let internal notFoundResult = NotFound :> IMemberListQueryResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberListQueryHandlerImpl =
        | QueryHandler of context:IFoobleContext * itemReadModelFactory:MemberListItemReadModelFactory *
              readModelFactory:MemberListReadModelFactory

        member private this.Context
            with get() =
                match this with
                | QueryHandler(context = x) -> x

        member private this.ItemReadModelFactory
            with get() =
                match this with
                | QueryHandler(itemReadModelFactory = x) -> x

        member private this.ReadModelFactory
            with get() =
                match this with
                | QueryHandler(readModelFactory = x) -> x

        interface IRequestHandler<IMemberListQuery, IMemberListQueryResult> with

            member this.Handle(message) =
#if DEBUG
                assertWith (validateRequired message "message" "Message")
#endif

                let members =
                    this.Context.GetMembers(considerDeactivated = false)
                    |> List.map (fun x -> this.ItemReadModelFactory.Invoke(x.Id, x.Nickname))

                match members with
                | [] -> notFoundResult
                | xs ->

                let memberCount = Seq.length members

                Seq.ofList xs
                |> fun x -> this.ReadModelFactory.Invoke(x, memberCount)
                |> makeSuccessResult

    let internal makeHandler context itemReadModelFactory readModelFactory =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
        assertWith (validateRequired itemReadModelFactory "itemReadModelFactory" "Item read model factory")
        assertWith (validateRequired readModelFactory "readModelFactory" "Read model factory")
#endif
        QueryHandler(context, itemReadModelFactory, readModelFactory) :>
            IRequestHandler<IMemberListQuery, IMemberListQueryResult>
