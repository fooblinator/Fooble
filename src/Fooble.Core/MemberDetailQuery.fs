namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open Fooble.Presentation
open MediatR
open System

/// Provides helpers for member detail query.
[<RequireQualifiedAccess>]
module MemberDetailQuery =

    [<DefaultAugmentation(false)>]
    type private MemberDetailQueryImpl =
        | Query of id:Guid

        interface IMemberDetailQuery with

            member this.Id
                with get() =
                    match this with
                    | Query(id = x) -> x

    /// <summary>
    /// Constructs a member detail query.
    /// </summary>
    /// <param name="id">The member id to search for.</param>
    /// <returns>Returns a member detail query.</returns>
    [<CompiledName("Make")>]
    let make id =
        ensureWith (validateMemberId id)
        Query(id) :> IMemberDetailQuery

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberDetailQueryResultImpl =
        | Success of readModel:IMemberDetailReadModel
        | NotFound

        interface IMemberDetailQueryResult with

            member this.ReadModel
                with get() =
                    match this with
                    | Success(readModel = x) -> x
                    | _ -> invalidOp "Result was not successful"

            member this.IsSuccess
                with get() =
                    match this with
                    | Success(readModel = _) -> true
                    | _ -> false

            member this.IsNotFound
                with get() =
                    match this with
                    | NotFound -> true
                    | _ -> false

    let internal makeSuccessResult readModel = Success(readModel) :> IMemberDetailQueryResult
    let internal notFoundResult = NotFound :> IMemberDetailQueryResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberDetailQueryHandlerImpl =
        | QueryHandler of context:IFoobleContext * readModelFactory:MemberDetailReadModelFactory

        member private this.Context
            with get() =
                match this with
                | QueryHandler(context = x) -> x

        member private this.ReadModelFactory
            with get() =
                match this with
                | QueryHandler(readModelFactory = x) -> x

        interface IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> with

            member this.Handle(message) =
#if DEBUG
                assertWith (validateRequired message "message" "Message")
#endif

                let readModel =
                    this.Context.GetMember(message.Id, considerDeactivated = false)
                    |> Option.map (fun x ->
                           this.ReadModelFactory.Invoke(message.Id, x.Username, x.Email, x.Nickname, x.Registered,
                               x.PasswordChanged))

                match readModel with
                | Some(x) -> makeSuccessResult x
                | None -> notFoundResult

    let internal makeHandler context readModelFactory =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
        assertWith (validateRequired readModelFactory "readModelFactory" "Read model factory")
#endif
        QueryHandler(context, readModelFactory) :> IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>
