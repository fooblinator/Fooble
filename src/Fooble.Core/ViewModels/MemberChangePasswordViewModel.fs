namespace Fooble.Presentation

open Fooble.Common
open System

[<RequireQualifiedAccess>]
module internal MemberChangePasswordViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberChangePasswordViewModelImpl =
        | ViewModel of id:Guid * currentPassword:string * password:string * confirmPassword:string

        interface IMemberChangePasswordViewModel with

            member this.Id
                with get() =
                    match this with
                    | ViewModel(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | ViewModel(currentPassword = x) -> x

            member this.Password
                with get() =
                    match this with
                    | ViewModel(password = x) -> x

            member this.ConfirmPassword
                with get() =
                    match this with
                    | ViewModel(confirmPassword = x) -> x

    let makeInitial id =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, String.Empty, String.Empty, String.Empty) :> IMemberChangePasswordViewModel

    let make id currentPassword password confirmPassword =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, currentPassword, password, confirmPassword) :> IMemberChangePasswordViewModel
