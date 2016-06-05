namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related helpers for self-service register.
[<RequireQualifiedAccess>]
module SelfServiceRegisterViewModel =

    [<DefaultAugmentation(false)>]
    type private SelfServiceRegisterViewModelImplementation =
        | ViewModel of username:string * password:string * confirmPassword:string * email:string * nickname:string

        interface ISelfServiceRegisterViewModel with

            member this.Username
                with get() =
                    match this with
                    | ViewModel(username = x) -> x

            member this.Password
                with get() =
                    match this with
                    | ViewModel(password = x) -> x

            member this.ConfirmPassword
                with get() =
                    match this with
                    | ViewModel(confirmPassword = x) -> x

            member this.Email
                with get() =
                    match this with
                    | ViewModel(email = x) -> x

            member this.Nickname
                with get() =
                    match this with
                    | ViewModel(nickname = x) -> x

    /// <summary>
    /// Represents an empty self-service register view model.
    /// </summary>
    /// <returns>Returns an empty self-service register view model.</returns>
    [<CompiledName("Empty")>]
    let empty =
        ViewModel(String.empty, String.empty, String.empty, String.empty, String.empty) :>
            ISelfServiceRegisterViewModel

    let internal make username password confirmPassword email nickname =
        ViewModel(username, password, confirmPassword, email, nickname) :> ISelfServiceRegisterViewModel

/// Provides presentation-related extension methods for self-service register.
[<RequireQualifiedAccess>]
[<Extension>]
module SelfServiceRegisterExtensions =

    /// <summary>
    /// Adds a model error to the model state if the self-service register command result is not successful.
    /// </summary>
    /// <param name="result">The self-service register command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    [<CompiledName("AddModelErrorIfNotSuccess")>]
    let addModelErrorIfNotSuccess (result:ISelfServiceRegisterCommandResult) (modelState:ModelStateDictionary) =

        [ (box >> isNotNull), "Result is required" ]
        |> validate result "result" |> enforce

        [ (isNotNull), "Model state is required" ]
        |> validate modelState "modelState" |> enforce

        match result with
        | x when x.IsUsernameUnavailable ->
              modelState.AddModelError("username", "Username is unavailable")
        | x when x.IsEmailUnavailable ->
              modelState.AddModelError("email", "Email is already registered")
        | _ -> ()

    /// <summary>
    /// Constructs a message display read model from a self-service register command result.
    /// </summary>
    /// <param name="result">The self-service register command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:ISelfServiceRegisterCommandResult) =

        [ (box >> isNotNull), "Result is required" ]
        |> validate result "result" |> enforce

        match result with
        | x when x.IsUsernameUnavailable ->
              MessageDisplayReadModel.make "Self-Service" "Register" 400 MessageDisplayReadModel.warningSeverity
                  "Requested username is unavailable."
        | x when x.IsEmailUnavailable ->
              MessageDisplayReadModel.make "Self-Service" "Register" 400 MessageDisplayReadModel.warningSeverity
                  "Supplied email is already registered."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a self-service register command from a self-service register view model.
    /// </summary>
    /// <param name="viewModel">The self-service register view model to extend.</param>
    /// <param name="id">The member id to add to the command.</param>
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:ISelfServiceRegisterViewModel) id =

        [ (box >> isNotNull), "View model is required" ]
        |> validate viewModel "result" |> enforce

        SelfServiceRegisterCommand.make id viewModel.Username viewModel.Password viewModel.Email viewModel.Nickname
