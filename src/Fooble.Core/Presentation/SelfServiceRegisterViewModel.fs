namespace Fooble.Presentation

open Fooble.Common

/// Contains a potential member's detailed information to be submitted for self-service registration.
type ISelfServiceRegisterViewModel =
    /// The username of the member.
    abstract Username:string with get
    /// The email of the member.
    abstract Email:string with get
    /// The nickname of the member.
    abstract Nickname:string with get

[<RequireQualifiedAccess>]
module internal SelfServiceRegisterViewModel =

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

    let internal empty =
        ViewModel (String.empty, String.empty, String.empty) :> ISelfServiceRegisterViewModel

    let internal make username email nickname =
        ViewModel (username, email, nickname) :> ISelfServiceRegisterViewModel
