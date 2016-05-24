namespace Fooble.Core

open Fooble.Core.Persistence
open MediatR
open System
open System.Diagnostics

/// <summary>
/// Provides functionality used in the querying and presentation of member details.
/// </summary>
[<RequireQualifiedAccess>]
module MemberDetail =

    (* Active Patterns *)

    let internal (|IsSuccess|IsNotFound|) (result:IMemberDetailQueryResult) =
        if result.IsSuccess
            then Choice1Of2 result.ReadModel
            else Choice2Of2 ()

    (* Query *)

    /// <summary>
    /// Provides functionality used in the querying of member details.
    /// </summary>
    [<RequireQualifiedAccess>]
    module Query =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | Query of Guid

            interface IMemberDetailQuery with

                member this.Id
                    with get() =
                        match this with
                        | Query x -> x


        /// <summary>
        /// Constructs a member detail query.
        /// </summary>
        /// <param name="id">The member id to search for.</param>
        /// <returns>Returns a member detail query.</returns>
        [<CompiledName("Make")>]
        let make id = Query id :> IMemberDetailQuery

    (* Read Model *)

    /// <summary>
    /// Provides functionality used in the presentation of member details.
    /// </summary>
    [<RequireQualifiedAccess>]
    module internal ReadModel =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | ReadModel of Guid * string

            interface IMemberDetailReadModel with

                member this.Id
                    with get() =
                        match this with
                        | ReadModel (x, _) -> x

                member this.Name
                    with get() =
                        match this with
                        | ReadModel (_, x) -> x

        let internal make id name =
            Debug.Assert(notIsNull name, "Name parameter was null")
            Debug.Assert(String.notIsEmpty name, "Name parameter was an empty string")
            ReadModel (id, name) :> IMemberDetailReadModel

    (* Query Result *)

    /// <summary>
    /// Provides functionality used in the querying of member details.
    /// </summary>
    [<RequireQualifiedAccess>]
    module internal QueryResult =

        [<DefaultAugmentation(false)>]
        type private Implementation =
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
    
        let internal makeSuccess readModel =
            Debug.Assert(notIsNull <| box readModel, "Read model parameter was null")
            Success readModel :> IMemberDetailQueryResult

        let internal notFound = NotFound :> IMemberDetailQueryResult

    (* Query Handler *)

    /// <summary>
    /// Provides functionality used in the querying of member details.
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

            interface IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> with

                member this.Handle(query) =
                    Debug.Assert(notIsNull <| box query, "Query parameter was null")
                    Seq.tryFind (fun (x:MemberData) -> x.Id = query.Id) this.Context.MemberData
                    |> Option.map (fun x -> ReadModel.make x.Id x.Name)
                    |> function
                       | Some x -> QueryResult.makeSuccess x
                       | None -> QueryResult.notFound

        let internal make context =
            Debug.Assert(notIsNull context, "Context parameter was null")
            QueryHandler context :> IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>
