namespace Fooble.Presentation

open Fooble.Common

/// <summary>
/// Represents the severity of a message display.
/// </summary>
/// <remarks>The severity is only one of "informational", "warning", or "error".</remarks>
type IMessageDisplaySeverity =
    /// Whether the severity is "informational" (or not).
    abstract IsInformational:bool with get
    /// Whether the severity is "warning" (or not).
    abstract IsWarning:bool with get
    /// Whether the severity is "error" (or not).
    abstract IsError:bool with get

/// Contains messages, for the purpose of presentation.
type IMessageDisplayReadModel =
    /// The heading of the message display.
    abstract Heading:string with get
    /// The sub-heading of the message display.
    abstract SubHeading:string with get
    /// The status code of the message display.
    abstract StatusCode:int with get
    /// The severity of the message display.
    abstract Severity:IMessageDisplaySeverity with get
    /// The message to be displayed.
    abstract Message:string with get

[<RequireQualifiedAccess>]
module internal MessageDisplayReadModel =

    let internal (|IsInformational|IsWarning|IsError|) (severity:IMessageDisplaySeverity) =
        if severity.IsInformational then Choice1Of3 ()
        elif severity.IsWarning then Choice2Of3 ()
        else Choice3Of3 () // IsError

    [<DefaultAugmentation(false)>]
    type private MessageDisplaySeverityImplementation =
        | Informational
        | Warning
        | Error

        interface IMessageDisplaySeverity with

            member this.IsInformational
                with get() =
                    match this with
                    | Informational -> true
                    | _ -> false

            member this.IsWarning
                with get() =
                    match this with
                    | Warning -> true
                    | _ -> false

            member this.IsError
                with get() =
                    match this with
                    | Error -> true
                    | _ -> false

    let internal informationalSeverity = Informational :> IMessageDisplaySeverity
    let internal warningSeverity = Warning :> IMessageDisplaySeverity
    let internal errorSeverity = Error :> IMessageDisplaySeverity

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MessageDisplayReadModelImplementation =
        | ReadModel of string * string * int * IMessageDisplaySeverity * string

        interface IMessageDisplayReadModel with

            member this.Heading
                with get() =
                    match this with
                    | ReadModel (x, _, _, _, _) -> x

            member this.SubHeading
                with get() =
                    match this with
                    | ReadModel (_, x, _, _, _) -> x

            member this.StatusCode
                with get() =
                    match this with
                    | ReadModel (_, _, x, _, _) -> x

            member this.Severity
                with get() =
                    match this with
                    | ReadModel (_, _, _, x, _) -> x

            member this.Message
                with get() =
                    match this with
                    | ReadModel (_, _, _, _, x) -> x

    let internal make heading subHeading statusCode severity message =
        assert (String.isNotNullOrEmpty heading)
        assert (isNotNull subHeading)
        assert (statusCode >= 0)
        assert (String.isNotNullOrEmpty message)
        ReadModel (heading, subHeading, statusCode, severity, message) :> IMessageDisplayReadModel
