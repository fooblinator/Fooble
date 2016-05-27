﻿namespace Fooble.Core

open Fooble.Core.Persistence
open MediatR
open System
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides functionality used in the gathering and persisting of member details.
[<RequireQualifiedAccess>]
[<Extension>]
module SelfServiceRegister =

    (* Active Patterns *)

    let internal (|IsSuccess|IsUsernameUnavailable|IsEmailUnavailable|) (result:ISelfServiceRegisterCommandResult) =
        if result.IsSuccess then Choice1Of3 ()
        elif result.IsUsernameUnavailable then Choice2Of3 ()
        else Choice3Of3 () // IsEmailUnavailable



    (* Command *)

    /// Provides functionality used in the persisting of member details.
    [<RequireQualifiedAccess>]
    module Command =

        [<DefaultAugmentation(false)>]
        type private Implementation =
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

        /// <summary>
        /// Constructs a self-service register command.
        /// </summary>
        /// <param name="id">The id that will potentially represent the member.</param>
        /// <param name="username">The username of the potential member.</param>
        /// <param name="email">The email of the potential member.</param>
        /// <param name="nickname">The nickname of the potential member.</param>
        /// <returns>Returns a self-service register command.</returns>
        [<CompiledName("Make")>]
        let make id username email nickname =
            Validation.raiseIfInvalid (Member.validateId id)
            Validation.raiseIfInvalid (Member.validateUsername username)
            Validation.raiseIfInvalid (Member.validateEmail email)
            Validation.raiseIfInvalid (Member.validateNickname nickname)
            Command (id, username, email, nickname) :> ISelfServiceRegisterCommand



    (* View Model *)

    /// Provides functionality used in the gathering of member details.
    [<RequireQualifiedAccess>]
    module ViewModel =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | ViewModel of string * string * string

            interface ISelfServiceRegisterViewModel with

                member this.Username
                    with get() =
                        match this with
                        | ViewModel (x, _, _) -> x

                member this.Email
                    with get() =
                        match this with
                        | ViewModel (_, x, _) -> x

                member this.Nickname
                    with get() =
                        match this with
                        | ViewModel (_, _, x) -> x

        /// <summary>
        /// Represents an empty self-service register view model.
        /// </summary>
        /// <returns>Returns an empty self-service register view model.</returns>
        [<CompiledName("Empty")>]
        let empty = ViewModel (String.empty, String.empty, String.empty) :> ISelfServiceRegisterViewModel

        /// <summary>
        /// Constructs a self-service register view model.
        /// </summary>
        /// <param name="username">The username of the potential member.</param>
        /// <param name="email">The email of the potential member.</param>
        /// <param name="nickname">The nickname of the potential member.</param>
        /// <remarks>
        /// Does not validate parameters. This allows for re-construction of the view model with previously-submitted,
        /// and potentially invalid form data. Need to manually validate and handle submitted form data.
        /// </remarks>
        /// <returns>Returns a self-service register view model.</returns>
        [<CompiledName("Make")>]
        let make username email nickname = ViewModel (username, email, nickname) :> ISelfServiceRegisterViewModel



    (* Command Result *)

    [<RequireQualifiedAccess>]
    module internal CommandResult =

        [<DefaultAugmentation(false)>]
        type private Implementation =
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

        let internal success = Success :> ISelfServiceRegisterCommandResult
        let internal usernameUnavailable = UsernameUnavailable :> ISelfServiceRegisterCommandResult
        let internal emailUnavailable = EmailUnavailable :> ISelfServiceRegisterCommandResult



    (* Command Handler *)

    [<RequireQualifiedAccess>]
    module internal CommandHandler =

        [<DefaultAugmentation(false)>]
        [<NoComparison>]
        type private Implementation =
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
                    | (true, _) -> CommandResult.usernameUnavailable
                    | (_, true) -> CommandResult.emailUnavailable
                    | _ ->

                        MemberData(Id = message.Id, Username = message.Username, Email = message.Email,
                            Nickname = message.Nickname)
                        |> this.Context.MemberData.AddObject
                        ignore <| this.Context.SaveChanges()
                        CommandResult.success

        let internal make context =
            assert (isNotNull context)
            CommandHandler context :> IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult>



    (* Extensions *)

    [<Extension>]
    [<CompiledName("AddModelErrorIfNotSuccess")>]
    let addModelErrorIfNotSuccess result (modelState:ModelStateDictionary) =

        [ (isNotNull << box), "Result is required" ]
        |> Validation.validate result "result"
        |> Validation.raiseIfInvalid

        [ (isNotNull), "Model is required" ]
        |> Validation.validate modelState "modelState"
        |> Validation.raiseIfInvalid

        match result with
        | IsUsernameUnavailable -> modelState.AddModelError("username", "Username is unavailable")
        | IsEmailUnavailable -> modelState.AddModelError("email", "Email is already registered")
        | IsSuccess _ -> ()
