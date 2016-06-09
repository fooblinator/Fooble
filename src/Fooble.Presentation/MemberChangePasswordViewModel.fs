namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System
open System.Runtime.CompilerServices

/// Provides presentation-related helpers for member change password.
[<Extension>]
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

    (* Extensions *)

    /// <summary>
    /// Constructs a member change password view model without passwords from an existing member change
    /// password view model.
    /// </summary>
    /// <param name="viewModel">The member change password view model to extend.</param>
    [<Extension>]
    [<CompiledName("Clean")>]
    let clean (viewModel:IMemberChangePasswordViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        makeInitial viewModel.Id

    /// <summary>
    /// Constructs a member change password command from a member change password view model.
    /// </summary>
    /// <param name="viewModel">The member change password view model to extend.</param>
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:IMemberChangePasswordViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangePasswordCommand.make viewModel.Id viewModel.CurrentPassword viewModel.NewPassword
