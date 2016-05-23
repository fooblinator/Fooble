namespace Fooble.Core

open System.Diagnostics
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
    [<Extension>]
    static member ToMessageDisplayReadModel(result:IMemberDetailQueryResult) =
        Debug.Assert(notIsNull <| box result, "Result parameter was null")
        match result with

        | MemberDetail.IsSuccess _ ->
            MessageDisplay.ReadModel.make "Member Detail" MessageDisplay.Severity.informational
                (Seq.singleton "Member detail query was successful")

        | MemberDetail.IsNotFound ->
            MessageDisplay.ReadModel.make "Member Detail" MessageDisplay.Severity.error
                (Seq.singleton "Member detail query was not successful and returned \"not found\"")

    (* Member List *)

    /// <summary>
    /// Constructs a message display read model from a member list query result.
    /// </summary>
    /// <param name="result">The member list query result to extend.</param>
    [<Extension>]
    static member ToMessageDisplayReadModel(result:IMemberListQueryResult) =
        Debug.Assert(notIsNull <| box result, "Result parameter was null")
        match result with

        | MemberList.IsSuccess _ ->
            MessageDisplay.ReadModel.make "Member List" MessageDisplay.Severity.informational
                (Seq.singleton "Member list query was successful")

        | MemberList.IsNotFound ->
            MessageDisplay.ReadModel.make "Member List" MessageDisplay.Severity.error
                (Seq.singleton "Member list query was not successful and returned \"not found\"")

    (* Self-Service Register *)

    /// <summary>
    /// Constructs a message display read model from a self-service register command result.
    /// </summary>
    /// <param name="result">The self-service register command result to extend.</param>
    [<Extension>]
    static member ToMessageDisplayReadModel(result:ISelfServiceRegisterCommandResult) =
        Debug.Assert(notIsNull <| box result, "Result parameter was null")
        match result with

        | SelfServiceRegister.IsSuccess _ ->
            MessageDisplay.ReadModel.make "Self-Service Register" MessageDisplay.Severity.informational
                (Seq.singleton "Self-service register command was successful")

        | SelfServiceRegister.IsDuplicateId ->
            MessageDisplay.ReadModel.make "Self-Service Register" MessageDisplay.Severity.error
                (Seq.singleton "Self-service register command was not successful and returned \"duplicate id\"")

    (* Validation Result *)

    /// <summary>
    /// Constructs a message display read model from a validation result.
    /// </summary>
    /// <param name="result">The validation result to extend.</param>
    [<Extension>]
    static member ToMessageDisplayReadModel(result:IValidationResult) =
        Debug.Assert(notIsNull <| box result, "Result parameter was null")
        match result with

        | Validation.IsValid ->
            MessageDisplay.ReadModel.make "Validation" MessageDisplay.Severity.informational
                (Seq.singleton "Validation was successful")

        | Validation.IsInvalid (_, x) ->
            MessageDisplay.ReadModel.make "Validation" MessageDisplay.Severity.error
                (Seq.singleton <| sprintf "Validation was not successful and returned: \"%s\"" x)
