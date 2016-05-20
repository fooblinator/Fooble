namespace Fooble.Core

open Fooble.Core.Persistence
open MediatR
open System
open System.Diagnostics

[<RequireQualifiedAccess>]
module MemberDetail =

    (* Query *)

    [<DefaultAugmentation(false)>]
    type private Query =
        | Query of Guid

        member this.Id =
            match this with
            | Query x -> x

        interface IMemberDetailQuery with
            member this.Id = this.Id

    [<CompiledName("MakeQuery")>]
    let makeQuery id = Query id :> IMemberDetailQuery

    (* Read Model *)

    [<DefaultAugmentation(false)>]
    type private ReadModel =
        | ReadModel of Guid * string

        member this.Id =
            match this with
            | ReadModel (x, _) -> x

        member this.Name =
            match this with
            | ReadModel (_, x) -> x

        interface IMemberDetailReadModel with
            member this.Id = this.Id
            member this.Name = this.Name

    let internal makeReadModel id name =
        Debug.Assert(notIsNull name, "Name parameter was null")
        Debug.Assert(String.notIsEmpty name, "Name parameter was empty string")
        ReadModel (id, name) :> IMemberDetailReadModel

    (* Query Result *)

    [<DefaultAugmentation(false)>]
    type private QueryResult =
        | Success of IMemberDetailReadModel
        | NotFound'

        member this.ReadModel =
            match this with
            | Success x -> x
            | NotFound' -> invalidOp "Result was not successful"

        member this.IsSuccess =
            match this with
            | Success _ -> true
            | NotFound' -> false

        member this.IsNotFound =
            match this with
            | Success _ -> false
            | NotFound' -> true

        interface IMemberDetailQueryResult with
            member this.ReadModel = this.ReadModel
            member this.IsSuccess = this.IsSuccess
            member this.IsNotFound = this.IsNotFound

    let internal makeSuccessResult readModel =
        Debug.Assert(notIsNull <| box readModel, "Read model parameter was null")
        Success readModel :> IMemberDetailQueryResult

    let internal notFoundResult = NotFound' :> IMemberDetailQueryResult

    (* Query Handler *)

    [<DefaultAugmentation(false)>]
    type private QueryHandler =
        | QueryHandler of IFoobleContext

        member this.Context =
            match this with
            | QueryHandler x -> x

        interface IMemberDetailQueryHandler with

            member this.Handle(query) =
                Debug.Assert(notIsNull <| box query, "Query parameter was null")
                Seq.tryFind (fun (x:MemberData) -> x.Id = query.Id) this.Context.MemberData
                |> Option.map (fun x -> makeReadModel x.Id x.Name)
                |> function
                   | Some x -> makeSuccessResult x
                   | None -> notFoundResult

    let internal makeQueryHandler context =
        Debug.Assert(notIsNull context, "Context parameter was null")
        QueryHandler context :> IMemberDetailQueryHandler

    (* Active Patterns *)

    let internal (|IsSuccess|IsNotFound|) (result:IMemberDetailQueryResult) =
        if result.IsSuccess
            then Choice1Of2 result.ReadModel
            else Choice2Of2 ()
