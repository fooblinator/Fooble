namespace Fooble.IntegrationTest

open Fooble.Common
open Fooble.Core
open FSharp.Configuration
open Fooble.Persistence

type internal Settings = AppSettings<"App.config">

[<AutoOpen>]
module internal IntegrationTestHelpers =

    let makeTestKeyGenerator key =
        { new IKeyGenerator with
            member this.GenerateKey() =
                match key with
                | Some x -> x
                | None -> Guid.random () }

    let makeTestMemberData id username email nickname =
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

        let idRef = ref id
        let usernameRef = ref username
        let emailRef = ref email
        let nicknameRef = ref nickname

        { new IMemberData with

              member this.Id
                  with get () = !idRef
                  and set (v) = idRef := v

              member this.Username
                  with get () = !usernameRef
                  and set (v) = usernameRef := v

              member this.Email
                  with get () = !emailRef
                  and set (v) = emailRef := v

              member this.Nickname
                  with get () = !nicknameRef
                  and set (v) = nicknameRef := v }

//    let makeTestMemberDataFactory () = MemberDataFactory(makeTestMemberData)
