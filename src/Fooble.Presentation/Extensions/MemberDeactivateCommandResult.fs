namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related extension methods for member deactivate.
[<Extension>]
[<RequireQualifiedAccess>]
module MemberDeactivateCommandResult =

    (* Extensions *)

    /// <summary>
    /// Adds a model error to the model state if the member deactivate command result is not successful.
    /// </summary>
    /// <param name="result">The member deactivate command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    [<CompiledName("AddModelErrors")>]
    let addModelErrors (result:IMemberDeactivateCommandResult) (modelState:ModelStateDictionary) =
        ensureWith (validateRequired result "result" "Result")
        ensureWith (validateRequired modelState "modelState" "Model state")
        match result with
        | x when x.IsIncorrectPassword -> modelState.AddModelError("currentPassword", "Current password is incorrect")
        | _ -> ()

    /// <summary>
    /// Constructs a message display read model from a member deactivate command result.
    /// </summary>
    /// <param name="result">The member deactivate command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IMemberDeactivateCommandResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
              MessageDisplayReadModel.make "Member" "Deactivate" 404 MessageDisplayReadModel.warningSeverity
                  "No matching member could be found."
        | x when x.IsIncorrectPassword ->
              MessageDisplayReadModel.make "Member" "Deactivate" 400 MessageDisplayReadModel.warningSeverity
                  "Supplied password is incorrect."
        | _ -> invalidOp "Result was not unsuccessful"
