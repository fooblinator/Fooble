namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open Fooble.Presentation
open MediatR

/// Provides query-related helpers for member list.
[<RequireQualifiedAccess>]
module MemberListQuery =

    [<DefaultAugmentation(false)>]
    type private MemberListQueryImplementation =
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
    type private MemberListQueryResultImplementation =
        | Success of IMemberListReadModel
        | NotFound

        interface IMemberListQueryResult with

            member this.ReadModel
                with get() =
                    match this with
                    | Success x -> x
                    | NotFound -> invalidOp "Result was not successful"

            member this.IsSuccess
                with get() =
                    match this with
                    | Success _ -> true
                    | NotFound -> false

            member this.IsNotFound
                with get() =
                    match this with
                    | Success _ -> false
                    | NotFound -> true

    let internal makeSuccessResult readModel = Success readModel :> IMemberListQueryResult
    let internal notFoundResult = NotFound :> IMemberListQueryResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberListQueryHandlerImplementation =
        | QueryHandler of IFoobleContext * MemberListItemReadModelFactory * MemberListReadModelFactory

        member private this.Context
            with get() =
                match this with
                | QueryHandler (x, _, _) -> x

        member private this.ItemReadModelFactory
            with get() =
                match this with
                | QueryHandler (_, x, _) -> x

        member private this.ReadModelFactory
            with get() =
                match this with
                | QueryHandler (_, _, x) -> x

        interface IRequestHandler<IMemberListQuery, IMemberListQueryResult> with

            member this.Handle(message) =
                assert (isNotNull <| box message)

                let members =
                    this.Context.GetMembers()
                    |> List.map (fun x -> this.ItemReadModelFactory.Invoke(x.Id, x.Nickname))

                match members with
                | [] -> notFoundResult
                | xs ->
                    Seq.ofList xs
                    |> this.ReadModelFactory.Invoke
                    |> makeSuccessResult

    let internal makeHandler context itemReadModelFactory readModelFactory =
        assert (not <| isNull context)
        QueryHandler (context, itemReadModelFactory, readModelFactory) :>
            IRequestHandler<IMemberListQuery, IMemberListQueryResult>
