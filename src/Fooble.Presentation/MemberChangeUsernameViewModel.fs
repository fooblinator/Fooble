namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System
open System.Runtime.CompilerServices

/// Provides presentation-related helpers for member change username.
[<Extension>]
[<RequireQualifiedAccess>]
module MemberChangeUsernameViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberChangeUsernameViewModelImpl =
        | ViewModel of id:Guid * currentPassword:string * newUsername:string

        interface IMemberChangeUsernameViewModel with

            member this.Id
                with get() =
                    match this with
                    | ViewModel(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | ViewModel(currentPassword = x) -> x

            member this.NewUsername
                with get() =
                    match this with
                    | ViewModel(newUsername = x) -> x

    /// <summary>
    /// Represents an initial member change username view model.
    /// </summary>
    /// <param name="id">The member id to operate on.</param>
    /// <returns>Returns an initial member change username view model.</returns>
    [<CompiledName("Make")>]
    let makeInitial id =
        ensureWith (validateMemberId id)
        ViewModel(id, String.empty, String.empty) :> IMemberChangeUsernameViewModel

    let internal make id currentPassword newUsername =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, currentPassword, newUsername) :> IMemberChangeUsernameViewModel

    (* Extensions *)

    /// <summary>
    /// Constructs a member change username view model without the current password from an existing member change
    /// username view model.
    /// </summary>
    /// <param name="viewModel">The member change username view model to extend.</param>
    [<Extension>]
    [<CompiledName("Clean")>]
    let clean (viewModel:IMemberChangeUsernameViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        make viewModel.Id String.empty viewModel.NewUsername

    /// <summary>
    /// Constructs a member change username command from a member change username view model.
    /// </summary>
    /// <param name="viewModel">The member change username view model to extend.</param>
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:IMemberChangeUsernameViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangeUsernameCommand.make viewModel.Id viewModel.CurrentPassword viewModel.NewUsername
