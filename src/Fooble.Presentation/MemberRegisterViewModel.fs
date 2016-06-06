namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related helpers for member register.
[<RequireQualifiedAccess>]
module MemberRegisterViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberRegisterViewModelImpl =
        | ViewModel of username:string * password:string * confirmPassword:string * email:string * nickname:string

        interface IMemberRegisterViewModel with

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
    /// Represents an empty member register view model.
    /// </summary>
    /// <returns>Returns an empty member register view model.</returns>
    [<CompiledName("Empty")>]
    let empty =
        ViewModel(String.empty, String.empty, String.empty, String.empty, String.empty) :> IMemberRegisterViewModel

    let internal make username password confirmPassword email nickname =
        ViewModel(username, password, confirmPassword, email, nickname) :> IMemberRegisterViewModel

/// Provides presentation-related extension methods for member register.
[<RequireQualifiedAccess>]
[<Extension>]
module MemberRegisterExtensions =

    /// <summary>
    /// Adds a model error to the model state if the member register command result is not successful.
    /// </summary>
    /// <param name="result">The member register command result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    [<CompiledName("AddModelErrorIfNotSuccess")>]
    let addModelErrorIfNotSuccess (result:IMemberRegisterCommandResult) (modelState:ModelStateDictionary) =

        [ (box >> isNotNull), "Result is required" ]
        |> validate result "result" |> enforce

        [ (isNotNull), "Model state is required" ]
        |> validate modelState "modelState" |> enforce

        match result with
        | x when x.IsUsernameUnavailable -> modelState.AddModelError("username", "Username is unavailable")
        | x when x.IsEmailUnavailable -> modelState.AddModelError("email", "Email is already registered")
        | _ -> ()

    /// <summary>
    /// Constructs a message display read model from a member register command result.
    /// </summary>
    /// <param name="result">The member register command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IMemberRegisterCommandResult) =

        [ (box >> isNotNull), "Result is required" ]
        |> validate result "result" |> enforce

        match result with
        | x when x.IsUsernameUnavailable ->
              MessageDisplayReadModel.make "Member" "Register" 400 MessageDisplayReadModel.warningSeverity
                  "Requested username is unavailable."
        | x when x.IsEmailUnavailable ->
              MessageDisplayReadModel.make "Member" "Register" 400 MessageDisplayReadModel.warningSeverity
                  "Supplied email is already registered."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a member register command from a member register view model.
    /// </summary>
    /// <param name="viewModel">The member register view model to extend.</param>
    /// <param name="id">The member id to add to the command.</param>
    [<Extension>]
    [<CompiledName("ToCommand")>]
    let toCommand (viewModel:IMemberRegisterViewModel) id =

        [ (box >> isNotNull), "View model is required" ]
        |> validate viewModel "result" |> enforce

        MemberRegisterCommand.make id viewModel.Username viewModel.Password viewModel.Email viewModel.Nickname

    /// <summary>
    /// Constructs a member register view model without passwords from an existing member register view
    /// model.
    /// </summary>
    /// <param name="viewModel">The member register view model to extend.</param>
    [<Extension>]
    [<CompiledName("Clean")>]
    let clean (viewModel:IMemberRegisterViewModel) =

        [ (box >> isNotNull), "View model is required" ]
        |> validate viewModel "result" |> enforce

        MemberRegisterViewModel.make viewModel.Username String.empty String.empty viewModel.Email viewModel.Nickname
