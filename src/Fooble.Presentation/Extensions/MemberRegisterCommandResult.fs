namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related extension methods for member register.
[<Extension>]
[<RequireQualifiedAccess>]
module MemberRegisterCommandResult =

    (* Extensions *)

    /// <summary>
    /// Adds a model error to the model state if the member register command result is not successful.
    /// </summary>
    /// <param name="result">The member register command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    [<CompiledName("AddModelErrors")>]
    let addModelErrors (result:IMemberRegisterCommandResult) (modelState:ModelStateDictionary) =
        ensureWith (validateRequired result "result" "Result")
        ensureWith (validateRequired modelState "modelState" "Model state")
        match result with
        | x when x.IsUsernameUnavailable -> modelState.AddModelError("username", "Username is unavailable")
        | x when x.IsEmailUnavailable -> modelState.AddModelError("email", "Email is already registered")
        | _ -> ()

    /// <summary>
    /// Constructs a message display read model from a member register command result.
    /// </summary>
    /// <param name="result">The member register command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IMemberRegisterCommandResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsUsernameUnavailable ->
              MessageDisplayReadModel.make "Member" "Register" 400 MessageDisplayReadModel.warningSeverity
                  "Requested username is unavailable."
        | x when x.IsEmailUnavailable ->
              MessageDisplayReadModel.make "Member" "Register" 400 MessageDisplayReadModel.warningSeverity
                  "Supplied email is already registered."
        | _ -> invalidOp "Result was not unsuccessful"
