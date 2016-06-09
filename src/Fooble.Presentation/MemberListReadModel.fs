namespace Fooble.Presentation

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
        | ReadModel of members:seq<IMemberListItemReadModel>

        interface IMemberListReadModel with

            member this.Members
                with get() =
                    match this with
                    | ReadModel(members = xs) -> xs

    let make members =
#if DEBUG
        assertOn members "members" [ (Seq.isNotNullOrEmpty), "Members is required" ]
#endif
        ReadModel(members) :> IMemberListReadModel
