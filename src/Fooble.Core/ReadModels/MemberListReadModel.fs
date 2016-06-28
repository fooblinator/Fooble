namespace Fooble.Presentation

open Fooble.Common
open System

[<RequireQualifiedAccess>]
module internal MemberListReadModel =

    [<DefaultAugmentation(false)>]
    type private MemberListItemReadModelImpl =
        | ItemReadModel of id:Guid * nickname:string * avatarData:string

        interface IMemberListItemReadModel with

            member this.Id
                with get() =
                    match this with
                    | ItemReadModel(id = x) -> x

            member this.Nickname
                with get() =
                    match this with
                    | ItemReadModel(nickname = x) -> x

            member this.AvatarData
                with get() =
                    match this with
                    | ItemReadModel(avatarData = x) -> x

    let makeItem id nickname avatarData =
#if DEBUG
        assertWith (validateMemberId id)
        assertWith (validateMemberNickname nickname)
#endif
        ItemReadModel(id, nickname, avatarData) :> IMemberListItemReadModel

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberListReadModelImpl =
        | ReadModel of members:seq<IMemberListItemReadModel> * memberCount:int

        interface IMemberListReadModel with

            member this.Members
                with get() =
                    match this with
                    | ReadModel(members = xs) -> xs

            member this.MemberCount
                with get() =
                    match this with
                    | ReadModel(memberCount = x) -> x

    let make members memberCount =
#if DEBUG
        assertOn members "members" [ (fun x -> not (isNull x || Seq.isEmpty x)), "Members is required" ]
        assertOn memberCount "memberCount" [ ((<=) 0), "Member count is less than zero" ]
#endif
        ReadModel(members, memberCount) :> IMemberListReadModel
