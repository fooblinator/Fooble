namespace Fooble.Presentation

open Fooble.Common
open System

[<RequireQualifiedAccess>]
module internal MemberChangeOtherViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberChangeOtherViewModelImpl =
        | ViewModel of id:Guid * nickname:string * avatarData:string

        interface IMemberChangeOtherViewModel with

            member this.Id
                with get() =
                    match this with
                    | ViewModel(id = x) -> x

            member this.Nickname
                with get() =
                    match this with
                    | ViewModel(nickname = x) -> x

            member this.AvatarData
                with get() =
                    match this with
                    | ViewModel(avatarData = x) -> x

    let make id nickname avatarData =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, nickname, avatarData) :> IMemberChangeOtherViewModel
