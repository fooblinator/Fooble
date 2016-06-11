namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System
open System.Runtime.CompilerServices

/// Provides presentation-related helpers for member change email.
[<Extension>]
[<RequireQualifiedAccess>]
module MemberChangeEmailViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberChangeEmailViewModelImpl =
        | ViewModel of id:Guid * currentPassword:string * newEmail:string

        interface IMemberChangeEmailViewModel with

            member this.Id
                with get() =
                    match this with
                    | ViewModel(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | ViewModel(currentPassword = x) -> x

            member this.NewEmail
                with get() =
                    match this with
                    | ViewModel(newEmail = x) -> x

    /// <summary>
    /// Represents an initial member change email view model.
    /// </summary>
    /// <param name="id">The member id to operate on.</param>
    /// <returns>Returns an initial member change email view model.</returns>
    [<CompiledName("Make")>]
    let makeInitial id =
        ensureWith (validateMemberId id)
        ViewModel(id, String.empty, String.empty) :> IMemberChangeEmailViewModel

    let internal make id currentPassword newEmail =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, currentPassword, newEmail) :> IMemberChangeEmailViewModel

    (* Extensions *)

    /// <summary>
    /// Constructs a member change email view model without the current password from an existing member change
    /// email view model.
    /// </summary>
    /// <param name="viewModel">The member change email view model to extend.</param>
    [<Extension>]
    [<CompiledName("Clean")>]
    let clean (viewModel:IMemberChangeEmailViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        make viewModel.Id String.empty viewModel.NewEmail

    /// <summary>
    /// Constructs a member change email command from a member change email view model.
    /// </summary>
    /// <param name="viewModel">The member change email view model to extend.</param>
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:IMemberChangeEmailViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangeEmailCommand.make viewModel.Id viewModel.CurrentPassword viewModel.NewEmail
