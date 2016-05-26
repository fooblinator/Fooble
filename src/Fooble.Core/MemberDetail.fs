namespace Fooble.Core

open Fooble.Core.Persistence
open MediatR
open System

/// Provides functionality used in the querying and presentation of member details.
[<RequireQualifiedAccess>]
module MemberDetail =

    (* Active Patterns *)

    let internal (|IsSuccess|IsNotFound|) (result:IMemberDetailQueryResult) =
        if result.IsSuccess
            then Choice1Of2 result.ReadModel
            else Choice2Of2 ()



    (* Query *)

    /// Provides functionality used in the querying of member details.
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
        let make id =
            Validation.raiseIfInvalid (Member.validateId id)
            Query id :> IMemberDetailQuery



    (* Read Model *)

    [<RequireQualifiedAccess>]
    module internal ReadModel =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | ReadModel of Guid * string * string

            interface IMemberDetailReadModel with

                member this.Id
                    with get() =
                        match this with
                        | ReadModel (x, _, _) -> x

                member this.Username
                    with get() =
                        match this with
                        | ReadModel (_, x, _) -> x

                member this.Name
                    with get() =
                        match this with
                        | ReadModel (_, _, x) -> x

        let internal make id username name =
            assert (Guid.isNotEmpty id)
            assert (isNotNull username)
            assert (String.isNotEmpty username)
            assert (String.isNotShorter 3 username)
            assert (String.isNotLonger 32 username)
            assert (isNotNull name)
            assert (String.isNotEmpty name)
            ReadModel (id, username, name) :> IMemberDetailReadModel



    (* Query Result *)

    [<RequireQualifiedAccess>]
    module internal QueryResult =

        [<DefaultAugmentation(false)>]
        [<NoComparison>]
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
            assert (isNotNull <| box readModel)
            Success readModel :> IMemberDetailQueryResult

        let internal notFound = NotFound :> IMemberDetailQueryResult



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

            interface IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> with

                member this.Handle(query) =
                    assert (isNotNull <| box query)
                    Seq.tryFind (fun (x:MemberData) -> x.Id = query.Id) this.Context.MemberData
                    |> Option.map (fun x -> ReadModel.make x.Id x.Username x.Name)
                    |> function
                       | Some x -> QueryResult.makeSuccess x
                       | None -> QueryResult.notFound

        let internal make context =
            assert (isNotNull context)
            QueryHandler context :> IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>
