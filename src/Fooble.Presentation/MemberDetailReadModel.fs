namespace Fooble.Presentation

open Fooble.Common
open System

[<RequireQualifiedAccess>]
module internal MemberDetailReadModel =

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MemberDetailReadModelImplementation =
        | ReadModel of Guid * string * string * string

        interface IMemberDetailReadModel with

            member this.Id
                with get() =
                    match this with
                    | ReadModel (x, _, _, _) -> x

            member this.Username
                with get() =
                    match this with
                    | ReadModel (_, x, _, _) -> x

            member this.Email
                with get() =
                    match this with
                    | ReadModel (_, _, x, _) -> x

            member this.Nickname
                with get() =
                    match this with
                    | ReadModel (_, _, _, x) -> x

    let internal make id username email nickname =
        assert (Guid.isNotEmpty id)
        assert (String.isNotNullOrEmpty username)
        assert (String.isNotShorter 3 username)
        assert (String.isNotLonger 32 username)
        assert (String.isMatch "^[a-z0-9]+$" username)
        assert (String.isNotNullOrEmpty email)
        assert (String.isNotLonger 254 email)
        assert (String.isEmail email)
        assert (String.isNotNullOrEmpty nickname)
        assert (String.isNotLonger 64 nickname)
        ReadModel (id, username, email, nickname) :> IMemberDetailReadModel
