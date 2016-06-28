namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System
open System.Runtime.CompilerServices

/// Provides presentation-related extension methods for view models.
[<Extension>]
type ViewModelExtensions =

    /// <summary>
    /// Removes the current password from an existing member change email view model.
    /// </summary>
    /// <param name="viewModel">The member change email view model to extend.</param>
    [<Extension>]
    static member Clean(viewModel:IMemberChangeEmailViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangeEmailViewModel.make viewModel.Id String.Empty viewModel.Email

    /// <summary>
    /// Removes the current password from an existing member change password view model.
    /// </summary>
    /// <param name="viewModel">The member change password view model to extend.</param>
    [<Extension>]
    static member Clean(viewModel:IMemberChangePasswordViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangePasswordViewModel.make viewModel.Id String.Empty String.Empty String.Empty

    /// <summary>
    /// Removes the current password from an existing member change username view model.
    /// </summary>
    /// <param name="viewModel">The member change username view model to extend.</param>
    [<Extension>]
    static member Clean(viewModel:IMemberChangeUsernameViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangeUsernameViewModel.make viewModel.Id String.Empty viewModel.Username

    /// <summary>
    /// Removes the current password from an existing member deactivate view model.
    /// </summary>
    /// <param name="viewModel">The member deactivate view model to extend.</param>
    [<Extension>]
    static member Clean(viewModel:IMemberDeactivateViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberDeactivateViewModel.make viewModel.Id String.Empty

    /// <summary>
    /// Removes the current password from an existing member register view model.
    /// </summary>
    /// <param name="viewModel">The member register view model to extend.</param>
    [<Extension>]
    static member Clean(viewModel:IMemberRegisterViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberRegisterViewModel.make viewModel.Username String.Empty String.Empty viewModel.Email
            viewModel.Nickname viewModel.AvatarData

    /// <summary>
    /// Constructs a member change email command from an existing member change email view model.
    /// </summary>
    /// <param name="viewModel">The member change email view model to extend.</param>
    [<Extension>]
    static member MapCommand(viewModel:IMemberChangeEmailViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangeEmailCommand.make viewModel.Id viewModel.CurrentPassword viewModel.Email

    /// <summary>
    /// Constructs a member change other command from an existing member change other view model.
    /// </summary>
    /// <param name="viewModel">The member change other view model to extend.</param>
    [<Extension>]
    static member MapCommand(viewModel:IMemberChangeOtherViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangeOtherCommand.make viewModel.Id viewModel.Nickname viewModel.AvatarData

    /// <summary>
    /// Constructs a member change password command from an existing member change password view model.
    /// </summary>
    /// <param name="viewModel">The member change password view model to extend.</param>
    [<Extension>]
    static member MapCommand(viewModel:IMemberChangePasswordViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangePasswordCommand.make viewModel.Id viewModel.CurrentPassword viewModel.Password

    /// <summary>
    /// Constructs a member change username command from an existing member change username view model.
    /// </summary>
    /// <param name="viewModel">The member change username view model to extend.</param>
    [<Extension>]
    static member MapCommand(viewModel:IMemberChangeUsernameViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangeUsernameCommand.make viewModel.Id viewModel.CurrentPassword viewModel.Username

    /// <summary>
    /// Constructs a member deactivate command from an existing member deactivate view model.
    /// </summary>
    /// <param name="viewModel">The member deactivate view model to extend.</param>
    [<Extension>]
    static member MapCommand(viewModel:IMemberDeactivateViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberDeactivateCommand.make viewModel.Id viewModel.CurrentPassword

    /// <summary>
    /// Constructs a member register command from an existing member register view model.
    /// </summary>
    /// <param name="viewModel">The member register view model to extend.</param>
    [<Extension>]
    static member MapCommand(viewModel:IMemberRegisterViewModel, id) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        ensureWith (validateMemberId id)
        MemberRegisterCommand.make id viewModel.Username viewModel.Password viewModel.Email viewModel.Nickname
            viewModel.AvatarData

    /// <summary>
    /// Randomly generates new avatar data for an existing member change other view model.
    /// </summary>
    /// <param name="viewModel">The member change other view model to extend.</param>
    [<Extension>]
    static member RandomizeAvatarData(viewModel:IMemberChangeOtherViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangeOtherViewModel.make viewModel.Id viewModel.Nickname (randomString 32)

    /// <summary>
    /// Randomly generates new avatar data for an existing member register view model.
    /// </summary>
    /// <param name="viewModel">The member register view model to extend.</param>
    [<Extension>]
    static member RandomizeAvatarData(viewModel:IMemberRegisterViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberRegisterViewModel.make viewModel.Username String.Empty String.Empty viewModel.Email viewModel.Nickname
            (randomString 32)
