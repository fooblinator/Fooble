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

[<RequireQualifiedAccess>]
module internal SelfServiceRegisterCommand =

    let internal (|IsSuccess|IsUsernameUnavailable|IsEmailUnavailable|) (result:ISelfServiceRegisterCommandResult) =
        if result.IsSuccess then Choice1Of3 ()
        elif result.IsUsernameUnavailable then Choice2Of3 ()
        else Choice3Of3 () // IsEmailUnavailable

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

    let internal make id username email nickname =
        assert (Guid.isNotEmpty id)
        assert (String.isNotNullOrEmpty username)
        assert (String.isNotShorter 3 username)
        assert (String.isNotLonger 32 username)
        assert (String.isMatch "^[a-z0-9]+$" username)
        assert (String.isNotNullOrEmpty email)
        assert (String.isNotLonger 254 email)
        assert (String.isEmail email)
        assert (String.isNotNullOrEmpty nickname)
        assert (String.isNotLonger 64 nickname)
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
        | CommandHandler of IFoobleContext

        member private this.Context
            with get() =
                match this with
                | CommandHandler x -> x

        interface IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult> with

            member this.Handle(message) =
                assert (isNotNull <| box message)

                let usernameFound =
                    query { for x in this.Context.MemberData do
                            select x.Username
                            contains message.Username }

                let emailFound =
                    query { for x in this.Context.MemberData do
                            select x.Email
                            contains message.Email }

                match (usernameFound, emailFound) with
                | (true, _) -> usernameUnavailableResult
                | (_, true) -> emailUnavailableResult
                | _ ->

                    MemberData(Id = message.Id, Username = message.Username, Email = message.Email,
                        Nickname = message.Nickname)
                    |> this.Context.MemberData.AddObject
                    ignore <| this.Context.SaveChanges()
                    successResult

    let internal makeHandler context =
        assert (isNotNull context)
        CommandHandler context :> IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult>
