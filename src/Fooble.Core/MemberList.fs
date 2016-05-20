namespace Fooble.Core

open Fooble.Core.Persistence
open MediatR
open System
open System.Diagnostics
open System.Linq

[<RequireQualifiedAccess>]
module MemberList =

    (* Query *)

    [<DefaultAugmentation(false)>]
    type private Query =
        | Query

        interface IMemberListQuery
    
    [<CompiledName("MakeQuery")>]
    let makeQuery () = Query :> IMemberListQuery

    (* Item Read Model *)

    [<DefaultAugmentation(false)>]
    type private ItemReadModel =
        | ItemReadModel of Guid * string

        member this.Id =
            match this with
            | ItemReadModel (x, _) -> x

        member this.Name =
            match this with
            | ItemReadModel (_, x) -> x

        interface IMemberListItemReadModel with
            member this.Id = this.Id
            member this.Name = this.Name

    let internal makeItemReadModel id name =
        Debug.Assert(notIsNull name, "Name parameter was null")
        Debug.Assert(String.notIsEmpty name, "Name parameter was empty string")
        ItemReadModel (id, name) :> IMemberListItemReadModel

    (* Read Model *)

    [<DefaultAugmentation(false)>]
    type private ReadModel =
        | ReadModel of seq<IMemberListItemReadModel>

        member this.Members =
            match this with
            | ReadModel xs -> xs

        interface IMemberListReadModel with
            member this.Members = this.Members

    let internal makeReadModel members =
        Debug.Assert(notIsNull members, "Members parameter was null")
        Debug.Assert(Seq.notIsEmpty members, "Members parameter was empty sequence")
        ReadModel members :> IMemberListReadModel

    (* Query Result *)

    [<DefaultAugmentation(false)>]
    type private QueryResult =
        | Success of IMemberListReadModel
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

        interface IMemberListQueryResult with
            member this.ReadModel = this.ReadModel
            member this.IsSuccess = this.IsSuccess
            member this.IsNotFound = this.IsNotFound

    let internal makeSuccessResult readModel =
        Debug.Assert(notIsNull <| box readModel, "Read model parameter was null")
        Success readModel :> IMemberListQueryResult

    let internal notFoundResult = NotFound' :> IMemberListQueryResult

    (* Query Handler *)

    [<DefaultAugmentation(false)>]
    type private QueryHandler =
        | QueryHandler of IFoobleContext

        member this.Context =
            match this with
            | QueryHandler x -> x

        interface IMemberListQueryHandler with

            member this.Handle(query) =
                Debug.Assert(notIsNull <| box query, "Query parameter was null")
                Seq.sortBy (fun (x:MemberData) -> x.Name) this.Context.MemberData
                |> Seq.map (fun x -> makeItemReadModel x.Id x.Name)
                |> List.ofSeq // materialize the results
                |> function
                   | [] -> notFoundResult
                   | xs -> Seq.ofList xs
                           |> makeReadModel
                           |> makeSuccessResult

    let internal makeQueryHandler context =
        Debug.Assert(not <| isNull context, "Context parameter was null")
        QueryHandler context :> IMemberListQueryHandler

    (* Active Patterns *)

    let internal (|IsSuccess|IsNotFound|) (result:IMemberListQueryResult) =
        if result.IsSuccess
            then Choice1Of2 result.ReadModel
            else Choice2Of2 ()
