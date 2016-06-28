namespace Fooble.Presentation

open Fooble.Common
open System

[<RequireQualifiedAccess>]
module internal MemberRegisterViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberRegisterViewModelImpl =
        | ViewModel of username:string * password:string * confirmPassword:string * email:string * nickname:string *
              avatarData:string

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

            member this.AvatarData
                with get() =
                    match this with
                    | ViewModel(avatarData = x) -> x

    let makeInitial () =
        ViewModel(String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, randomString 32) :>
            IMemberRegisterViewModel

    let make username password confirmPassword email nickname avatarData =
        ViewModel(username, password, confirmPassword, email, nickname, avatarData) :> IMemberRegisterViewModel
