namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System
open System.Runtime.CompilerServices

[<RequireQualifiedAccess>]
module internal MemberListReadModel =

    [<DefaultAugmentation(false)>]
    type private MemberListItemReadModelImplementation =
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
        assertMemberId id
        assertMemberNickname nickname
        ItemReadModel(id, nickname) :> IMemberListItemReadModel

    [<DefaultAugmentation(false)>]
    type private MemberListReadModelImplementation =
        | ReadModel of members:seq<IMemberListItemReadModel>

        interface IMemberListReadModel with

            member this.Members
                with get() =
                    match this with
                    | ReadModel(members = xs) -> xs

    let make members =
        assert (Seq.isNotNullOrEmpty members)
        ReadModel(members) :> IMemberListReadModel

/// Provides presentation-related extension methods for member list.
[<RequireQualifiedAccess>]
[<Extension>]
module MemberListExtensions =

    /// <summary>
    /// Constructs a message display read model from a member list query result.
    /// </summary>
    /// <param name="result">The member list query result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IMemberListQueryResult) =

        [ (isNotNull << box), "Result parameter was null" ]
        |> validate result "result" |> enforce

        match result with
        | x when x.IsNotFound ->
              MessageDisplayReadModel.make "Member" "List" 200 MessageDisplayReadModel.informationalSeverity
                  "No members have yet been added."
        | _ -> invalidOp "Result was not unsuccessful"
