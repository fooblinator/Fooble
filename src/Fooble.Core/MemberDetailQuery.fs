namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open Fooble.Presentation
open MediatR
open System

/// Provides query-related helpers for member detail.
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
        enforce (Member.validateId id)
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
                assert (isNotNull (box message))

                let readModel =
                    this.Context.GetMember(message.Id)
                    |> Option.map (fun x ->
                           this.ReadModelFactory.Invoke(x.Id, x.Username, x.Email, x.Nickname, x.Registered,
                               x.PasswordChanged))

                match readModel with
                | Some x -> makeSuccessResult x
                | None -> notFoundResult

    let internal makeHandler context readModelFactory =
        QueryHandler(context, readModelFactory) :> IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>
