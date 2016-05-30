namespace Fooble.Presentation

open Fooble.Common

/// Provides presentation-related helpers for self-service register.
[<RequireQualifiedAccess>]
module SelfServiceRegisterViewModel =

    [<DefaultAugmentation(false)>]
    type private SelfServiceRegisterViewModelImplementation =
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
    let empty =
        ViewModel (String.empty, String.empty, String.empty) :> ISelfServiceRegisterViewModel

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
    let make username email nickname =
        ViewModel (username, email, nickname) :> ISelfServiceRegisterViewModel
