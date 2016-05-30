namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

/// <summary>
/// Represents the status of a self-service registration command.
/// </summary>
/// <remarks>The result is only one of "success", "username unavailable" or "email unavailable".</remarks>
type ISelfServiceRegisterCommandResult =
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get
    /// Whether the result is "username unavailable" (or not).
    abstract IsUsernameUnavailable:bool with get
    /// Whether the result is "email unavailable" (or not).
    abstract IsEmailUnavailable:bool with get

/// Represents the self-service registration command, and contains the potential member's detailed information.
type ISelfServiceRegisterCommand =
    inherit IRequest<ISelfServiceRegisterCommandResult>
    /// The id that will potentially represent the member.
    abstract Id:Guid with get
    /// The username of the member.
    abstract Username:string with get
    /// The email of the member.
    abstract Email:string with get
    /// The nickname of the member.
    abstract Nickname:string with get

/// Provides command-related helpers for self-service register.
[<RequireQualifiedAccess>]
module SelfServiceRegisterCommand =

    [<DefaultAugmentation(false)>]
    type private SelfServiceRegisterCommandImplementation =
        | Command of Guid * string * string * string

        interface ISelfServiceRegisterCommand with

            member this.Id
                with get() =
                    match this with
                    | Command (x, _, _, _) -> x

            member this.Username
                with get() =
                    match this with
                    | Command (_, x, _, _) -> x

            member this.Email
                with get() =
                    match this with
                    | Command (_, _, x, _) -> x

            member this.Nickname
                with get() =
                    match this with
                    | Command (_, _, _, x) -> x

    /// <summary>
    /// Constructs a self-service register command.
    /// </summary>
    /// <param name="id">The id that will potentially represent the member.</param>
    /// <param name="username">The username of the potential member.</param>
    /// <param name="email">The email of the potential member.</param>
    /// <param name="nickname">The nickname of the potential member.</param>
    /// <returns>Returns a self-service register command.</returns>
    [<CompiledName("Make")>]
    let make id username email nickname =
        enforce (Member.validateId id)
        enforce (Member.validateUsername username)
        enforce (Member.validateEmail email)
        enforce (Member.validateNickname nickname)
        Command (id, username, email, nickname) :> ISelfServiceRegisterCommand

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
        | CommandHandler of IFoobleContext * MemberDataFactory

        member private this.Context
            with get() =
                match this with
                | CommandHandler (x, _) -> x

        member private this.MemberDataFactory
            with get() =
                match this with
                | CommandHandler (_, x) -> x

        interface IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult> with

            member this.Handle(message) =
                assert (isNotNull <| box message)

                let usernameFound = this.Context.ExistsMemberUsername(message.Username)
                let emailFound = this.Context.ExistsMemberEmail(message.Email)

                match (usernameFound, emailFound) with
                | (true, _) -> usernameUnavailableResult
                | (_, true) -> emailUnavailableResult
                | _ ->
                    this.MemberDataFactory.Invoke(message.Id, message.Username, message.Email, message.Nickname)
                    |> this.Context.AddMember
                    this.Context.SaveChanges()
                    successResult

    let internal makeHandler context memberDataFactory =
        assert (isNotNull context)
        assert (isNotNull memberDataFactory)
        CommandHandler (context, memberDataFactory) :>
            IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult>
