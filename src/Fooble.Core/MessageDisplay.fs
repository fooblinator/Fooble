namespace Fooble.Core

open System.Diagnostics

[<RequireQualifiedAccess>]
module MessageDisplay =

    (* Validation *)

    [<CompiledName("ValidateHeading")>]
    let validateHeading heading =
        [ (notIsNull), "Heading parameter was null"
          (String.notIsEmpty), "Heading parameter was empty string" ]
        |> Validation.validate heading "heading"

    [<CompiledName("ValidateMessages")>]
    let validateMessages messages =
        [ (notIsNull), "Messages parameter was null"
          (Seq.notIsEmpty), "Messages parameter was empty sequence"
          (Seq.notContainsNulls), "Messages parameter contained null(s)"
          (Seq.notContainsEmptyStrings), "Messages parameter contained empty string(s)" ]
        |> Validation.validate messages "messages"

    (* Severity *)

    [<DefaultAugmentation(false)>]
    type private Severity =
        | Informational'
        | Warning'
        | Error'

        member this.IsInformational =
            match this with
            | Informational' _ -> true
            | _ -> false

        member this.IsWarning =
            match this with
            | Warning' _ -> true
            | _ -> false

        member this.IsError =
            match this with
            | Error' _ -> true
            | _ -> false

        interface IMessageDisplaySeverity with
            member this.IsInformational = this.IsInformational
            member this.IsWarning = this.IsWarning
            member this.IsError = this.IsError

    [<CompiledName("InformationalSeverity")>]
    let informationalSeverity = Informational' :> IMessageDisplaySeverity
    
    [<CompiledName("WarningSeverity")>]
    let warningSeverity = Warning' :> IMessageDisplaySeverity
    
    [<CompiledName("ErrorSeverity")>]
    let errorSeverity = Error' :> IMessageDisplaySeverity

    (* Read Model *)

    [<DefaultAugmentation(false)>]
    type private ReadModel =
        | ReadModel of string * IMessageDisplaySeverity * seq<string>

        member this.Heading =
            match this with
            | ReadModel (x, _, _) -> x

        member this.Severity =
            match this with
            | ReadModel (_, x, _) -> x

        member this.Messages =
            match this with
            | ReadModel (_, _, xs) -> xs

        interface IMessageDisplayReadModel with
            member this.Heading = this.Heading
            member this.Severity = this.Severity
            member this.Messages = this.Messages
    
    [<CompiledName("MakeReadModel")>]
    let makeReadModel heading severity messages =
        Validation.raiseIfInvalid <| validateHeading heading
        Validation.raiseIfInvalid <| validateMessages messages
        ReadModel (heading, severity, messages) :> IMessageDisplayReadModel

    (* Active Patterns *)

    let internal (|IsInformational|IsWarning|IsError|) (severity:IMessageDisplaySeverity) =
        if severity.IsInformational
            then Choice1Of3 ()
            elif severity.IsWarning
                then Choice2Of3 ()
                else Choice3Of3 ()
