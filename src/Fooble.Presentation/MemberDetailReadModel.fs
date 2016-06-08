namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System
open System.Runtime.CompilerServices

[<RequireQualifiedAccess>]
module internal MemberDetailReadModel =

    [<DefaultAugmentation(false)>]
    type private MemberDetailReadModelImpl =
        | ReadModel of id:Guid * username:string * email:string * nickname:string * registered:DateTime *
              passwordChanged:DateTime

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

            member this.Registered
                with get() =
                    match this with
                    | ReadModel(registered = x) -> x

            member this.PasswordChanged
                with get() =
                    match this with
                    | ReadModel(passwordChanged = x) -> x

    let make id username email nickname registered passwordChanged =
#if DEBUG
        assertWith (validateMemberId id)
        assertWith (validateMemberUsername username)
        assertWith (validateMemberEmail email)
        assertWith (validateMemberNickname nickname)
#endif
        ReadModel(id, username, email, nickname, registered, passwordChanged) :> IMemberDetailReadModel

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
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IMemberDetailQueryResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
              MessageDisplayReadModel.make "Member" "Detail" 404 MessageDisplayReadModel.warningSeverity
                  "No matching member could be found."
        | _ -> invalidOp "Result was not unsuccessful"
