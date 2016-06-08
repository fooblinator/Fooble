namespace Fooble.Presentation

open Fooble.Common

/// Provides presentation-related helpers for message display.
[<RequireQualifiedAccess>]
module MessageDisplayReadModel =

    [<DefaultAugmentation(false)>]
    type private MessageDisplaySeverityImpl =
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

    /// <summary>
    /// Represents a message display severity of "informational".
    /// </summary>
    /// <returns>Returns a message display severity of "informational".</returns>
    [<CompiledName("InformationalSeverity")>]
    let informationalSeverity = Informational :> IMessageDisplaySeverity

    /// <summary>
    /// Represents a message display severity of "warning".
    /// </summary>
    /// <returns>Returns a message display severity of "warning".</returns>
    [<CompiledName("WarningSeverity")>]
    let warningSeverity = Warning :> IMessageDisplaySeverity

    /// <summary>
    /// Represents a message display severity of "error".
    /// </summary>
    /// <returns>Returns a message display severity of "error".</returns>
    [<CompiledName("ErrorSeverity")>]
    let errorSeverity = Error :> IMessageDisplaySeverity

    [<DefaultAugmentation(false)>]
    type private MessageDisplayReadModelImpl =
        | ReadModel of heading:string * subHeading:string * statusCode:int * severity:IMessageDisplaySeverity *
              message:string

        interface IMessageDisplayReadModel with

            member this.Heading
                with get() =
                    match this with
                    | ReadModel(heading = x) -> x

            member this.SubHeading
                with get() =
                    match this with
                    | ReadModel(subHeading = x) -> x

            member this.StatusCode
                with get() =
                    match this with
                    | ReadModel(statusCode = x) -> x

            member this.Severity
                with get() =
                    match this with
                    | ReadModel(severity = x) -> x

            member this.Message
                with get() =
                    match this with
                    | ReadModel(message = x) -> x

    /// <summary>
    /// Constructs a message display read model.
    /// </summary>
    /// <param name="heading">The message display heading.</param>
    /// <param name="subHeading">The message display sub-heading.</param>
    /// <param name="statusCode">The message display status code.</param>
    /// <param name="severity">The message display severity.</param>
    /// <param name="messages">The messages to be displayed.</param>
    /// <returns>Returns a message display read model.</returns>
    [<CompiledName("Make")>]
    let make heading subHeading statusCode severity message =
        ensureOn heading "heading" [ (String.isNotNullOrEmpty), "Heading is required" ]
        ensureWith (validateRequired subHeading "subHeading" "Sub-heading")
        ensureOn statusCode "statusCode" [ ((<=) 0), "Status code parameter is less than zero" ]
        ensureWith (validateRequired severity "severity" "Severity")
        ensureOn message "message" [ (String.isNotNullOrEmpty), "Message is required" ]
        ReadModel(heading, subHeading, statusCode, severity, message) :> IMessageDisplayReadModel
