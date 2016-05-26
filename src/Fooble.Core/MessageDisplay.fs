namespace Fooble.Core

open System.Runtime.CompilerServices

/// Provides functionality used in the presentation of messages.
[<RequireQualifiedAccess>]
[<Extension>]
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
        [<NoComparison>]
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



    (* Extensions *)

    /// <summary>
    /// Constructs a message display read model from a member detail query result.
    /// </summary>
    /// <param name="result">The member detail query result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplay.ReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let ofMemberDetailQueryResult result =

        [ (isNotNull << box), "Result parameter was null" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        match result with
        | MemberDetail.IsSuccess _ -> invalidOp "Result was not unsuccessful"

        | MemberDetail.IsNotFound ->
            ReadModel.make "Member" "Detail" 404 Severity.warning "No matching member could be found."

    /// <summary>
    /// Constructs a message display read model from a member list query result.
    /// </summary>
    /// <param name="result">The member list query result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplay.ReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let ofMemberListQueryResult result =

        [ (isNotNull << box), "Result parameter was null" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        match result with
        | MemberList.IsSuccess _ -> invalidOp "Result was not unsuccessful"

        | MemberList.IsNotFound ->
            ReadModel.make "Member" "List" 200 Severity.informational "No members have yet been added."

    /// <summary>
    /// Constructs a message display read model from a self-service register command result.
    /// </summary>
    /// <param name="result">The self-service register command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplay.ReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let ofSelfServiceRegisterCommandResult result =

        [ (isNotNull << box), "Result parameter was null" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        match result with
        | SelfServiceRegister.IsSuccess -> invalidOp "Result was not unsuccessful"

        | SelfServiceRegister.IsUsernameUnavailable ->
            ReadModel.make "Self-Service" "Register" 400 Severity.warning "Requested username is unavailable."

    /// <summary>
    /// Constructs a message display read model from a validation result.
    /// </summary>
    /// <param name="result">The validation result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on "invalid" results. For displaying a "valid" result, use
    /// <see cref="MessageDisplay.ReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let ofValidationResult result =

        [ (isNotNull << box), "Result parameter was null" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        match result with
        | Validation.IsValid ->  invalidOp "Result was not invalid"

        | Validation.IsInvalid (_, x) ->
            ReadModel.make "Validation" String.empty 400 Severity.error
                (sprintf "Validation was not successful and returned: \"%s\"" x)
