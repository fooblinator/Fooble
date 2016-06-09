﻿namespace Fooble.Presentation

open Fooble.Common
open System

[<RequireQualifiedAccess>]
module internal MemberListReadModel =

    [<DefaultAugmentation(false)>]
    type private MemberListItemReadModelImpl =
        | ItemReadModel of id:Guid * nickname:string

        interface IMemberListItemReadModel with

            member this.Id
                with get() =
                    match this with
                    | ItemReadModel(id = x) -> x

            member this.Nickname
                with get() =
                    match this with
                    | ItemReadModel(nickname = x) -> x

    let makeItem id nickname =
#if DEBUG
        assertWith (validateMemberId id)
        assertWith (validateMemberNickname nickname)
#endif
        ItemReadModel(id, nickname) :> IMemberListItemReadModel

    [<DefaultAugmentation(false)>]
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
        assertOn members "members" [ (Seq.isNotNullOrEmpty), "Members is required" ]
        assertOn memberCount "memberCount" [ ((<=) 0), "Member count is less than zero" ]
#endif
        ReadModel(members, memberCount) :> IMemberListReadModel
