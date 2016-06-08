namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

/// Provides helpers for member change password command.
[<RequireQualifiedAccess>]
module MemberChangePasswordCommand =

    [<DefaultAugmentation(false)>]
    type private MemberChangePasswordCommandImpl =
        | Command of id:Guid * currentPassword:string * newPassword:string

        interface IMemberChangePasswordCommand with

            member this.Id
                with get() =
                    match this with
                    | Command(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | Command(currentPassword = x) -> x

            member this.NewPassword
                with get() =
                    match this with
                    | Command(newPassword = x) -> x

    /// <summary>
    /// Constructs a member change password command.
    /// </summary>
    /// <param name="id">The id that represents the member.</param>
    /// <param name="currentPassword">The current password of the member.</param>
    /// <param name="newPassword">The new password of the member.</param>
    /// <returns>Returns a member change password command.</returns>
    [<CompiledName("Make")>]
    let make id currentPassword newPassword =
        ensureWith (validateMemberId id)
        ensureWith (validateMemberPasswordsWith currentPassword None "currentPassword" "Current password")
        ensureWith (validateMemberPasswordsWith newPassword None "newPassword" "New password")
        Command(id, currentPassword, newPassword) :> IMemberChangePasswordCommand

    [<DefaultAugmentation(false)>]
    type private MemberChangePasswordCommandResultImpl =
        | Success
        | NotFound
        | IncorrectPassword

        interface IMemberChangePasswordCommandResult with

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

    let internal successResult = Success :> IMemberChangePasswordCommandResult
    let internal notFoundResult = NotFound :> IMemberChangePasswordCommandResult
    let internal incorrectPasswordResult = IncorrectPassword :> IMemberChangePasswordCommandResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberChangePasswordCommandHandlerImpl =
        | CommandHandler of context:IFoobleContext

        member private this.Context
            with get() =
                match this with
                | CommandHandler(context = x) -> x

        interface IRequestHandler<IMemberChangePasswordCommand, IMemberChangePasswordCommandResult> with

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

                let iterations = Random().Next(100000, 101000)
                x.PasswordData <- Crypto.hash message.NewPassword iterations
                x.PasswordChanged <- DateTime.UtcNow

                this.Context.SaveChanges()
                successResult

    let internal makeHandler context =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
#endif
        CommandHandler(context) :> IRequestHandler<IMemberChangePasswordCommand, IMemberChangePasswordCommandResult>
