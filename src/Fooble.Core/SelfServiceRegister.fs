namespace Fooble.Core

open Fooble.Common
open Fooble.Presentation
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides functionality used in the gathering and persisting of member details.
[<RequireQualifiedAccess>]
[<Extension>]
module SelfServiceRegister =

    /// <summary>
    /// Represents an empty self-service register view model.
    /// </summary>
    /// <returns>Returns an empty self-service register view model.</returns>
    [<CompiledName("EmptyViewModel")>]
    let emptyViewModel = SelfServiceRegisterViewModel.empty

    /// <summary>
    /// Constructs a self-service register view model.
    /// </summary>
    /// <param name="username">The username of the potential member.</param>
    /// <param name="email">The email of the potential member.</param>
    /// <param name="nickname">The nickname of the potential member.</param>
    /// <remarks>
    /// Does not validate parameters. This allows for re-construction of the view model with previously-submitted,
    /// and potentially invalid form data. Need to manually validate and handle submitted form data.
    /// </remarks>
    /// <returns>Returns a self-service register view model.</returns>
    [<CompiledName("MakeViewModel")>]
    let makeViewModel username email nickname = SelfServiceRegisterViewModel.make username email nickname

    /// <summary>
    /// Constructs a self-service register command.
    /// </summary>
    /// <param name="id">The id that will potentially represent the member.</param>
    /// <param name="username">The username of the potential member.</param>
    /// <param name="email">The email of the potential member.</param>
    /// <param name="nickname">The nickname of the potential member.</param>
    /// <returns>Returns a self-service register command.</returns>
    [<CompiledName("MakeCommand")>]
    let makeCommand id username email nickname =
        ValidationResult.enforce (Member.validateId id)
        ValidationResult.enforce (Member.validateUsername username)
        ValidationResult.enforce (Member.validateEmail email)
        ValidationResult.enforce (Member.validateNickname nickname)
        SelfServiceRegisterCommand.make id username email nickname

    /// <summary>
    /// Adds a model error to the model state if the self-service register command result is not successful.
    /// </summary>
    /// <param name="result">The self-service register command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    [<CompiledName("AddModelErrorIfNotSuccess")>]
    let addModelErrorIfNotSuccess result (modelState:ModelStateDictionary) =

        [ (isNotNull << box), "Result is required" ]
        |> ValidationResult.get result "result"
        |> ValidationResult.enforce

        [ (isNotNull), "Model is required" ]
        |> ValidationResult.get modelState "modelState"
        |> ValidationResult.enforce

        match result with
        | SelfServiceRegisterCommand.IsUsernameUnavailable ->
            modelState.AddModelError("username", "Username is unavailable")
        | SelfServiceRegisterCommand.IsEmailUnavailable ->
            modelState.AddModelError("email", "Email is already registered")
        | SelfServiceRegisterCommand.IsSuccess _ -> ()

    /// <summary>
    /// Constructs a message display read model from a self-service register command result.
    /// </summary>
    /// <param name="result">The self-service register command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplay.MakeReadModel"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel result =

        [ (isNotNull << box), "Result parameter was null" ]
        |> ValidationResult.get result "result"
        |> ValidationResult.enforce

        match result with
        | SelfServiceRegisterCommand.IsSuccess -> invalidOp "Result was not unsuccessful"
        | SelfServiceRegisterCommand.IsUsernameUnavailable ->
            MessageDisplay.makeReadModel "Self-Service" "Register" 400 MessageDisplay.warningSeverity
                "Requested username is unavailable."
        | SelfServiceRegisterCommand.IsEmailUnavailable ->
            MessageDisplay.makeReadModel "Self-Service" "Register" 400 MessageDisplay.warningSeverity
                "Supplied email is already registered."
