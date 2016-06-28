namespace Fooble.Presentation

open Fooble.Common
open System

[<RequireQualifiedAccess>]
module internal MemberChangeUsernameViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberChangeUsernameViewModelImpl =
        | ViewModel of id:Guid * currentPassword:string * username:string

        interface IMemberChangeUsernameViewModel with

            member this.Id
                with get() =
                    match this with
                    | ViewModel(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | ViewModel(currentPassword = x) -> x

            member this.Username
                with get() =
                    match this with
                    | ViewModel(username = x) -> x

    let make id currentPassword username =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, currentPassword, username) :> IMemberChangeUsernameViewModel
