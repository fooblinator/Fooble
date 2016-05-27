namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices

/// Provides functionality used in the presentation of messages.
[<RequireQualifiedAccess>]
[<Extension>]
module MessageDisplay =

    /// <summary>
    /// Represents a message display severity of "informational".
    /// </summary>
    /// <returns>Returns a message display severity of "informational".</returns>
    [<CompiledName("InformationalSeverity")>]
    let informationalSeverity = MessageDisplayReadModel.informationalSeverity

    /// <summary>
    /// Represents a message display severity of "warning".
    /// </summary>
    /// <returns>Returns a message display severity of "warning".</returns>
    [<CompiledName("WarningSeverity")>]
    let warningSeverity = MessageDisplayReadModel.warningSeverity

    /// <summary>
    /// Represents a message display severity of "error".
    /// </summary>
    /// <returns>Returns a message display severity of "error".</returns>
    [<CompiledName("ErrorSeverity")>]
    let errorSeverity = MessageDisplayReadModel.errorSeverity

    /// <summary>
    /// Constructs a message display read model.
    /// </summary>
    /// <param name="heading">The message display heading.</param>
    /// <param name="subHeading">The message display sub-heading.</param>
    /// <param name="statusCode">The message display status code.</param>
    /// <param name="severity">The message display severity.</param>
    /// <param name="messages">The messages to be displayed.</param>
    /// <returns>Returns a message display read model.</returns>
    [<CompiledName("MakeReadModel")>]
    let makeReadModel heading subHeading statusCode severity message =

        [ (String.isNotNullOrEmpty), "Heading is required" ]
        |> ValidationResult.get heading "heading"
        |> ValidationResult.enforce

        [ (isNotNull), "Sub-heading is required" ]
        |> ValidationResult.get subHeading "subHeading"
        |> ValidationResult.enforce

        [ (fun x -> x >= 0), "Status code parameter is less than zero" ]
        |> ValidationResult.get statusCode "statusCode"
        |> ValidationResult.enforce

        [ (String.isNotNullOrEmpty), "Message is required" ]
        |> ValidationResult.get message "message"
        |> ValidationResult.enforce

        MessageDisplayReadModel.make heading subHeading statusCode severity message
