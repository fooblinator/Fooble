namespace Fooble.Presentation

open Fooble.Common
open System

[<RequireQualifiedAccess>]
module internal MemberChangeEmailViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberChangeEmailViewModelImpl =
        | ViewModel of id:Guid * currentPassword:string * email:string

        interface IMemberChangeEmailViewModel with

            member this.Id
                with get() =
                    match this with
                    | ViewModel(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | ViewModel(currentPassword = x) -> x

            member this.Email
                with get() =
                    match this with
                    | ViewModel(email = x) -> x

    let make id currentPassword email =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, currentPassword, email) :> IMemberChangeEmailViewModel
