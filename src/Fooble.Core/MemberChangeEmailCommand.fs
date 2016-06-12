namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

/// Provides helpers for member change email command.
[<RequireQualifiedAccess>]
module MemberChangeEmailCommand =

    [<DefaultAugmentation(false)>]
    type private MemberChangeEmailCommandImpl =
        | Command of id:Guid * currentPassword:string * newEmail:string

        interface IMemberChangeEmailCommand with

            member this.Id
                with get() =
                    match this with
                    | Command(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | Command(currentPassword = x) -> x

            member this.NewEmail
                with get() =
                    match this with
                    | Command(newEmail = x) -> x

    /// <summary>
    /// Constructs a member change email command.
    /// </summary>
    /// <param name="id">The id that represents the member.</param>
    /// <param name="currentPassword">The current password of the member.</param>
    /// <param name="newEmail">The new email of the member.</param>
    /// <returns>Returns a member change email command.</returns>
    [<CompiledName("Make")>]
    let make id currentPassword newEmail =
        ensureWith (validateMemberId id)
        ensureWith (validateMemberEmailWith newEmail "newEmail" "New email")
        Command(id, currentPassword, newEmail) :> IMemberChangeEmailCommand

    [<DefaultAugmentation(false)>]
    type private MemberChangeEmailCommandResultImpl =
        | Success
        | NotFound
        | IncorrectPassword
        | UnavailableEmail

        interface IMemberChangeEmailCommandResult with

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

            member this.IsUnavailableEmail
                with get() =
                    match this with
                    | UnavailableEmail -> true
                    | _ -> false

    let internal successResult = Success :> IMemberChangeEmailCommandResult
    let internal notFoundResult = NotFound :> IMemberChangeEmailCommandResult
    let internal incorrectPasswordResult = IncorrectPassword :> IMemberChangeEmailCommandResult
    let internal unavailableEmailResult = UnavailableEmail :> IMemberChangeEmailCommandResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberChangeEmailCommandHandlerImpl =
        | CommandHandler of context:IFoobleContext

        member private this.Context
            with get() =
                match this with
                | CommandHandler(context = x) -> x

        interface IRequestHandler<IMemberChangeEmailCommand, IMemberChangeEmailCommandResult> with

            member this.Handle(message) =
#if DEBUG
                assertWith (validateRequired message "message" "Message")
#endif

                match this.Context.GetMember(message.Id, considerDeactivated = false) with
                | None -> notFoundResult
                | Some(x) ->

                match Crypto.verify x.PasswordData message.CurrentPassword with
                | false -> incorrectPasswordResult
                | _ ->

                let emailFound = this.Context.ExistsMemberEmail(message.NewEmail, considerDeactivated = true)

                match emailFound with
                | true -> unavailableEmailResult
                | _ ->

                x.Email <- message.NewEmail

                this.Context.SaveChanges()
                successResult

    let internal makeHandler context =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
#endif
        CommandHandler(context) :> IRequestHandler<IMemberChangeEmailCommand, IMemberChangeEmailCommandResult>
