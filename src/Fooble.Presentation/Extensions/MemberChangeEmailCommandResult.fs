namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related extension methods for member changeEmail.
[<Extension>]
[<RequireQualifiedAccess>]
module MemberChangeEmailCommandResult =

    (* Extensions *)

    /// <summary>
    /// Adds a model error to the model state if the member change email command result is not successful.
    /// </summary>
    /// <param name="result">The member change email command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    [<CompiledName("AddModelErrors")>]
    let addModelErrors (result:IMemberChangeEmailCommandResult) (modelState:ModelStateDictionary) =
        ensureWith (validateRequired result "result" "Result")
        ensureWith (validateRequired modelState "modelState" "Model state")
        match result with
        | x when x.IsIncorrectPassword -> modelState.AddModelError("currentPassword", "Current password is incorrect")
        | x when x.IsUnavailableEmail -> modelState.AddModelError("newEmail", "Email is already registered")
        | _ -> ()

    /// <summary>
    /// Constructs a message display read model from a member change email command result.
    /// </summary>
    /// <param name="result">The member change email command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IMemberChangeEmailCommandResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
              MessageDisplayReadModel.make "Member" "Change Email" 404 MessageDisplayReadModel.warningSeverity
                  "No matching member could be found."
        | x when x.IsIncorrectPassword ->
              MessageDisplayReadModel.make "Member" "Change Email" 400 MessageDisplayReadModel.warningSeverity
                  "Supplied password is incorrect."
        | x when x.IsUnavailableEmail ->
              MessageDisplayReadModel.make "Member" "Change Email" 400 MessageDisplayReadModel.warningSeverity
                  "Supplied email is already registered."
        | _ -> invalidOp "Result was not unsuccessful"
