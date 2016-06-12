namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System
open System.Runtime.CompilerServices

/// Provides presentation-related helpers for member deactivate.
[<Extension>]
[<RequireQualifiedAccess>]
module MemberDeactivateViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberDeactivateViewModelImpl =
        | ViewModel of id:Guid * currentPassword:string

        interface IMemberDeactivateViewModel with

            member this.Id
                with get() =
                    match this with
                    | ViewModel(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | ViewModel(currentPassword = x) -> x

    /// <summary>
    /// Represents an initial member deactivate view model.
    /// </summary>
    /// <param name="id">The member id to operate on.</param>
    /// <returns>Returns an initial member deactivate view model.</returns>
    [<CompiledName("Make")>]
    let makeInitial id =
        ensureWith (validateMemberId id)
        ViewModel(id, String.empty) :> IMemberDeactivateViewModel

    let internal make id currentPassword =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, currentPassword) :> IMemberDeactivateViewModel

    (* Extensions *)

    /// <summary>
    /// Constructs a member deactivate view model without the current password from an existing member deactivate
    /// view model.
    /// </summary>
    /// <param name="viewModel">The member deactivate view model to extend.</param>
    [<Extension>]
    [<CompiledName("Clean")>]
    let clean (viewModel:IMemberDeactivateViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        make viewModel.Id String.empty

    /// <summary>
    /// Constructs a member deactivate command from a member deactivate view model.
    /// </summary>
    /// <param name="viewModel">The member deactivate view model to extend.</param>
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:IMemberDeactivateViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberDeactivateCommand.make viewModel.Id viewModel.CurrentPassword
