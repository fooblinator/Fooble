namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

/// Provides helpers for member deactivate command.
[<RequireQualifiedAccess>]
module MemberDeactivateCommand =

    [<DefaultAugmentation(false)>]
    type private MemberDeactivateCommandImpl =
        | Command of id:Guid * currentPassword:string

        interface IMemberDeactivateCommand with

            member this.Id
                with get() =
                    match this with
                    | Command(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | Command(currentPassword = x) -> x

    /// <summary>
    /// Constructs a member deactivate command.
    /// </summary>
    /// <param name="id">The id that represents the member.</param>
    /// <param name="currentPassword">The current password of the member.</param>
    /// <returns>Returns a member deactivate command.</returns>
    [<CompiledName("Make")>]
    let make id currentPassword =
        ensureWith (validateMemberId id)
        Command(id, currentPassword) :> IMemberDeactivateCommand

    [<DefaultAugmentation(false)>]
    type private MemberDeactivateCommandResultImpl =
        | Success
        | NotFound
        | IncorrectPassword

        interface IMemberDeactivateCommandResult with

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

    let internal successResult = Success :> IMemberDeactivateCommandResult
    let internal notFoundResult = NotFound :> IMemberDeactivateCommandResult
    let internal incorrectPasswordResult = IncorrectPassword :> IMemberDeactivateCommandResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberDeactivateCommandHandlerImpl =
        | CommandHandler of context:IFoobleContext

        member private this.Context
            with get() =
                match this with
                | CommandHandler(context = x) -> x

        interface IRequestHandler<IMemberDeactivateCommand, IMemberDeactivateCommandResult> with

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

                x.IsDeactivated <- true

                this.Context.SaveChanges()
                successResult

    let internal makeHandler context =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
#endif
        CommandHandler(context) :> IRequestHandler<IMemberDeactivateCommand, IMemberDeactivateCommandResult>
