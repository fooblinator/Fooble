namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related helpers for self-service change password.
[<RequireQualifiedAccess>]
module SelfServiceChangePasswordViewModel =

    [<DefaultAugmentation(false)>]
    type private SelfServiceChangePasswordViewModelImplementation =
        | ViewModel of currentPassword:string * newPassword:string * confirmPassword:string

        interface ISelfServiceChangePasswordViewModel with

            member this.CurrentPassword
                with get() =
                    match this with
                    | ViewModel(currentPassword = x) -> x

            member this.NewPassword
                with get() =
                    match this with
                    | ViewModel(newPassword = x) -> x

            member this.ConfirmPassword
                with get() =
                    match this with
                    | ViewModel(confirmPassword = x) -> x

    /// <summary>
    /// Represents an empty self-service change password view model.
    /// </summary>
    /// <returns>Returns an empty self-service change password view model.</returns>
    [<CompiledName("Empty")>]
    let empty = ViewModel(String.empty, String.empty, String.empty) :> ISelfServiceChangePasswordViewModel

    let internal make currentPassword newPassword confirmPassword =
        ViewModel(currentPassword, newPassword, confirmPassword) :> ISelfServiceChangePasswordViewModel

/// Provides presentation-related extension methods for self-service changePassword.
[<RequireQualifiedAccess>]
[<Extension>]
module SelfServiceChangePasswordExtensions =

    /// <summary>
    /// Adds a model error to the model state if the self-service change password command result is not successful.
    /// </summary>
    /// <param name="result">The self-service change password command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    [<CompiledName("AddModelErrorIfNotSuccess")>]
    let addModelErrorIfNotSuccess (result:ISelfServiceChangePasswordCommandResult) (modelState:ModelStateDictionary) =

        [ (box >> isNotNull), "Result is required" ]
        |> validate result "result" |> enforce

        [ (isNotNull), "Model state is required" ]
        |> validate modelState "modelState" |> enforce

        match result with
        | x when x.IsInvalid ->
              modelState.AddModelError("currentPassword", "Password is invalid")
        | _ -> ()

    /// <summary>
    /// Constructs a message display read model from a self-service change password command result.
    /// </summary>
    /// <param name="result">The self-service change password command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:ISelfServiceChangePasswordCommandResult) =

        [ (box >> isNotNull), "Result is required" ]
        |> validate result "result" |> enforce

        match result with
        | x when x.IsNotFound ->
              MessageDisplayReadModel.make "Self-Service" "Change Password" 404 MessageDisplayReadModel.warningSeverity
                  "No matching member could be found."
        | x when x.IsInvalid ->
              MessageDisplayReadModel.make "Self-Service" "Change Password" 400 MessageDisplayReadModel.warningSeverity
                  "Supplied password is invalid."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a self-service change password command from a self-service change password view model.
    /// </summary>
    /// <param name="viewModel">The self-service change password view model to extend.</param>
    /// <param name="id">The member id to add to the command.</param>
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:ISelfServiceChangePasswordViewModel) id =

        [ (box >> isNotNull), "View model is required" ]
        |> validate viewModel "result" |> enforce

        SelfServiceChangePasswordCommand.make id viewModel.CurrentPassword viewModel.NewPassword

    /// <summary>
    /// Constructs a self-service change password view model without passwords from an existing self-service change
    /// password view model.
    /// </summary>
    /// <param name="viewModel">The self-service change password view model to extend.</param>
    [<Extension>]
    [<CompiledName("Clean")>]
    let clean (viewModel:ISelfServiceChangePasswordViewModel) =

        [ (box >> isNotNull), "View model is required" ]
        |> validate viewModel "result" |> enforce

        SelfServiceChangePasswordViewModel.empty
