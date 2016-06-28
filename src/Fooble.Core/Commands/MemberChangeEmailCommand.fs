namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

[<RequireQualifiedAccess>]
module internal MemberChangeEmailCommand =

    [<DefaultAugmentation(false)>]
    type private MemberChangeEmailCommandImpl =
        | Command of id:Guid * currentPassword:string * email:string

        interface IMemberChangeEmailCommand with

            member this.Id
                with get() =
                    match this with
                    | Command(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | Command(currentPassword = x) -> x

            member this.Email
                with get() =
                    match this with
                    | Command(email = x) -> x

    let make id currentPassword email =
#if DEBUG
        assertWith (validateMemberId id)
        assertWith (validateRequired currentPassword "currentPassword" "Current password")
        assertWith (validateMemberEmail email)
#endif
        Command(id, currentPassword, email) :> IMemberChangeEmailCommand

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

    let successResult = Success :> IMemberChangeEmailCommandResult
    let notFoundResult = NotFound :> IMemberChangeEmailCommandResult
    let incorrectPasswordResult = IncorrectPassword :> IMemberChangeEmailCommandResult
    let unavailableEmailResult = UnavailableEmail :> IMemberChangeEmailCommandResult

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

                match this.Context.GetMember(message.Id, includeDeactivated = false) with
                | None -> notFoundResult
                | Some(x) ->

                match Crypto.verify x.PasswordData message.CurrentPassword with
                | false -> incorrectPasswordResult
                | _ ->

                match message.Email = x.Email with
                | true -> successResult
                | _ ->

                let emailFound = this.Context.ExistsMemberEmail(message.Email, includeDeactivated = true)

                match emailFound with
                | true -> unavailableEmailResult
                | _ ->

                x.Email <- message.Email

                this.Context.SaveChanges()
                successResult

    let makeHandler context =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
#endif
        CommandHandler(context) :> IRequestHandler<IMemberChangeEmailCommand, IMemberChangeEmailCommandResult>
