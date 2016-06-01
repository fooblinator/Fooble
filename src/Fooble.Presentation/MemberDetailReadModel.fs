namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System
open System.Runtime.CompilerServices

[<RequireQualifiedAccess>]
module internal MemberDetailReadModel =

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberDetailReadModelImplementation =
        | ReadModel of Guid * string * string * string

        interface IMemberDetailReadModel with

            member this.Id
                with get() =
                    match this with
                    | ReadModel (x, _, _, _) -> x

            member this.Username
                with get() =
                    match this with
                    | ReadModel (_, x, _, _) -> x

            member this.Email
                with get() =
                    match this with
                    | ReadModel (_, _, x, _) -> x

            member this.Nickname
                with get() =
                    match this with
                    | ReadModel (_, _, _, x) -> x

    let make id username email nickname =
        assertMemberId id
        assertMemberUsername username
        assertMemberEmail email
        assertMemberNickname nickname
        ReadModel (id, username, email, nickname) :> IMemberDetailReadModel

/// Provides presentation-related extension methods for member detail.
[<RequireQualifiedAccess>]
[<Extension>]
module MemberDetailExtensions =

    /// <summary>
    /// Constructs a message display read model from a member detail query result.
    /// </summary>
    /// <param name="result">The member detail query result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplay.MakeReadModel"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IMemberDetailQueryResult) =

        [ (isNotNull << box), "Result parameter was null" ]
        |> validate result "result" |> enforce

        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.make "Member" "Detail" 404 MessageDisplayReadModel.warningSeverity
                "No matching member could be found."
        | _ -> invalidOp "Result was not unsuccessful"
