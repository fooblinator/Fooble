namespace Fooble.Core

open System.Runtime.CompilerServices

/// <summary>
/// Provides extensions used for presentation.
/// </summary>
[<AutoOpen; Extension>]
type CoreExtensions = // TODO: need to provide overloads that allow overriding heading and severity

    (* Member Detail *)

    /// <summary>
    /// Constructs a message display read model from a member detail query result.
    /// </summary>
    /// <param name="result">The member detail query result to extend.</param>
    /// <param name="heading">The heading of the message display.</param>
    /// <param name="subHeading">The sub-heading of the message display.</param>
    /// <param name="statusCode">The status code of the message display.</param>
    /// <param name="severity">The severity of the message display.</param>
    /// <param name="message">The message to be displayed.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel
        (result:IMemberDetailQueryResult, heading, subHeading, statusCode, severity, message) =

        [ (notIsNull << box), "Result parameter was null" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        MessageDisplay.ReadModel.make heading subHeading statusCode severity message

    /// <summary>
    /// Constructs a message display read model from a member detail query result.
    /// </summary>
    /// <param name="result">The member detail query result to extend.</param>
    /// <param name="statusCode">The status code of the message display.</param>
    /// <param name="severity">The severity of the message display.</param>
    /// <param name="message">The message to be displayed.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel (result:IMemberDetailQueryResult, statusCode, severity, message) =
        CoreExtensions.ToMessageDisplayReadModel(result, "Member", "Detail", statusCode, severity, message)

    /// <summary>
    /// Constructs a message display read model from a member detail query result.
    /// </summary>
    /// <param name="result">The member detail query result to extend.</param>
    /// <param name="message">The message to be displayed.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel (result:IMemberDetailQueryResult, message) =
        match result with

        | MemberDetail.IsSuccess _ ->
            CoreExtensions.ToMessageDisplayReadModel(result, 200, MessageDisplay.Severity.informational, message)

        | MemberDetail.IsNotFound ->
            CoreExtensions.ToMessageDisplayReadModel(result, 404, MessageDisplay.Severity.warning, message)

    /// <summary>
    /// Constructs a message display read model from a member detail query result.
    /// </summary>
    /// <param name="result">The member detail query result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel (result:IMemberDetailQueryResult) =
        match result with

        | MemberDetail.IsSuccess _ ->
            CoreExtensions.ToMessageDisplayReadModel(result, "Member detail query was successful")

        | MemberDetail.IsNotFound ->
            CoreExtensions.ToMessageDisplayReadModel(result, "No matching member could be found.")

    (* Member List *)

    /// <summary>
    /// Constructs a message display read model from a member list query result.
    /// </summary>
    /// <param name="result">The member list query result to extend.</param>
    /// <param name="heading">The heading of the message display.</param>
    /// <param name="subHeading">The sub-heading of the message display.</param>
    /// <param name="statusCode">The status code of the message display.</param>
    /// <param name="severity">The severity of the message display.</param>
    /// <param name="message">The message to be displayed.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel
        (result:IMemberListQueryResult, heading, subHeading, statusCode, severity, message) =

        [ (notIsNull << box), "Result parameter was null" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        MessageDisplay.ReadModel.make heading subHeading statusCode severity message

    /// <summary>
    /// Constructs a message display read model from a member list query result.
    /// </summary>
    /// <param name="result">The member list query result to extend.</param>
    /// <param name="statusCode">The status code of the message display.</param>
    /// <param name="severity">The severity of the message display.</param>
    /// <param name="message">The message to be displayed.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel (result:IMemberListQueryResult, statusCode, severity, message) =
        CoreExtensions.ToMessageDisplayReadModel(result, "Member", "List", statusCode, severity, message)

    /// <summary>
    /// Constructs a message display read model from a member list query result.
    /// </summary>
    /// <param name="result">The member list query result to extend.</param>
    /// <param name="message">The message to be displayed.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel (result:IMemberListQueryResult, message) =
        CoreExtensions.ToMessageDisplayReadModel(result, 200, MessageDisplay.Severity.informational, message)

    /// <summary>
    /// Constructs a message display read model from a member list query result.
    /// </summary>
    /// <param name="result">The member list query result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel (result:IMemberListQueryResult) =
        match result with

        | MemberList.IsSuccess _ ->
            CoreExtensions.ToMessageDisplayReadModel(result, "Member list query was successful")

        | MemberList.IsNotFound -> CoreExtensions.ToMessageDisplayReadModel(result, "No members have yet been added.")

    (* Validation Result *)

    /// <summary>
    /// Constructs a message display read model from a validation result.
    /// </summary>
    /// <param name="result">The validation result to extend.</param>
    /// <param name="heading">The heading of the message display.</param>
    /// <param name="subHeading">The sub-heading of the message display.</param>
    /// <param name="statusCode">The status code of the message display.</param>
    /// <param name="severity">The severity of the message display.</param>
    /// <param name="message">The message to be displayed.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel
        (result:IValidationResult, heading, subHeading, statusCode, severity, message) =

        [ (notIsNull << box), "Result parameter was null" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        MessageDisplay.ReadModel.make heading subHeading statusCode severity message

    /// <summary>
    /// Constructs a message display read model from a validation result.
    /// </summary>
    /// <param name="result">The validation result to extend.</param>
    /// <param name="statusCode">The status code of the message display.</param>
    /// <param name="severity">The severity of the message display.</param>
    /// <param name="message">The message to be displayed.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel (result:IValidationResult, statusCode, severity, message) =
        CoreExtensions.ToMessageDisplayReadModel(result, "Validation", String.empty, statusCode, severity, message)

    /// <summary>
    /// Constructs a message display read model from a validation result.
    /// </summary>
    /// <param name="result">The validation result to extend.</param>
    /// <param name="message">The message to be displayed.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel (result:IValidationResult, message) =
        match result with

        | Validation.IsValid ->
            CoreExtensions.ToMessageDisplayReadModel(result, 200, MessageDisplay.Severity.informational, message)

        | Validation.IsInvalid _ ->
            CoreExtensions.ToMessageDisplayReadModel(result, 400, MessageDisplay.Severity.error, message)

    /// <summary>
    /// Constructs a message display read model from a validation result.
    /// </summary>
    /// <param name="result">The validation result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    [<Extension>]
    static member ToMessageDisplayReadModel (result:IValidationResult) =
        match result with

        | Validation.IsValid -> CoreExtensions.ToMessageDisplayReadModel(result, "Validation was successful")

        | Validation.IsInvalid (_, x) ->
            CoreExtensions.ToMessageDisplayReadModel(result,
                (sprintf "Validation was not successful and returned: \"%s\"" x))
