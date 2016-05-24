namespace Fooble.Core

/// Provides functionality used in the presentation of messages.
[<RequireQualifiedAccess>]
module MessageDisplay =

    (* Active Patterns *)

    let internal (|IsInformational|IsWarning|IsError|) (severity:IMessageDisplaySeverity) =
        if severity.IsInformational
            then Choice1Of3 ()
            elif severity.IsWarning
                then Choice2Of3 ()
                else Choice3Of3 ()



    (* Severity *)

    /// Provides functionality used in the presentation of messages - specifically the message display severity.
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

    /// Provides functionality used in the presentation of messages - specifically the message display read model.
    [<RequireQualifiedAccess>]
    module ReadModel =

        [<DefaultAugmentation(false)>]
        type private Implementation =
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

            [ (isNotNull), "Heading parameter was null"
              (String.isNotEmpty), "Heading parameter was an empty string" ]
            |> Validation.validate heading "heading"
            |> Validation.raiseIfInvalid

            [ (isNotNull), "Sub-heading parameter was null" ]
            |> Validation.validate subHeading "subHeading"
            |> Validation.raiseIfInvalid

            [ (fun x -> x >= 0), "Status code parameter was less than zero" ]
            |> Validation.validate statusCode "statusCode"
            |> Validation.raiseIfInvalid

            [ (isNotNull), "Message parameter was null"
              (String.isNotEmpty), "Message parameter was an empty string" ]
            |> Validation.validate message "message"
            |> Validation.raiseIfInvalid

            ReadModel (heading, subHeading, statusCode, severity, message) :> IMessageDisplayReadModel
