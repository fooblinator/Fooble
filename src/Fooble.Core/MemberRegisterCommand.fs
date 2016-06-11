namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

/// Provides helpers for member register command.
[<RequireQualifiedAccess>]
module MemberRegisterCommand =

    [<DefaultAugmentation(false)>]
    type private MemberRegisterCommandImpl =
        | Command of id:Guid * username:string * password:string * email:string * nickname:string

        interface IMemberRegisterCommand with

            member this.Id
                with get() =
                    match this with
                    | Command(id = x) -> x

            member this.Username
                with get() =
                    match this with
                    | Command(username = x) -> x

            member this.Password
                with get() =
                    match this with
                    | Command(password = x) -> x

            member this.Email
                with get() =
                    match this with
                    | Command(email = x) -> x

            member this.Nickname
                with get() =
                    match this with
                    | Command(nickname = x) -> x

    /// <summary>
    /// Constructs a member register command.
    /// </summary>
    /// <param name="id">The id that will potentially represent the member.</param>
    /// <param name="username">The username of the potential member.</param>
    /// <param name="password">The password of the potential member.</param>
    /// <param name="email">The email of the potential member.</param>
    /// <param name="nickname">The nickname of the potential member.</param>
    /// <returns>Returns a member register command.</returns>
    [<CompiledName("Make")>]
    let make id username password email nickname =
        ensureWith (validateMemberId id)
        ensureWith (validateMemberUsername username)
        ensureWith (validateMemberPasswords password None)
        ensureWith (validateMemberEmail email)
        ensureWith (validateMemberNickname nickname)
        Command(id, username, password, email, nickname) :> IMemberRegisterCommand

    [<DefaultAugmentation(false)>]
    type private MemberRegisterCommandResultImpl =
        | Success
        | UnavailableUsername
        | UnavailableEmail

        interface IMemberRegisterCommandResult with

            member this.IsSuccess
                with get() =
                    match this with
                    | Success _ -> true
                    | _ -> false

            member this.IsUnavailableUsername
                with get() =
                    match this with
                    | UnavailableUsername -> true
                    | _ -> false

            member this.IsUnavailableEmail
                with get() =
                    match this with
                    | UnavailableEmail -> true
                    | _ -> false

    let internal successResult = Success :> IMemberRegisterCommandResult
    let internal unavailableUsernameResult = UnavailableUsername :> IMemberRegisterCommandResult
    let internal unavailableEmailResult = UnavailableEmail :> IMemberRegisterCommandResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberRegisterCommandHandlerImpl =
        | CommandHandler of context:IFoobleContext * memberDataFactory:MemberDataFactory

        member private this.Context
            with get() =
                match this with
                | CommandHandler(context = x) -> x

        member private this.MemberDataFactory
            with get() =
                match this with
                | CommandHandler(memberDataFactory = x) -> x

        interface IRequestHandler<IMemberRegisterCommand, IMemberRegisterCommandResult> with

            member this.Handle(message) =
#if DEBUG
                assertWith (validateRequired message "message" "Message")
#endif

                let usernameFound = this.Context.ExistsMemberUsername(message.Username)
                let emailFound = this.Context.ExistsMemberEmail(message.Email)

                match (usernameFound, emailFound) with
                | (true, _) -> unavailableUsernameResult
                | (_, true) -> unavailableEmailResult
                | _ ->

                let iterations = Random().Next(100000, 101000)
                let passwordData = Crypto.hash message.Password iterations

                this.MemberDataFactory.Invoke(message.Id, message.Username, passwordData, message.Email,
                    message.Nickname)
                |> this.Context.AddMember

                this.Context.SaveChanges()
                successResult

    let internal makeHandler context memberDataFactory =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
        assertWith (validateRequired memberDataFactory "memberDataFactory" "Member data factory")
#endif
        CommandHandler(context, memberDataFactory) :>
            IRequestHandler<IMemberRegisterCommand, IMemberRegisterCommandResult>
