namespace Fooble.Core

open System.Runtime.CompilerServices

/// Provides extensions used for presentation.
[<AutoOpen; Extension>]
type CoreExtensions =

    (* Member Detail *)

    /// <summary>
    /// Constructs a message display read model from a member detail query result.
    /// </summary>
    /// <param name="result">The member detail query result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplay.ReadModel.Make"/> directly.</remarks>
    [<Extension>]
    static member ToMessageDisplayReadModel result =

        [ (isNotNull << box), "Result parameter was null" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        match result with
        | MemberDetail.IsSuccess _ -> invalidOp "Result was not unsuccessful"

        | MemberDetail.IsNotFound ->
            MessageDisplay.ReadModel.make "Member" "Detail" 404 MessageDisplay.Severity.warning
                "No matching member could be found."



    (* Member List *)

    /// <summary>
    /// Constructs a message display read model from a member list query result.
    /// </summary>
    /// <param name="result">The member list query result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplay.ReadModel.Make"/> directly.</remarks>
    [<Extension>]
    static member ToMessageDisplayReadModel result =

        [ (isNotNull << box), "Result parameter was null" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        match result with
        | MemberList.IsSuccess _ -> invalidOp "Result was not unsuccessful"

        | MemberList.IsNotFound ->
            MessageDisplay.ReadModel.make "Member" "List" 200 MessageDisplay.Severity.informational
                "No members have yet been added."



    (* Self-Service Register *)

    /// <summary>
    /// Constructs a message display read model from a self-service register command result.
    /// </summary>
    /// <param name="result">The self-service register command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplay.ReadModel.Make"/> directly.</remarks>
    [<Extension>]
    static member ToMessageDisplayReadModel result =

        [ (isNotNull << box), "Result parameter was null" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        match result with
        | SelfServiceRegister.IsSuccess -> invalidOp "Result was not unsuccessful"

        | SelfServiceRegister.IsUsernameUnavailable ->
            MessageDisplay.ReadModel.make "Self-Service" "Register" 400 MessageDisplay.Severity.warning
                "Requested username is unavailable."



    (* Validation Result *)

    /// <summary>
    /// Constructs a message display read model from a validation result.
    /// </summary>
    /// <param name="result">The validation result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on "invalid" results. For displaying a "valid" result, use
    /// <see cref="MessageDisplay.ReadModel.Make"/> directly.</remarks>
    [<Extension>]
    static member ToMessageDisplayReadModel result =

        [ (isNotNull << box), "Result parameter was null" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        match result with
        | Validation.IsValid ->  invalidOp "Result was not invalid"

        | Validation.IsInvalid (_, x) ->
            MessageDisplay.ReadModel.make "Validation" String.empty 400 MessageDisplay.Severity.error
                (sprintf "Validation was not successful and returned: \"%s\"" x)
