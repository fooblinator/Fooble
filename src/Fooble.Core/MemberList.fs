namespace Fooble.Core

open Fooble.Core.Persistence
open System
open System.Diagnostics

/// <summary>
/// Provides functionality used in the querying and presentation of member lists.
/// </summary>
[<RequireQualifiedAccess>]
module MemberList =

    (* Active Patterns *)

    let internal (|IsSuccess|IsNotFound|) (result:IMemberListQueryResult) =
        if result.IsSuccess
            then Choice1Of2 result.ReadModel
            else Choice2Of2 ()

    (* Query *)

    /// <summary>
    /// Provides functionality used in the querying of member lists.
    /// </summary>
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

    /// <summary>
    /// Provides functionality used in the presentation of member lists.
    /// </summary>
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

                member this.Name
                    with get() =
                        match this with
                        | ItemReadModel (_, x) -> x

        let internal make id name =
            Debug.Assert(notIsNull name, "Name parameter was null")
            Debug.Assert(String.notIsEmpty name, "Name parameter was empty string")
            ItemReadModel (id, name) :> IMemberListItemReadModel

    (* Read Model *)

    /// <summary>
    /// Provides functionality used in the presentation of member lists.
    /// </summary>
    [<RequireQualifiedAccess>]
    module internal ReadModel =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | ReadModel of seq<IMemberListItemReadModel>

            interface IMemberListReadModel with

                member this.Members
                    with get() =
                        match this with
                        | ReadModel xs -> xs

        let internal make members =
            Debug.Assert(notIsNull members, "Members parameter was null")
            Debug.Assert(Seq.notIsEmpty members, "Members parameter was empty sequence")
            ReadModel members :> IMemberListReadModel

    (* Query Result *)

    /// <summary>
    /// Provides functionality used in the querying of member lists.
    /// </summary>
    [<RequireQualifiedAccess>]
    module internal QueryResult =

        [<DefaultAugmentation(false)>]
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
            Debug.Assert(notIsNull <| box readModel, "Read model parameter was null")
            Success readModel :> IMemberListQueryResult

        let internal notFound = NotFound :> IMemberListQueryResult

    (* Query Handler *)

    /// <summary>
    /// Provides functionality used in the querying of member lists.
    /// </summary>
    [<RequireQualifiedAccess>]
    module internal QueryHandler =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | QueryHandler of IFoobleContext

            member private this.Context
                with get() =
                    match this with
                    | QueryHandler x -> x

            interface IMemberListQueryHandler with

                member this.Handle(query) =
                    Debug.Assert(notIsNull <| box query, "Query parameter was null")
                    Seq.sortBy (fun (x:MemberData) -> x.Name) this.Context.MemberData
                    |> Seq.map (fun x -> ItemReadModel.make x.Id x.Name)
                    |> List.ofSeq // materialize the results
                    |> function
                       | [] -> QueryResult.notFound
                       | xs -> Seq.ofList xs
                               |> ReadModel.make
                               |> QueryResult.makeSuccess

        let internal make context =
            Debug.Assert(not <| isNull context, "Context parameter was null")
            QueryHandler context :> IMemberListQueryHandler
