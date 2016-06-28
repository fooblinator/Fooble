namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

[<RequireQualifiedAccess>]
module internal MemberChangeUsernameCommand =

    [<DefaultAugmentation(false)>]
    type private MemberChangeUsernameCommandImpl =
        | Command of id:Guid * currentPassword:string * username:string

        interface IMemberChangeUsernameCommand with

            member this.Id
                with get() =
                    match this with
                    | Command(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | Command(currentPassword = x) -> x

            member this.Username
                with get() =
                    match this with
                    | Command(username = x) -> x

    let make id currentPassword username =
#if DEBUG
        assertWith (validateMemberId id)
        assertWith (validateRequired currentPassword "currentPassword" "Current password")
        assertWith (validateMemberUsername username)
#endif
        Command(id, currentPassword, username) :> IMemberChangeUsernameCommand

    [<DefaultAugmentation(false)>]
    type private MemberChangeUsernameCommandResultImpl =
        | Success
        | NotFound
        | IncorrectPassword
        | UnavailableUsername

        interface IMemberChangeUsernameCommandResult with

            member this.IsSuccess
                with get() =
                    match this with
                    | Success _ -> true
                    | _ -> false

            member this.IsNotFound
                with get() =
                    match this with
                    | NotFound -> true
                    | _ -> false

            member this.IsIncorrectPassword
                with get() =
                    match this with
                    | IncorrectPassword -> true
                    | _ -> false

            member this.IsUnavailableUsername
                with get() =
                    match this with
                    | UnavailableUsername -> true
                    | _ -> false

    let successResult = Success :> IMemberChangeUsernameCommandResult
    let notFoundResult = NotFound :> IMemberChangeUsernameCommandResult
    let incorrectPasswordResult = IncorrectPassword :> IMemberChangeUsernameCommandResult
    let unavailableUsernameResult = UnavailableUsername :> IMemberChangeUsernameCommandResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberChangeUsernameCommandHandlerImpl =
        | CommandHandler of context:IFoobleContext

        member private this.Context
            with get() =
                match this with
                | CommandHandler(context = x) -> x

        interface IRequestHandler<IMemberChangeUsernameCommand, IMemberChangeUsernameCommandResult> with

            member this.Handle(message) =
#if DEBUG
                assertWith (validateRequired message "message" "Message")
#endif

                match this.Context.GetMember(message.Id, includeDeactivated = false) with
                | None -> notFoundResult
                | Some(x) ->

                match Crypto.verify x.PasswordData message.CurrentPassword with
                | false -> incorrectPasswordResult
                | _ ->

                match message.Username = x.Username with
                | true -> successResult
                | _ ->

                let usernameFound = this.Context.ExistsMemberUsername(message.Username, includeDeactivated = true)

                match usernameFound with
                | true -> unavailableUsernameResult
                | _ ->

                x.Username <- message.Username

                this.Context.SaveChanges()
                successResult

    let makeHandler context =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
#endif
        CommandHandler(context) :> IRequestHandler<IMemberChangeUsernameCommand, IMemberChangeUsernameCommandResult>
