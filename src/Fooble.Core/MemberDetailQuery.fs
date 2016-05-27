namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open Fooble.Presentation
open MediatR
open System

/// <summary>
/// Represents the status of a member detail query, and potential results, if successful.
/// </summary>
/// <remarks>The result is only one of "success" or "not found".</remarks>
type IMemberDetailQueryResult =
    /// The single member's detail information to be presented.
    abstract ReadModel:IMemberDetailReadModel with get
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get
    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

/// Contains a request for a single member's detailed information, for the purpose of presentation.
type IMemberDetailQuery =
    inherit IRequest<IMemberDetailQueryResult>
    /// The member id to search for.
    abstract Id:Guid with get

[<RequireQualifiedAccess>]
module internal MemberDetailQuery =

    let internal (|IsSuccess|IsNotFound|) (result:IMemberDetailQueryResult) =
        if result.IsSuccess then Choice1Of2 result.ReadModel
        else Choice2Of2 () // IsNotFound

    [<DefaultAugmentation(false)>]
    type private MemberDetailQueryImplementation =
        | Query of Guid

        interface IMemberDetailQuery with

            member this.Id
                with get() =
                    match this with
                    | Query x -> x

    let internal make id =
        assert (Guid.isNotEmpty id)
        Query id :> IMemberDetailQuery

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberDetailQueryResultImplementation =
        | Success of IMemberDetailReadModel
        | NotFound

        interface IMemberDetailQueryResult with

            member this.ReadModel
                with get() =
                    match this with
                    | Success x -> x
                    | _ -> invalidOp "Result was not successful"

            member this.IsSuccess
                with get() =
                    match this with
                    | Success _ -> true
                    | _ -> false

            member this.IsNotFound
                with get() =
                    match this with
                    | NotFound -> true
                    | _ -> false

    let internal makeSuccessResult readModel = Success readModel :> IMemberDetailQueryResult
    let internal notFoundResult = NotFound :> IMemberDetailQueryResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberDetailQueryHandlerImplementation =
        | QueryHandler of IFoobleContext

        member private this.Context
            with get() =
                match this with
                | QueryHandler x -> x

        interface IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> with

            member this.Handle(message) =
                assert (isNotNull <| box message)

                let readModel =
                    query { for x in this.Context.MemberData do
                            where (x.Id = message.Id)
                            select x }
                    |> Seq.tryHead
                    |> Option.map (fun x -> MemberDetailReadModel.make x.Id x.Username x.Email x.Nickname)

                match readModel with
                | Some x -> makeSuccessResult x
                | None -> notFoundResult

    let internal makeHandler context =
        assert (isNotNull context)
        QueryHandler context :> IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>
