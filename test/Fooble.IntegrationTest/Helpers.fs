namespace Fooble.IntegrationTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Fooble.Presentation
open FSharp.Configuration

type internal Settings = AppSettings<"App.config">

[<AutoOpen>]
module internal Helpers =

    let internal makeMemberData id username email nickname =
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

    let internal makeMemberDataFactory () = MemberDataFactory(makeMemberData)

    let private makeMemberDetailReadModel id username email nickname =
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

        { new IMemberDetailReadModel with
            member this.Id with get() = id
            member this.Username with get() = username
            member this.Email with get() = email
            member this.Nickname with get() = nickname }

    let internal makeMemberDetailReadModelFactory () = MemberDetailReadModelFactory(makeMemberDetailReadModel)

    let private makeMemberListItemReadModel id nickname =
        assert (Guid.isNotEmpty id)
        assert (String.isNotNullOrEmpty nickname)
        assert (String.isNotLonger 64 nickname)

        { new IMemberListItemReadModel with
            member this.Id with get() = id
            member this.Nickname with get() = nickname }

    let internal makeMemberListItemReadModelFactory () = MemberListItemReadModelFactory(makeMemberListItemReadModel)

    let private makeMemberListReadModel members =
        assert (Seq.isNotNullOrEmpty members)

        { new IMemberListReadModel with
            member this.Members with get() = members }

    let internal makeMemberListReadModelFactory () = MemberListReadModelFactory(makeMemberListReadModel)

    let internal makeSelfServiceRegisterViewModel username email nickname =
        { new ISelfServiceRegisterViewModel with
            member this.Username with get() = username
            member this.Email with get() = email
            member this.Nickname with get() = nickname }

    let internal makeKeyGenerator key =
        { new IKeyGenerator with
            member this.GenerateKey() =
                match key with
                | Some x -> x
                | None -> Guid.random () }