namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

/// Provides command-related helpers for self-service register.
[<RequireQualifiedAccess>]
module SelfServiceRegisterCommand =

    [<DefaultAugmentation(false)>]
    type private SelfServiceRegisterCommandImplementation =
        | Command of id:Guid * username:string * password:string * email:string * nickname:string

        interface ISelfServiceRegisterCommand with

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
    /// Constructs a self-service register command.
    /// </summary>
    /// <param name="id">The id that will potentially represent the member.</param>
    /// <param name="username">The username of the potential member.</param>
    /// <param name="password">The password of the potential member.</param>
    /// <param name="email">The email of the potential member.</param>
    /// <param name="nickname">The nickname of the potential member.</param>
    /// <returns>Returns a self-service register command.</returns>
    [<CompiledName("Make")>]
    let make id username password email nickname =
        enforce (Member.validateId id)
        enforce (Member.validateUsername username)
        enforce (Member.validatePassword password)
        enforce (Member.validateEmail email)
        enforce (Member.validateNickname nickname)
        Command(id, username, password, email, nickname) :> ISelfServiceRegisterCommand

    [<DefaultAugmentation(false)>]
    type private SelfServiceRegisterCommandResultImplementation =
        | Success
        | UsernameUnavailable
        | EmailUnavailable

        interface ISelfServiceRegisterCommandResult with

            member this.IsSuccess
                with get() =
                    match this with
                    | Success _ -> true
                    | _ -> false

            member this.IsUsernameUnavailable
                with get() =
                    match this with
                    | UsernameUnavailable -> true
                    | _ -> false

            member this.IsEmailUnavailable
                with get() =
                    match this with
                    | EmailUnavailable -> true
                    | _ -> false

    let internal successResult = Success :> ISelfServiceRegisterCommandResult
    let internal usernameUnavailableResult = UsernameUnavailable :> ISelfServiceRegisterCommandResult
    let internal emailUnavailableResult = EmailUnavailable :> ISelfServiceRegisterCommandResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private SelfServiceRegisterCommandHandlerImplementation =
        | CommandHandler of context:IFoobleContext * memberDataFactory:MemberDataFactory

        member private this.Context
            with get() =
                match this with
                | CommandHandler(context = x) -> x

        member private this.MemberDataFactory
            with get() =
                match this with
                | CommandHandler(memberDataFactory = x) -> x

        interface IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult> with

            member this.Handle(message) =
                assert (isNotNull <| box message)

                let usernameFound = this.Context.ExistsMemberUsername(message.Username)
                let emailFound = this.Context.ExistsMemberEmail(message.Email)

                match (usernameFound, emailFound) with
                | (true, _) -> usernameUnavailableResult
                | (_, true) -> emailUnavailableResult
                | _ ->

                let iterations = Random().Next(100000, 101000)
                let passwordData = Crypto.hash message.Password iterations

                this.MemberDataFactory.Invoke(message.Id, message.Username, passwordData, message.Email,
                    message.Nickname)
                |> this.Context.AddMember

                this.Context.SaveChanges()
                successResult

    let internal makeHandler context memberDataFactory =
        assert (isNotNull context)
        assert (isNotNull memberDataFactory)
        CommandHandler(context, memberDataFactory) :>
            IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult>
