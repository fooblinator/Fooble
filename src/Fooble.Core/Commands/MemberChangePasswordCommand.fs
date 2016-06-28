namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

[<RequireQualifiedAccess>]
module internal MemberChangePasswordCommand =

    [<DefaultAugmentation(false)>]
    type private MemberChangePasswordCommandImpl =
        | Command of id:Guid * currentPassword:string * password:string

        interface IMemberChangePasswordCommand with

            member this.Id
                with get() =
                    match this with
                    | Command(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | Command(currentPassword = x) -> x

            member this.Password
                with get() =
                    match this with
                    | Command(password = x) -> x

    let make id currentPassword password =
#if DEBUG
        assertWith (validateMemberId id)
        assertWith (validateRequired currentPassword "currentPassword" "Current password")
        assertWith (validateMemberPasswords password None)
#endif
        Command(id, currentPassword, password) :> IMemberChangePasswordCommand

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

    let successResult = Success :> IMemberChangePasswordCommandResult
    let notFoundResult = NotFound :> IMemberChangePasswordCommandResult
    let incorrectPasswordResult = IncorrectPassword :> IMemberChangePasswordCommandResult

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

                match this.Context.GetMember(message.Id, includeDeactivated = false) with
                | None -> notFoundResult
                | Some(x) ->

                match Crypto.verify x.PasswordData message.CurrentPassword with
                | false -> incorrectPasswordResult
                | _ ->

                let iterations = Random().Next(100000, 101000)
                x.PasswordData <- Crypto.hash message.Password iterations
                x.PasswordChangedOn <- DateTime.UtcNow

                this.Context.SaveChanges()
                successResult

    let makeHandler context =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
#endif
        CommandHandler(context) :> IRequestHandler<IMemberChangePasswordCommand, IMemberChangePasswordCommandResult>
