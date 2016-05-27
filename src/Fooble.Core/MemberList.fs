namespace Fooble.Core

open Fooble.Core.Persistence
open MediatR
open System

/// Provides functionality used in the querying and presentation of member lists.
[<RequireQualifiedAccess>]
module MemberList =

    (* Active Patterns *)

    let internal (|IsSuccess|IsNotFound|) (result:IMemberListQueryResult) =
        if result.IsSuccess
            then Choice1Of2 result.ReadModel
            else Choice2Of2 ()



    (* Query *)

    /// Provides functionality used in the querying of member lists.
    [<RequireQualifiedAccess>]
    module Query =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | Query

            interface IMemberListQuery

        /// <summary>
        /// Constructs a member list query.
        /// </summary>
        /// <returns>Returns a member list query.</returns>
        [<CompiledName("Make")>]
        let make () = Query :> IMemberListQuery



    (* Item Read Model *)

    [<RequireQualifiedAccess>]
    module internal ItemReadModel =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | ItemReadModel of Guid * string

            interface IMemberListItemReadModel with

                member this.Id
                    with get() =
                        match this with
                        | ItemReadModel (x, _) -> x

                member this.Nickname
                    with get() =
                        match this with
                        | ItemReadModel (_, x) -> x

        let internal make id nickname =
            assert (Guid.isNotEmpty id)
            assert (String.isNotNullOrEmpty nickname)
            assert (String.isNotLonger 64 nickname)
            ItemReadModel (id, nickname) :> IMemberListItemReadModel



    (* Read Model *)

    [<RequireQualifiedAccess>]
    module internal ReadModel =

        [<DefaultAugmentation(false)>]
        [<NoComparison>]
        type private Implementation =
            | ReadModel of seq<IMemberListItemReadModel>

            interface IMemberListReadModel with

                member this.Members
                    with get() =
                        match this with
                        | ReadModel xs -> xs

        let internal make members =
            assert (Seq.isNotNullOrEmpty members)
            ReadModel members :> IMemberListReadModel



    (* Query Result *)

    [<RequireQualifiedAccess>]
    module internal QueryResult =

        [<DefaultAugmentation(false)>]
        [<NoComparison>]
        type private Implementation =
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

        let internal makeSuccess readModel =
            assert (isNotNull <| box readModel)
            Success readModel :> IMemberListQueryResult

        let internal notFound = NotFound :> IMemberListQueryResult



    (* Query Handler *)

    [<RequireQualifiedAccess>]
    module internal QueryHandler =

        [<DefaultAugmentation(false)>]
        [<NoComparison>]
        type private Implementation =
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
                        |> Seq.map (fun x -> ItemReadModel.make x.Id x.Nickname)
                        |> List.ofSeq // materialize

                    match members with
                    | [] -> QueryResult.notFound
                    | xs -> Seq.ofList xs |> ReadModel.make |> QueryResult.makeSuccess

        let internal make context =
            assert (not <| isNull context)
            QueryHandler context :> IRequestHandler<IMemberListQuery, IMemberListQueryResult>
