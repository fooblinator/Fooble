namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

/// Provides helpers for member change username command.
[<RequireQualifiedAccess>]
module MemberChangeUsernameCommand =

    [<DefaultAugmentation(false)>]
    type private MemberChangeUsernameCommandImpl =
        | Command of id:Guid * currentPassword:string * newUsername:string

        interface IMemberChangeUsernameCommand with

            member this.Id
                with get() =
                    match this with
                    | Command(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | Command(currentPassword = x) -> x

            member this.NewUsername
                with get() =
                    match this with
                    | Command(newUsername = x) -> x

    /// <summary>
    /// Constructs a member change username command.
    /// </summary>
    /// <param name="id">The id that represents the member.</param>
    /// <param name="currentPassword">The current password of the member.</param>
    /// <param name="newUsername">The new username of the member.</param>
    /// <returns>Returns a member change username command.</returns>
    [<CompiledName("Make")>]
    let make id currentPassword newUsername =
        ensureWith (validateMemberId id)
        ensureWith (validateMemberUsernameWith newUsername "newUsername" "New username")
        Command(id, currentPassword, newUsername) :> IMemberChangeUsernameCommand

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

    let internal successResult = Success :> IMemberChangeUsernameCommandResult
    let internal notFoundResult = NotFound :> IMemberChangeUsernameCommandResult
    let internal incorrectPasswordResult = IncorrectPassword :> IMemberChangeUsernameCommandResult
    let internal unavailableUsernameResult = UnavailableUsername :> IMemberChangeUsernameCommandResult

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

                match this.Context.GetMember(message.Id) with
                | None -> notFoundResult
                | Some(x) ->

                match Crypto.verify x.PasswordData message.CurrentPassword with
                | false -> incorrectPasswordResult
                | _ ->

                let usernameFound = this.Context.ExistsMemberUsername(message.NewUsername)

                match usernameFound with
                | true -> unavailableUsernameResult
                | _ ->

                x.Username <- message.NewUsername

                this.Context.SaveChanges()
                successResult

    let internal makeHandler context =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
#endif
        CommandHandler(context) :> IRequestHandler<IMemberChangeUsernameCommand, IMemberChangeUsernameCommandResult>
