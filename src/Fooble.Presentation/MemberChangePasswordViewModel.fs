namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related helpers for member change password.
[<RequireQualifiedAccess>]
module MemberChangePasswordViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberChangePasswordViewModelImpl =
        | ViewModel of currentPassword:string * newPassword:string * confirmPassword:string

        interface IMemberChangePasswordViewModel with

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
    /// Represents an empty member change password view model.
    /// </summary>
    /// <returns>Returns an empty member change password view model.</returns>
    [<CompiledName("Empty")>]
    let empty = ViewModel(String.empty, String.empty, String.empty) :> IMemberChangePasswordViewModel

    let internal make currentPassword newPassword confirmPassword =
        ViewModel(currentPassword, newPassword, confirmPassword) :> IMemberChangePasswordViewModel

/// Provides presentation-related extension methods for member changePassword.
[<RequireQualifiedAccess>]
[<Extension>]
module MemberChangePasswordExtensions =

    /// <summary>
    /// Adds a model error to the model state if the member change password command result is not successful.
    /// </summary>
    /// <param name="result">The member change password command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    [<CompiledName("AddModelErrorIfNotSuccess")>]
    let addModelErrorIfNotSuccess (result:IMemberChangePasswordCommandResult) (modelState:ModelStateDictionary) =

        [ (box >> isNotNull), "Result is required" ]
        |> validate result "result" |> enforce

        [ (isNotNull), "Model state is required" ]
        |> validate modelState "modelState" |> enforce

        match result with
        | x when x.IsInvalid -> modelState.AddModelError("currentPassword", "Password is invalid")
        | _ -> ()

    /// <summary>
    /// Constructs a message display read model from a member change password command result.
    /// </summary>
    /// <param name="result">The member change password command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IMemberChangePasswordCommandResult) =

        [ (box >> isNotNull), "Result is required" ]
        |> validate result "result" |> enforce

        match result with
        | x when x.IsNotFound ->
              MessageDisplayReadModel.make "Member" "Change Password" 404 MessageDisplayReadModel.warningSeverity
                  "No matching member could be found."
        | x when x.IsInvalid ->
              MessageDisplayReadModel.make "Member" "Change Password" 400 MessageDisplayReadModel.warningSeverity
                  "Supplied password is invalid."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a member change password command from a member change password view model.
    /// </summary>
    /// <param name="viewModel">The member change password view model to extend.</param>
    /// <param name="id">The member id to add to the command.</param>
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:IMemberChangePasswordViewModel) id =

        [ (box >> isNotNull), "View model is required" ]
        |> validate viewModel "result" |> enforce

        MemberChangePasswordCommand.make id viewModel.CurrentPassword viewModel.NewPassword

    /// <summary>
    /// Constructs a member change password view model without passwords from an existing member change
    /// password view model.
    /// </summary>
    /// <param name="viewModel">The member change password view model to extend.</param>
    [<Extension>]
    [<CompiledName("Clean")>]
    let clean (viewModel:IMemberChangePasswordViewModel) =

        [ (box >> isNotNull), "View model is required" ]
        |> validate viewModel "result" |> enforce

        MemberChangePasswordViewModel.empty
