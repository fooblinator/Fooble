namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

/// Provides command-related helpers for self-service change password.
[<RequireQualifiedAccess>]
module SelfServiceChangePasswordCommand =

    [<DefaultAugmentation(false)>]
    type private SelfServiceChangePasswordCommandImplementation =
        | Command of id:Guid * currentPassword:string * newPassword:string

        interface ISelfServiceChangePasswordCommand with

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

    let internal validateCurrentPassword currentPassword =
        match Member.validatePassword currentPassword with
        | x when x.IsValid -> x
        | x ->

        let paramName = x.ParamName.Replace("password", "currentPassword")
        let message = x.Message.Replace("Password ", "Current password ")
        ValidationResult.makeInvalid paramName message

    let internal validateNewPassword newPassword =
        match Member.validatePassword newPassword with
        | x when x.IsValid -> x
        | x ->

        let paramName = x.ParamName.Replace("password", "newPassword")
        let message = x.Message.Replace("Password ", "New password ")
        ValidationResult.makeInvalid paramName message

    /// <summary>
    /// Constructs a self-service change password command.
    /// </summary>
    /// <param name="id">The id that represents the member.</param>
    /// <param name="currentPassword">The current password of the member.</param>
    /// <param name="newPassword">The new password of the member.</param>
    /// <returns>Returns a self-service change password command.</returns>
    [<CompiledName("Make")>]
    let make id currentPassword newPassword =
        enforce (Member.validateId id)
        enforce (validateCurrentPassword currentPassword)
        enforce (validateNewPassword newPassword)
        Command(id, currentPassword, newPassword) :> ISelfServiceChangePasswordCommand

    [<DefaultAugmentation(false)>]
    type private SelfServiceChangePasswordCommandResultImplementation =
        | Success
        | NotFound
        | Invalid

        interface ISelfServiceChangePasswordCommandResult with

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

            member this.IsInvalid
                with get() =
                    match this with
                    | Invalid -> true
                    | _ -> false

    let internal successResult = Success :> ISelfServiceChangePasswordCommandResult
    let internal notFoundResult = NotFound :> ISelfServiceChangePasswordCommandResult
    let internal invalidResult = Invalid :> ISelfServiceChangePasswordCommandResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private SelfServiceChangePasswordCommandHandlerImplementation =
        | CommandHandler of context:IFoobleContext

        member private this.Context
            with get() =
                match this with
                | CommandHandler(context = x) -> x

        interface IRequestHandler<ISelfServiceChangePasswordCommand, ISelfServiceChangePasswordCommandResult> with

            member this.Handle(message) =
                assert (isNotNull <| box message)

                match this.Context.GetMember(message.Id) with
                | None -> notFoundResult
                | Some x ->

                match Crypto.verify x.PasswordData message.CurrentPassword with
                | false -> invalidResult
                | _ ->

                let iterations = Random().Next(100000, 101000)
                x.PasswordData <- Crypto.hash message.NewPassword iterations
                x.PasswordChanged <- DateTime.UtcNow

                this.Context.SaveChanges()
                successResult

    let internal makeHandler context =
        CommandHandler(context) :>
            IRequestHandler<ISelfServiceChangePasswordCommand, ISelfServiceChangePasswordCommandResult>
