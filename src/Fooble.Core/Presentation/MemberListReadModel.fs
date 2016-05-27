namespace Fooble.Presentation

open Fooble.Common
open System

/// Contains a single member's information, for the purpose of presentation as part of a list of members.
type IMemberListItemReadModel =
    /// The id that represents the member.
    abstract Id:Guid with get
    /// The nickname of the member.
    abstract Nickname:string with get

/// Contains a list of members' information, for the purpose of presentation.
type IMemberListReadModel =
    abstract Members:seq<IMemberListItemReadModel>

[<RequireQualifiedAccess>]
module internal MemberListReadModel =

    [<DefaultAugmentation(false)>]
    type private MemberListItemReadModelImplementation =
        | ItemReadModel of Guid * string

        interface IMemberListItemReadModel with

            member this.Id
                with get() =
                    match this with
                    | ItemReadModel (x, _) -> x

            member this.Nickname
                with get() =
                    match this with
                    | ItemReadModel (_, x) -> x

    let internal makeItem id nickname =
        assert (Guid.isNotEmpty id)
        assert (String.isNotNullOrEmpty nickname)
        assert (String.isNotLonger 64 nickname)
        ItemReadModel (id, nickname) :> IMemberListItemReadModel

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberListReadModelImplementation =
        | ReadModel of seq<IMemberListItemReadModel>

        interface IMemberListReadModel with

            member this.Members
                with get() =
                    match this with
                    | ReadModel xs -> xs

    let internal make members =
        assert (Seq.isNotNullOrEmpty members)
        ReadModel members :> IMemberListReadModel
