namespace Fooble.Presentation

open Fooble.Common
open System

[<RequireQualifiedAccess>]
module internal MemberDetailReadModel =

    [<DefaultAugmentation(false)>]
    type private MemberDetailReadModelImpl =
        | ReadModel of id:Guid * username:string * email:string * nickname:string * avatarData:string *
              registered:DateTime * passwordChanged:DateTime

        interface IMemberDetailReadModel with

            member this.Id
                with get() =
                    match this with
                    | ReadModel(id = x) -> x

            member this.Username
                with get() =
                    match this with
                    | ReadModel(username = x) -> x

            member this.Email
                with get() =
                    match this with
                    | ReadModel(email = x) -> x

            member this.Nickname
                with get() =
                    match this with
                    | ReadModel(nickname = x) -> x

            member this.AvatarData
                with get() =
                    match this with
                    | ReadModel(avatarData = x) -> x

            member this.Registered
                with get() =
                    match this with
                    | ReadModel(registered = x) -> x

            member this.PasswordChanged
                with get() =
                    match this with
                    | ReadModel(passwordChanged = x) -> x

    let make id username email nickname avatarData registered passwordChanged =
#if DEBUG
        assertWith (validateMemberId id)
        assertWith (validateMemberUsername username)
        assertWith (validateMemberEmail email)
        assertWith (validateMemberNickname nickname)
#endif
        ReadModel(id, username, email, nickname, avatarData, registered, passwordChanged) :> IMemberDetailReadModel
