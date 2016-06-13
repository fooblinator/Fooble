namespace Fooble.Core

open Fooble.Common
open Fooble.Persistence
open MediatR
open System

/// Provides helpers for member change other command.
[<RequireQualifiedAccess>]
module MemberChangeOtherCommand =

    [<DefaultAugmentation(false)>]
    type private MemberChangeOtherCommandImpl =
        | Command of id:Guid * newNickname:string

        interface IMemberChangeOtherCommand with

            member this.Id
                with get() =
                    match this with
                    | Command(id = x) -> x

            member this.NewNickname
                with get() =
                    match this with
                    | Command(newNickname = x) -> x

    /// <summary>
    /// Constructs a member change other command.
    /// </summary>
    /// <param name="id">The id that represents the member.</param>
    /// <param name="newNickname">The new nickname of the member.</param>
    /// <returns>Returns a member change other command.</returns>
    [<CompiledName("Make")>]
    let make id newNickname =
        ensureWith (validateMemberId id)
        ensureWith (validateMemberNicknameWith newNickname "newNickname" "New nickname")
        Command(id, newNickname) :> IMemberChangeOtherCommand

    [<DefaultAugmentation(false)>]
    type private MemberChangeOtherCommandResultImpl =
        | Success
        | NotFound

        interface IMemberChangeOtherCommandResult with

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

    let internal successResult = Success :> IMemberChangeOtherCommandResult
    let internal notFoundResult = NotFound :> IMemberChangeOtherCommandResult

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberChangeOtherCommandHandlerImpl =
        | CommandHandler of context:IFoobleContext

        member private this.Context
            with get() =
                match this with
                | CommandHandler(context = x) -> x

        interface IRequestHandler<IMemberChangeOtherCommand, IMemberChangeOtherCommandResult> with

            member this.Handle(message) =
#if DEBUG
                assertWith (validateRequired message "message" "Message")
#endif

                match this.Context.GetMember(message.Id, includeDeactivated = false) with
                | None -> notFoundResult
                | Some(x) ->

                x.Nickname <- message.NewNickname

                this.Context.SaveChanges()
                successResult

    let internal makeHandler context =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
#endif
        CommandHandler(context) :> IRequestHandler<IMemberChangeOtherCommand, IMemberChangeOtherCommandResult>
