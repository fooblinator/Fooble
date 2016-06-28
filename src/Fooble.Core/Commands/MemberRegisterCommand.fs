namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

[<RequireQualifiedAccess>]
module internal MemberRegisterCommand =

    [<DefaultAugmentation(false)>]
    type private MemberRegisterCommandImpl =
        | Command of id:Guid * username:string * password:string * email:string * nickname:string * avatarData:string

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

            member this.AvatarData
                with get() =
                    match this with
                    | Command(avatarData = x) -> x

    let make id username password email nickname avatarData =
#if DEBUG
        assertWith (validateMemberId id)
        assertWith (validateMemberUsername username)
        assertWith (validateMemberPasswords password None)
        assertWith (validateMemberEmail email)
        assertWith (validateMemberNickname nickname)
        assertWith (validateMemberAvatarData avatarData)
#endif
        Command(id, username, password, email, nickname, avatarData) :> IMemberRegisterCommand

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

    let successResult = Success :> IMemberRegisterCommandResult
    let unavailableUsernameResult = UnavailableUsername :> IMemberRegisterCommandResult
    let unavailableEmailResult = UnavailableEmail :> IMemberRegisterCommandResult

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

                let usernameFound = this.Context.ExistsMemberUsername(message.Username, includeDeactivated = true)
                let emailFound = this.Context.ExistsMemberEmail(message.Email, includeDeactivated = true)

                match (usernameFound, emailFound) with
                | (true, _) -> unavailableUsernameResult
                | (_, true) -> unavailableEmailResult
                | _ ->

                let iterations = Random().Next(100000, 101000)
                let passwordData = Crypto.hash message.Password iterations

                this.MemberDataFactory.Invoke(message.Id, message.Username, passwordData, message.Email,
                    message.Nickname, message.AvatarData, DateTime.UtcNow, DateTime.UtcNow, None)
                |> this.Context.AddMember

                this.Context.SaveChanges()
                successResult

    let makeHandler context memberDataFactory =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
        assertWith (validateRequired memberDataFactory "memberDataFactory" "Member data factory")
#endif
        CommandHandler(context, memberDataFactory) :>
            IRequestHandler<IMemberRegisterCommand, IMemberRegisterCommandResult>
