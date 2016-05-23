namespace Fooble.Core

open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics

/// <summary>
/// Provides functionality used in the presentation of messages.
/// </summary>
[<RequireQualifiedAccess>]
module MessageDisplay =

    (* Active Patterns *)

    let internal (|IsInformational|IsWarning|IsError|) (severity:IMessageDisplaySeverity) =
        if severity.IsInformational
            then Choice1Of3 ()
            elif severity.IsWarning
                then Choice2Of3 ()
                else Choice3Of3 ()

    (* Validation *)

    /// <summary>
    /// Validates the supplied message display heading.
    /// </summary>
    /// <param name="heading">The message display heading.</param>
    /// <returns>Returns a validation result.</returns>
    [<CompiledName("ValidateHeading")>]
    let validateHeading heading =
        [ (notIsNull), "Heading parameter was null"
          (String.notIsEmpty), "Heading parameter was empty string" ]
        |> Validation.validate heading "heading"

    /// <summary>
    /// Validates the supplied messages to be displayed.
    /// </summary>
    /// <param name="messages">The messages to be displayed.</param>
    /// <returns>Returns a validation result.</returns>
    [<CompiledName("ValidateMessages")>]
    let validateMessages messages =
        [ (notIsNull), "Messages parameter was null"
          (Seq.notIsEmpty), "Messages parameter was empty sequence"
          (Seq.notContainsNulls), "Messages parameter contained null(s)"
          (Seq.notContainsEmptyStrings), "Messages parameter contained empty string(s)" ]
        |> Validation.validate messages "messages"

    (* Severity *)

    /// <summary>
    /// Provides functionality used in the presentation of messages - specifically the message display severity.
    /// </summary>
    [<RequireQualifiedAccess>]
    module Severity =

        [<DefaultAugmentation(false)>]
        type private Implementation =
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
        [<CompiledName("Informational")>]
        let informational = Informational :> IMessageDisplaySeverity
    
        /// <summary>
        /// Represents a message display severity of "warning".
        /// </summary>
        /// <returns>Returns a message display severity of "warning".</returns>
        [<CompiledName("Warning")>]
        let warning = Warning :> IMessageDisplaySeverity
    
        /// <summary>
        /// Represents a message display severity of "error".
        /// </summary>
        /// <returns>Returns a message display severity of "error".</returns>
        [<CompiledName("Error")>]
        let error = Error :> IMessageDisplaySeverity

    (* Read Model *)

    /// <summary>
    /// Provides functionality used in the presentation of messages - specifically the message display read model.
    /// </summary>
    [<RequireQualifiedAccess>]
    module ReadModel =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | ReadModel of string * IMessageDisplaySeverity * seq<string>

            interface IMessageDisplayReadModel with

                member this.Heading
                    with get() =
                        match this with
                        | ReadModel (x, _, _) -> x

                member this.Severity
                    with get() =
                        match this with
                        | ReadModel (_, x, _) -> x

                member this.Messages
                    with get() =
                        match this with
                        | ReadModel (_, _, x) -> x

        /// <summary>
        /// Constructs a message display read model.
        /// </summary>
        /// <param name="heading">The message display heading.</param>
        /// <param name="severity">The message display severity.</param>
        /// <param name="messages">The messages to be displayed.</param>
        /// <returns>Returns a message display read model.</returns>
        [<CompiledName("Make")>]
        let make heading severity messages =
            Validation.raiseIfInvalid <| validateHeading heading
            Validation.raiseIfInvalid <| validateMessages messages
            ReadModel (heading, severity, messages) :> IMessageDisplayReadModel
