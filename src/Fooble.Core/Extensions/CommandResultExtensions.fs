namespace Fooble.Core

open Fooble.Common
open Fooble.Presentation
open System.Runtime.CompilerServices

/// Provides presentation-related extension methods for command results.
[<Extension>]
type CommandResultExtensions =

    (* Extensions *)

    /// <summary>
    /// Constructs a message display read model from an existing member change email command result.
    /// </summary>
    /// <param name="result">The member change email command result to extend.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberChangeEmailCommandResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.makeWarning "Member" "Change Email" 404
                "No matching member could be found."
        | x when x.IsIncorrectPassword ->
            MessageDisplayReadModel.makeWarning "Member" "Change Email" 400
                "Supplied password is incorrect."
        | x when x.IsUnavailableEmail ->
            MessageDisplayReadModel.makeWarning "Member" "Change Email" 400
                "Supplied email is already registered."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a message display read model from an existing member change other command result.
    /// </summary>
    /// <param name="result">The member change other command result to extend.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberChangeOtherCommandResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.makeWarning "Member" "Change Other" 404
                "No matching member could be found."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a message display read model from an existing member change password command result.
    /// </summary>
    /// <param name="result">The member change password command result to extend.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberChangePasswordCommandResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.makeWarning "Member" "Change Password" 404
                "No matching member could be found."
        | x when x.IsIncorrectPassword ->
            MessageDisplayReadModel.makeWarning "Member" "Change Password" 400
                "Supplied password is incorrect."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a message display read model from an existing member change username command result.
    /// </summary>
    /// <param name="result">The member change username command result to extend.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberChangeUsernameCommandResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.makeWarning "Member" "Change Username" 404
                "No matching member could be found."
        | x when x.IsIncorrectPassword ->
            MessageDisplayReadModel.makeWarning "Member" "Change Username" 400
                "Supplied password is incorrect."
        | x when x.IsUnavailableUsername ->
            MessageDisplayReadModel.makeWarning "Member" "Change Username" 400
                "Requested username is unavailable."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a message display read model from an existing member deactivate command result.
    /// </summary>
    /// <param name="result">The member deactivate command result to extend.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberDeactivateCommandResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.makeWarning "Member" "Deactivate" 404
                "No matching member could be found."
        | x when x.IsIncorrectPassword ->
            MessageDisplayReadModel.makeWarning "Member" "Deactivate" 400
                "Supplied password is incorrect."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a message display read model from an existing member register command result.
    /// </summary>
    /// <param name="result">The member register command result to extend.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberRegisterCommandResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsUnavailableUsername ->
            MessageDisplayReadModel.makeWarning "Member" "Register" 400
                "Requested username is unavailable."
        | x when x.IsUnavailableEmail ->
            MessageDisplayReadModel.makeWarning "Member" "Register" 400
                "Supplied email is already registered."
        | _ -> invalidOp "Result was not unsuccessful"
