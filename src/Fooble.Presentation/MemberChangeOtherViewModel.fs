namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System
open System.Runtime.CompilerServices

/// Provides presentation-related helpers for member change other.
[<Extension>]
[<RequireQualifiedAccess>]
module MemberChangeOtherViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberChangeOtherViewModelImpl =
        | ViewModel of id:Guid * newNickname:string

        interface IMemberChangeOtherViewModel with

            member this.Id
                with get() =
                    match this with
                    | ViewModel(id = x) -> x

            member this.NewNickname
                with get() =
                    match this with
                    | ViewModel(newNickname = x) -> x

    /// <summary>
    /// Represents an initial member change other view model.
    /// </summary>
    /// <param name="id">The member id to operate on.</param>
    /// <returns>Returns an initial member change other view model.</returns>
    [<CompiledName("Make")>]
    let makeInitial id =
        ensureWith (validateMemberId id)
        ViewModel(id, String.empty) :> IMemberChangeOtherViewModel

    let internal make id newNickname =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, newNickname) :> IMemberChangeOtherViewModel

    (* Extensions *)

    /// <summary>
    /// Constructs a member change other command from a member change other view model.
    /// </summary>
    /// <param name="viewModel">The member change other view model to extend.</param>
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:IMemberChangeOtherViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberChangeOtherCommand.make viewModel.Id viewModel.NewNickname
