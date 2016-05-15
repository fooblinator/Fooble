[<RequireQualifiedAccess>]
module Fooble.Core.MemberList

open MediatR
open System.Runtime.CompilerServices

(* Active Patterns *)

let internal (|IsNotFound|) (status:IMemberListQueryFailureStatus) = ()

(* Query *)

type private MemberListQueryImplementation() =
    interface IMemberListQuery

[<CompiledName("MakeQuery")>]
let makeQuery() =

    let contracts =
        [ postCondition (isNullValue >> not) "(MemberList) makeQuery returned null value" ]

    let body _ =
        MemberListQueryImplementation() :> IMemberListQuery

    ensure contracts body ()

(* Item Read Model *)

type private MemberListItemReadModelImplementation =
    { id:string; name:string }

    interface IMemberListItemReadModel with

        member this.Id =

            let contracts =
                [ postCondition (isNullValue >> not) "(IMemberListItemReadModel) Id property returned null value"
                  postCondition (isEmptyString >> not) "(IMemberListItemReadModel) Id property returned empty string"
                  postCondition (isGuidString) "(IMemberListItemReadModel) Id property returned string with invalid GUID format" ]

            let body x =
                match x with
                | { MemberListItemReadModelImplementation.id = i; name = _ } -> i

            ensure contracts body this

        member this.Name =

            let contracts =
                [ postCondition (isNullValue >> not) "(IMemberListItemReadModel) Name property returned null value"
                  postCondition (isEmptyString >> not) "(IMemberListItemReadModel) Name property returned empty string" ]

            let body x =
                match x with
                | { MemberListItemReadModelImplementation.id = _; name = n } -> n

            ensure contracts body this

let internal makeItemReadModel id name =

    let contracts =
        [ preCondition (fst >> isNullValue >> not) "(MemberList) makeItemReadModel id argument was null value"
          preCondition (fst >> isEmptyString >> not) "(MemberList) makeItemReadModel id argument was empty string"
          preCondition (fst >> isGuidString) "(MemberList) makeItemReadModel id argument was string with invalid GUID format"
          preCondition (snd >> isNullValue >> not) "(MemberList) makeItemReadModel name argument was null value"
          preCondition (snd >> isEmptyString >> not) "(MemberList) makeItemReadModel name argument was empty string"
          postCondition (isNullValue >> not) "(MemberList) makeItemReadModel returned null value" ]

    let body (x, y) =
        { MemberListItemReadModelImplementation.id = x; name = y } :> IMemberListItemReadModel

    ensure contracts body (id, name)

(* Read Model *)

type private MemberListReadModelImplementation =
    { members:seq<IMemberListItemReadModel> }

    interface IMemberListReadModel with

        member this.Members =

            let contracts =
                [ postCondition (isNullValue >> not) "(IMemberListReadModel) Members property returned null value"
                  postCondition (isEmptyValue >> not) "(IMemberListReadModel) Members property returned empty value"
                  postCondition (containsNullValues >> not) "(IMemberListReadModel) Members property returned containing null values" ]

            let body x =
                match x with
                | { MemberListReadModelImplementation.members = ms } -> ms

            ensure contracts body this

let internal makeReadModel members =

    let contracts =
        [ preCondition (isNullValue >> not) "(MemberList) makeReadModel members argument was null value"
          preCondition (isEmptyValue >> not) "(MemberList) makeReadModel members argument was empty value"
          preCondition (containsNullValues >> not) "(MemberList) makeReadModel members argument contained null values"
          postCondition (isNullValue >> not) "(MemberList) makeReadModel returned null value" ]

    let body x =
        { MemberListReadModelImplementation.members = x } :> IMemberListReadModel

    ensure contracts body members

(* Query Failure Status *)

type private MemberListQueryFailureStatusImplementation =
    | NotFound

    interface IMemberListQueryFailureStatus with

        member this.IsNotFound =

            let contracts =
                [ postCondition (Operators.id) "(IMemberListQueryFailureStatus) IsNotFound property returned false" ]

            let body x =
                match x with
                | MemberListQueryFailureStatusImplementation.NotFound -> true

            ensure contracts body this

[<CompiledName("NotFoundQueryFailureStatus")>]
let notFoundQueryFailureStatus =

    let contracts =
        [ postCondition (isNullValue >> not) "(MemberList) notFoundQueryFailureStatus returned null value" ]

    let body _ =
        MemberListQueryFailureStatusImplementation.NotFound :> IMemberListQueryFailureStatus

    ensure contracts body ()

(* Query Handler *)

[<AllowNullLiteral>]
type private MemberListQueryHandlerImplementation(context:IDataContext) =

    interface IMemberListQueryHandler with

        member this.Handle(query) =

            let contracts =
                [ preCondition (isNullValue >> not) "(IMemberListQueryHandler) Handle query argument was null value"
                  postCondition (isNullValue >> not) "(IMemberListQueryHandler) Handle returned null value" ]

            let body _ =
                match List.ofSeq context.Members with
                | [] -> failureResult notFoundQueryFailureStatus
                | mds ->
                    List.map (fun (md:IMemberData) -> makeItemReadModel md.Id md.Name) mds
                    |> Seq.ofList
                    |> makeReadModel
                    |> successResult

            ensure contracts body query

let internal makeQueryHandler context =

    let contracts =
        [ preCondition (isNullValue >> not) "(MemberList) makeQueryHandler context argument was null value"
          postCondition (isNullValue >> not) "(MemberList) makeQueryHandler returned null value" ]

    let body x =
        MemberListQueryHandlerImplementation(x) :> IMemberListQueryHandler

    ensure contracts body context
