[<RequireQualifiedAccess>]
module Fooble.Core.MemberDetail

open Fooble.Core.Persistence
open MediatR
open System.Runtime.CompilerServices

(* Active Patterns *)

let internal (|IsNotFound|) (status:IMemberDetailQueryFailureStatus) = ()

(* Validators *)

[<CompiledName("ValidateId")>]
let validateId id =
    [ validateIsNotNullValue; validateIsNotEmptyString; validateIsGuidString ]
    |> List.tryPick (fun fn -> fn id "id" "Id")

(* Query *)

type private MemberDetailQueryImplementation =
    { id:string }

    interface IMemberDetailQuery with

        member this.Id =

            let contracts =
                [ postCondition (isNullValue >> not) "(IMemberDetailQuery) Id property returned null value"
                  postCondition (isEmptyString >> not) "(IMemberDetailQuery) Id property returned empty string"
                  postCondition (isGuidString) "(IMemberDetailQuery) Id property returned string with invalid GUID format" ]

            let body x =
                match x with
                | { MemberDetailQueryImplementation.id = i } -> i

            ensure contracts body this

[<CompiledName("MakeQuery")>]
let makeQuery id =

    let contracts =
        [ postCondition (isNullValue >> not) "(MemberDetail) makeQuery returned null value" ]

    let body x =
        enforce (validateId id)
        { MemberDetailQueryImplementation.id = x } :> IMemberDetailQuery

    ensure contracts body id

(* Read Model *)

type private MemberDetailReadModelImplementation =
    { id:string; name:string }

    interface IMemberDetailReadModel with

        member this.Id =

            let contracts =
                [ postCondition (isNullValue >> not) "(IMemberDetailReadModel) Id property returned null value"
                  postCondition (isEmptyString >> not) "(IMemberDetailReadModel) Id property returned empty string"
                  postCondition (isGuidString) "(IMemberDetailReadModel) Id property returned string with invalid GUID format" ]

            let body x =
                match x with
                | { MemberDetailReadModelImplementation.id = i; name = _ } -> i

            ensure contracts body this

        member this.Name =

            let contracts =
                [ postCondition (isNullValue >> not) "(IMemberDetailReadModel) Name property returned null value"
                  postCondition (isEmptyString >> not) "(IMemberDetailReadModel) Name property returned empty string" ]

            let body x =
                match x with
                | { MemberDetailReadModelImplementation.id = _; name = n } -> n

            ensure contracts body this

let internal makeReadModel id name =

    let contracts =
        [ preCondition (fst >> isNullValue >> not) "(MemberDetail) makeReadModel id argument was null value"
          preCondition (fst >> isEmptyString >> not) "(MemberDetail) makeReadModel id argument was empty string"
          preCondition (fst >> isGuidString) "(MemberDetail) makeReadModel id argument was string with invalid GUID format"
          preCondition (snd >> isNullValue >> not) "(MemberDetail) makeReadModel name argument was null value"
          preCondition (snd >> isEmptyString >> not) "(MemberDetail) makeReadModel name argument was empty string"
          postCondition (isNullValue >> not) "(MemberDetail) makeReadModel returned null value" ]

    let body (x, y) =
        { MemberDetailReadModelImplementation.id = x; name = y } :> IMemberDetailReadModel

    ensure contracts body (id, name)

(* Query Failure Status *)

type private MemberDetailQueryFailureStatusImplementation =
    | NotFound

    interface IMemberDetailQueryFailureStatus with

        member this.IsNotFound =

            let contracts =
                [ postCondition (Operators.id) "(IMemberDetailQueryFailureStatus) IsNotFound property returned false" ]

            let body x =
                match x with
                | MemberDetailQueryFailureStatusImplementation.NotFound -> true

            ensure contracts body this

[<CompiledName("NotFoundQueryFailureStatus")>]
let notFoundQueryFailureStatus =

    let contracts =
        [ postCondition (isNullValue >> not) "(MemberDetail) notFoundQueryFailureStatus returned null value" ]

    let body _ =
        MemberDetailQueryFailureStatusImplementation.NotFound :> IMemberDetailQueryFailureStatus

    ensure contracts body ()

(* Query Handler *)

[<AllowNullLiteral>]
type private MemberDetailQueryHandlerImplementation(context:IDataContext) =

    interface IMemberDetailQueryHandler with

        member this.Handle(query) =

            let contracts =
                [ preCondition (isNullValue >> not) "(IMemberDetailQueryHandler) Handle query argument was null value"
                  postCondition (isNullValue >> not) "(IMemberDetailQueryHandler) Handle returned null value" ]

            let body (x:IMemberDetailQuery) =
                box (context.Members.Find(x.Id)) // TODO: need to make MemberData so it will be valid as null... shouldn't have to box here
                |> Option.ofObj
                |> Option.map unbox<MemberData>
                |> Option.map (fun md -> makeReadModel md.Id md.Name)
                |> failureIfNone notFoundQueryFailureStatus

            ensure contracts body query

let internal makeQueryHandler context =

    let contracts =
        [ preCondition (isNullValue >> not) "(MemberDetail) makeQueryHandler context argument was null value"
          postCondition (isNullValue >> not) "(MemberDetail) makeQueryHandler returned null value" ]

    let body x =
        MemberDetailQueryHandlerImplementation(x) :> IMemberDetailQueryHandler

    ensure contracts body context
