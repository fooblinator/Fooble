[<RequireQualifiedAccess>]
module Fooble.Core.MessageDisplay

(* Active Patterns *)

let internal (|IsInformational|IsWarning|IsError|) (severity:IMessageDisplaySeverity) =
    if severity.IsInformational
        then Choice1Of3 ()
        elif severity.IsWarning
            then Choice2Of3 ()
            else Choice3Of3 ()

(* Validators *)

[<CompiledName("ValidateHeading")>]
let validateHeading heading =
    [ validateIsNotNullValue; validateIsNotEmptyString ]
    |> List.tryPick (fun fn -> fn heading "heading" "Heading")

[<CompiledName("ValidateMessages")>]
let validateMessages messages =
    seq {
        yield [ validateIsNotNullValue; validateIsNotEmptyValue ]
            |> List.tryPick (fun fn -> fn messages "messages" "Message list")
        yield [ validateContainsNotNullValues; validateContainsNotEmptyStrings ]
            |> List.tryPick (fun fn -> fn messages "messages" "Message list items")
    }
    |> Seq.tryPick Operators.id

(* Severity *)

type private MessageDisplaySeverityImplementation =
    | Informational
    | Warning
    | Error

    interface IMessageDisplaySeverity with

        member this.IsInformational =
            match this with
            | MessageDisplaySeverityImplementation.Informational -> true
            | _ -> false

        member this.IsWarning =
            match this with
            | MessageDisplaySeverityImplementation.Warning -> true
            | _ -> false

        member this.IsError =
            match this with
            | MessageDisplaySeverityImplementation.Error -> true
            | _ -> false

[<CompiledName("InformationalSeverity")>]
let informationalSeverity =

    let contracts =
        [ postCondition (isNullValue >> not) "(MessageDisplay) informationalSeverity returned null value" ]

    let body _ =
        MessageDisplaySeverityImplementation.Informational :> IMessageDisplaySeverity

    ensure contracts body ()

[<CompiledName("WarningSeverity")>]
let warningSeverity =

    let contracts =
        [ postCondition (isNullValue >> not) "(MessageDisplay) warningSeverity returned null value" ]

    let body _ =
        MessageDisplaySeverityImplementation.Warning :> IMessageDisplaySeverity

    ensure contracts body ()

[<CompiledName("ErrorSeverity")>]
let errorSeverity =

    let contracts =
        [ postCondition (isNullValue >> not) "(MessageDisplay) errorSeverity returned null value" ]

    let body _ =
        MessageDisplaySeverityImplementation.Error :> IMessageDisplaySeverity

    ensure contracts body ()

(* Read Model *)

type private MessageDisplayReadModelImplementation =
    { heading:string; severity:IMessageDisplaySeverity; messages:seq<string> }

    interface IMessageDisplayReadModel with

        member this.Heading =

            let contracts =
                [ postCondition (isNullValue >> not) "(IMessageDisplayReadModel) Heading property returned null value"
                  postCondition (isEmptyString >> not) "(IMessageDisplayReadModel) Heading property returned empty string" ]

            let body x =
                match x with
                | { MessageDisplayReadModelImplementation.heading = h; severity = _; messages = _ } -> h

            ensure contracts body this

        member this.Severity =

            let contracts =
                [ postCondition (isNullValue >> not) "(IMessageDisplayReadModel) Severity property returned null value" ]

            let body x =
                match x with
                | { MessageDisplayReadModelImplementation.heading = _; severity = s; messages = _ } -> s

            ensure contracts body this

        member this.Messages =

            let contracts =
                [ postCondition (isNullValue >> not) "(IMessageDisplayReadModel) Messages property returned null value"
                  postCondition (isEmptyValue >> not) "(IMessageDisplayReadModel) Messages property returned empty value"
                  postCondition (containsNullValues >> not) "(IMessageDisplayReadModel) Messages property returned containing null values"
                  postCondition (containsEmptyStrings >> not) "(IMessageDisplayReadModel) Messages property returned containing empty strings" ]

            let body x =
                match x with
                | { MessageDisplayReadModelImplementation.heading = _; severity = _; messages = ms } -> ms

            ensure contracts body this

[<CompiledName("MakeReadModel")>]
let makeReadModel heading severity messages =

    let contracts =
        [ postCondition (isNullValue >> not) "(MessageDisplay) makeReadModel returned null value" ]

    let body (x, y, z) =
        enforce (validateHeading heading)
        enforce (validateMessages messages)
        { MessageDisplayReadModelImplementation.heading = x; severity = y; messages = z } :> IMessageDisplayReadModel

    ensure contracts body (heading, severity, messages)
