namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related helpers for member change password.
[<RequireQualifiedAccess>]
module MemberChangePasswordViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberChangePasswordViewModelImpl =
        | ViewModel of id:Guid * currentPassword:string * newPassword:string * confirmPassword:string

        interface IMemberChangePasswordViewModel with

            member this.Id
                with get() =
                    match this with
                    | ViewModel(id = x) -> x

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
    /// Represents an initial member change password view model.
    /// </summary>
    /// <param name="id">The member id to operate on.</param>
    /// <returns>Returns an initial member change password view model.</returns>
    [<CompiledName("Make")>]
    let makeInitial id =
        ensureWith (validateMemberId id)
        ViewModel(id, String.empty, String.empty, String.empty) :> IMemberChangePasswordViewModel

    let internal make id currentPassword newPassword confirmPassword =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, currentPassword, newPassword, confirmPassword) :> IMemberChangePasswordViewModel

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
    [<CompiledName("AddModelErrors")>]
    let addModelErrors (result:IMemberChangePasswordCommandResult) (modelState:ModelStateDictionary) =
        ensureWith (validateRequired result "result" "Result")
        ensureWith (validateRequired modelState "modelState" "Model state")
        match result with
        | x when x.IsInvalid -> modelState.AddModelError("currentPassword", "Current password is incorrect")
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
        ensureWith (validateRequired result "result" "Result")
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
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:IMemberChangePasswordViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangePasswordCommand.make viewModel.Id viewModel.CurrentPassword viewModel.NewPassword

    /// <summary>
    /// Constructs a member change password view model without passwords from an existing member change
    /// password view model.
    /// </summary>
    /// <param name="viewModel">The member change password view model to extend.</param>
    [<Extension>]
    [<CompiledName("Clean")>]
    let clean (viewModel:IMemberChangePasswordViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangePasswordViewModel.makeInitial viewModel.Id
