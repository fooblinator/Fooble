namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices

/// Provides presentation-related helpers for member register.
[<Extension>]
[<RequireQualifiedAccess>]
module MemberRegisterViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberRegisterViewModelImpl =
        | ViewModel of username:string * password:string * confirmPassword:string * email:string * nickname:string

        interface IMemberRegisterViewModel with

            member this.Username
                with get() =
                    match this with
                    | ViewModel(username = x) -> x

            member this.Password
                with get() =
                    match this with
                    | ViewModel(password = x) -> x

            member this.ConfirmPassword
                with get() =
                    match this with
                    | ViewModel(confirmPassword = x) -> x

            member this.Email
                with get() =
                    match this with
                    | ViewModel(email = x) -> x

            member this.Nickname
                with get() =
                    match this with
                    | ViewModel(nickname = x) -> x

    /// <summary>
    /// Represents an empty member register view model.
    /// </summary>
    /// <returns>Returns an empty member register view model.</returns>
    [<CompiledName("Empty")>]
    let empty =
        ViewModel(String.empty, String.empty, String.empty, String.empty, String.empty) :> IMemberRegisterViewModel

    let internal make username password confirmPassword email nickname =
        ViewModel(username, password, confirmPassword, email, nickname) :> IMemberRegisterViewModel

    (* Extensions *)

    /// <summary>
    /// Constructs a member register view model without passwords from an existing member register view
    /// model.
    /// </summary>
    /// <param name="viewModel">The member register view model to extend.</param>
    [<Extension>]
    [<CompiledName("Clean")>]
    let clean (viewModel:IMemberRegisterViewModel) =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        make viewModel.Username String.empty String.empty viewModel.Email viewModel.Nickname

    /// <summary>
    /// Constructs a member register command from a member register view model.
    /// </summary>
    /// <param name="viewModel">The member register view model to extend.</param>
    /// <param name="id">The member id to add to the command.</param>
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:IMemberRegisterViewModel) id =
        ensureWith (validateRequired viewModel "viewModel" "View model")
        MemberRegisterCommand.make id viewModel.Username viewModel.Password viewModel.Email viewModel.Nickname
