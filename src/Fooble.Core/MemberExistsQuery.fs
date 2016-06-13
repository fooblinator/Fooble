namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

/// Provides helpers for member exists query.
[<RequireQualifiedAccess>]
module MemberExistsQuery =

    [<DefaultAugmentation(false)>]
    type private MemberExistsQueryImpl =
        | Query of id:Guid

        interface IMemberExistsQuery with

            member this.Id
                with get() =
                    match this with
                    | Query(id = x) -> x

    /// <summary>
    /// Constructs a member exists query.
    /// </summary>
    /// <param name="id">The member id to search for.</param>
    /// <returns>Returns a member exists query.</returns>
    [<CompiledName("Make")>]
    let make id =
        ensureWith (validateMemberId id)
        Query(id) :> IMemberExistsQuery

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberExistsQueryResultImpl =
        | Success
        | NotFound

        interface IMemberExistsQueryResult with

            member this.IsSuccess
                with get() =
                    match this with
                    | Success -> true
                    | _ -> false

            member this.IsNotFound
                with get() =
                    match this with
                    | NotFound -> true
                    | _ -> false

    let internal successResult = Success :> IMemberExistsQueryResult
    let internal notFoundResult = NotFound :> IMemberExistsQueryResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberExistsQueryHandlerImpl =
        | QueryHandler of context:IFoobleContext

        member private this.Context
            with get() =
                match this with
                | QueryHandler(context = x) -> x

        interface IRequestHandler<IMemberExistsQuery, IMemberExistsQueryResult> with

            member this.Handle(message) =
#if DEBUG
                assertWith (validateRequired message "message" "Message")
#endif

                match this.Context.ExistsMemberId(message.Id, includeDeactivated = false) with
                | true -> successResult
                | false -> notFoundResult

    let internal makeHandler context =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
#endif
        QueryHandler(context) :> IRequestHandler<IMemberExistsQuery, IMemberExistsQueryResult>
