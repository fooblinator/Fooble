namespace Fooble.Core

[<RequireQualifiedAccess>]
module MessageDisplay =

    (* Validators *)

    [<CompiledName("ValidateHeading")>]
    let validateHeading heading =
        [ (notIsNull), "Heading was null"
          (String.notIsEmpty), "Heading was empty string" ]
        |> validate heading "heading"

    [<CompiledName("ValidateMessages")>]
    let validateMessages messages =
        [ (notIsNull), "Messages was null"
          (Seq.notIsEmpty), "Messages was empty sequence"
          (Seq.notContainsNulls), "Messages contained null(s)"
          (Seq.notContainsEmptyStrings), "Messages contained empty string(s)" ]
        |> validate messages "messages"

    [<RequireQualifiedAccess>]
    module Severity =

        (* Implementation *)

        type private Implementation =
            | Informational
            | Warning
            | Error

            interface IMessageDisplaySeverity with

                member this.IsInformational =
                    match this with
                    | Informational -> true
                    | _ -> false

                member this.IsWarning =
                    match this with
                    | Warning -> true
                    | _ -> false

                member this.IsError =
                    match this with
                    | Error -> true
                    | _ -> false

        (* Construction *)

        [<CompiledName("Informational")>]
        let informational = Informational :> IMessageDisplaySeverity

        [<CompiledName("Warning")>]
        let warning = Warning :> IMessageDisplaySeverity

        [<CompiledName("Error")>]
        let error = Error :> IMessageDisplaySeverity

    [<RequireQualifiedAccess>]
    module ReadModel =

        (* Implementation *)

        type private Implementation =
            { heading:string; severity:IMessageDisplaySeverity; messages:seq<string> }

            interface IMessageDisplayReadModel with

                member this.Heading = this.heading
                member this.Severity = this.severity
                member this.Messages = this.messages

        (* Construction *)

        [<CompiledName("Make")>]
        let make heading severity messages =
            raiseIfInvalid <| validateHeading heading
            raiseIfInvalid <| validateMessages messages
            { heading = heading; severity = severity; messages = messages } :> IMessageDisplayReadModel

[<AutoOpen>]
module internal MessageDisplayLibrary =

    (* Active Patterns *)

    let internal (|IsInformational|IsWarning|IsError|) (severity:IMessageDisplaySeverity) =
        if severity.IsInformational
            then Choice1Of3 ()
            elif severity.IsWarning
                then Choice2Of3 ()
                else Choice3Of3 ()
