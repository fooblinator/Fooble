namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related extension methods for self-service register.
[<RequireQualifiedAccess>]
[<Extension>]
module SelfServiceRegisterExtensions =

    /// <summary>
    /// Adds a model error to the model state if the self-service register command result is not successful.
    /// </summary>
    /// <param name="result">The self-service register command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    [<CompiledName("AddModelErrorIfNotSuccess")>]
    let addModelErrorIfNotSuccess (result:ISelfServiceRegisterCommandResult) (modelState:ModelStateDictionary) =

        [ (isNotNull << box), "Result is required" ]
        |> validate result "result" |> enforce

        [ (isNotNull), "Model is required" ]
        |> validate modelState "modelState" |> enforce

        match result with
        | x when x.IsUsernameUnavailable ->
            modelState.AddModelError("username", "Username is unavailable")
        | x when x.IsEmailUnavailable ->
            modelState.AddModelError("email", "Email is already registered")
        | _ -> ()

    /// <summary>
    /// Constructs a message display read model from a self-service register command result.
    /// </summary>
    /// <param name="result">The self-service register command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplay.MakeReadModel"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:ISelfServiceRegisterCommandResult) =

        [ (isNotNull << box), "Result is required" ]
        |> validate result "result" |> enforce

        match result with
        | x when x.IsUsernameUnavailable ->
            MessageDisplayReadModel.make "Self-Service" "Register" 400 MessageDisplayReadModel.warningSeverity
                "Requested username is unavailable."
        | x when x.IsEmailUnavailable ->
            MessageDisplayReadModel.make "Self-Service" "Register" 400 MessageDisplayReadModel.warningSeverity
                "Supplied email is already registered."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a self-service register command from a self-service register view model.
    /// </summary>
    /// <param name="viewModel">The self-service register view model to extend.</param>
    /// <param name="id">The member id to add to the command.</param>
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:ISelfServiceRegisterViewModel) id =

        [ (isNotNull << box), "View model is required" ]
        |> validate viewModel "result" |> enforce

        SelfServiceRegisterCommand.make id viewModel.Username viewModel.Email viewModel.Nickname