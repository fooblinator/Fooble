namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open Fooble.Presentation
open MediatR

/// <summary>
/// Represents the status of a member list query, and potential results, if successful.
/// </summary>
/// <remarks>The result is only one of "success" or "not found".</remarks>
type IMemberListQueryResult =
    /// The list of members' information to be presented.
    abstract ReadModel:IMemberListReadModel with get
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get
    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

/// Contains a request for a list of members' information, for the purpose of presentation.
type IMemberListQuery =
    inherit IRequest<IMemberListQueryResult>

[<RequireQualifiedAccess>]
module internal MemberListQuery =

    let internal (|IsSuccess|IsNotFound|) (result:IMemberListQueryResult) =
        if result.IsSuccess then Choice1Of2 result.ReadModel
        else Choice2Of2 () // IsNotFound

    [<DefaultAugmentation(false)>]
    type private MemberListQueryImplementation =
        | Query

        interface IMemberListQuery

    let internal make () = Query :> IMemberListQuery

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
        | QueryHandler of IFoobleContext

        member private this.Context
            with get() =
                match this with
                | QueryHandler x -> x

        interface IRequestHandler<IMemberListQuery, IMemberListQueryResult> with

            member this.Handle(message) =
                assert (isNotNull <| box message)

                let members =
                    query { for x in this.Context.MemberData do
                            sortBy x.Nickname
                            select x }
                    |> Seq.map (fun x -> MemberListReadModel.makeItem x.Id x.Nickname)
                    |> List.ofSeq // materialize

                match members with
                | [] -> notFoundResult
                | xs -> Seq.ofList xs |> MemberListReadModel.make |> makeSuccessResult

    let internal makeHandler context =
        assert (not <| isNull context)
        QueryHandler context :> IRequestHandler<IMemberListQuery, IMemberListQueryResult>
