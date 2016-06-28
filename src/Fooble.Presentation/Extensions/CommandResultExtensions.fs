namespace Fooble.Core

open Fooble.Common
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related extension methods for command results.
[<Extension>]
type CommandResultExtensions =

    (* Extensions *)

    /// <summary>
    /// Adds a model error to the model state if the member change email command result is not successful.
    /// </summary>
    /// <param name="result">The member change email command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    static member AddModelErrors(result:IMemberChangeEmailCommandResult, modelState:ModelStateDictionary) =
        ensureWith (validateRequired result "result" "Result")
        ensureWith (validateRequired modelState "modelState" "Model state")
        match result with
        | x when x.IsIncorrectPassword -> modelState.AddModelError("currentPassword", "Current password is incorrect")
        | x when x.IsUnavailableEmail -> modelState.AddModelError("email", "Email is already registered")
        | _ -> ()

    /// <summary>
    /// Adds a model error to the model state if the member change password command result is not successful.
    /// </summary>
    /// <param name="result">The member change password command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    static member AddModelErrors(result:IMemberChangePasswordCommandResult, modelState:ModelStateDictionary) =
        ensureWith (validateRequired result "result" "Result")
        ensureWith (validateRequired modelState "modelState" "Model state")
        match result with
        | x when x.IsIncorrectPassword -> modelState.AddModelError("currentPassword", "Current password is incorrect")
        | _ -> ()

    /// <summary>
    /// Adds a model error to the model state if the member change username command result is not successful.
    /// </summary>
    /// <param name="result">The member change username command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    static member AddModelErrors(result:IMemberChangeUsernameCommandResult, modelState:ModelStateDictionary) =
        ensureWith (validateRequired result "result" "Result")
        ensureWith (validateRequired modelState "modelState" "Model state")
        match result with
        | x when x.IsIncorrectPassword -> modelState.AddModelError("currentPassword", "Current password is incorrect")
        | x when x.IsUnavailableUsername -> modelState.AddModelError("username", "Username is unavailable")
        | _ -> ()

    /// <summary>
    /// Adds a model error to the model state if the member deactivate command result is not successful.
    /// </summary>
    /// <param name="result">The member deactivate command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    static member AddModelErrors(result:IMemberDeactivateCommandResult, modelState:ModelStateDictionary) =
        ensureWith (validateRequired result "result" "Result")
        ensureWith (validateRequired modelState "modelState" "Model state")
        match result with
        | x when x.IsIncorrectPassword -> modelState.AddModelError("currentPassword", "Current password is incorrect")
        | _ -> ()

    /// <summary>
    /// Adds a model error to the model state if the member register command result is not successful.
    /// </summary>
    /// <param name="result">The member register command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    static member AddModelErrors(result:IMemberRegisterCommandResult, modelState:ModelStateDictionary) =
        ensureWith (validateRequired result "result" "Result")
        ensureWith (validateRequired modelState "modelState" "Model state")
        match result with
        | x when x.IsUnavailableUsername -> modelState.AddModelError("username", "Username is unavailable")
        | x when x.IsUnavailableEmail -> modelState.AddModelError("email", "Email is already registered")
        | _ -> ()
