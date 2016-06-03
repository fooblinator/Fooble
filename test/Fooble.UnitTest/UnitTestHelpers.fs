namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Fooble.Presentation
open Moq
open Moq.FSharp.Extensions
open Swensen.Unquote
open System
open System.Collections.Specialized
open System.Diagnostics
open System.Web
open System.Web.Mvc

[<DebuggerStepThrough>]
[<AutoOpen>]
module internal UnitTestHelpers =

    let private bindModel<'T> formValues =
        let modelType = typeof<'T>

        let formValues' = NameValueCollection()
        Map.iter (fun k v -> formValues'.Add(k, v)) formValues

        let valueProvider = NameValueCollectionValueProvider(formValues', null)

        let modelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, modelType)

        let bindingContext =
            ModelBindingContext(ModelName = modelType.Name, ValueProvider = valueProvider,
                ModelMetadata = modelMetadata)

        let httpRequestMock = Mock<HttpRequestBase>()
        httpRequestMock.SetupFunc(fun x -> x.Form).Returns(formValues').End

        let httpContextMock = Mock<HttpContextBase>()
        httpContextMock.SetupFunc(fun x -> x.Request).Returns(httpRequestMock.Object).End

        let controllerContext = ControllerContext()
        controllerContext.HttpContext <- httpContextMock.Object

        let binder = FoobleModelBinder()

        (binder.BindModel(controllerContext, bindingContext) :?> 'T, bindingContext.ModelState)

    let bindSelfServiceRegisterViewModel username password confirmPassword email nickname =
        Map.empty
            .Add("Username", username)
            .Add("Password", password)
            .Add("ConfirmPassword", confirmPassword)
            .Add("Email", email)
            .Add("Nickname", nickname)
        |> bindModel<ISelfServiceRegisterViewModel>

    let bindSelfServiceRegisterViewModel2 username password confirmPassword email nickname =
        fst (bindSelfServiceRegisterViewModel username password confirmPassword email nickname)

    let fixInvalidArgMessage (message:string) =
        let i = message.IndexOf("Parameter name: ")
        message.Remove(i).Trim()

    let private makeBadPasswordWith charset =
        let chars = Set.toList charset
        let charsLen = chars.Length
        let random = Random()
        fun len -> [| for _ in 0..len-1 -> chars.[random.Next(charsLen)] |] |> String

    let makeBadPasswordWithoutDigits =
        let charset =
            Set.ofList [ '0' .. '9' ]
            |> Set.difference Password.charset
        let rec generate len =
            let res = makeBadPasswordWith charset len
            match (Password.hasLowerAlphas res, Password.hasUpperAlphas res, Password.hasSpecialChars res) with
            | (true, true, true) -> res
            | _ -> generate len
        fun len -> assert (len > 0); generate len

    let makeBadPasswordWithoutLowerAlphas =
        let charset =
            Set.ofList [ 'a' .. 'z' ]
            |> Set.difference Password.charset
        let rec generate len =
            let res = makeBadPasswordWith charset len
            match (Password.hasDigits res, Password.hasUpperAlphas res, Password.hasSpecialChars res) with
            | (true, true, true) -> res
            | _ -> generate len
        fun len -> assert (len > 0); generate len

    let makeBadPasswordWithoutUpperAlphas =
        let charset =
            Set.ofList [ 'A' .. 'Z' ]
            |> Set.difference Password.charset
        let rec generate len =
            let res = makeBadPasswordWith charset len
            match (Password.hasDigits res, Password.hasLowerAlphas res, Password.hasSpecialChars res) with
            | (true, true, true) -> res
            | _ -> generate len
        fun len -> assert (len > 0); generate len

    let makeBadPasswordWithoutSpecialChars =
        let charset =
            String.toArray Password.specialCharsPattern
            |> Set.ofArray
            |> Set.difference Password.charset
        let rec generate len =
            let res = makeBadPasswordWith charset len
            match (Password.hasDigits res, Password.hasLowerAlphas res, Password.hasUpperAlphas res) with
            | (true, true, true) -> res
            | _ -> generate len
        fun len -> assert (len > 0); generate len

    let makeBadPasswordWithInvalidChars len =
        Password.random (len - 2) |> sprintf "%c%s" Char.MinValue

    let makeTestKeyGenerator key =
        { new IKeyGenerator with
            member this.GenerateKey() =
                match key with
                | Some x -> x
                | None -> Guid.random () }

    let makeTestMemberData id username password email nickname =
        assertMemberId id
        assertMemberUsername username
        assertMemberPassword password
        assertMemberEmail email
        assertMemberNickname nickname

        let idRef = ref id
        let usernameRef = ref username
        let passwordRef = ref password
        let emailRef = ref email
        let nicknameRef = ref nickname

        { new IMemberData with

              member this.Id
                  with get () = !idRef
                  and set (v) = idRef := v

              member this.Username
                  with get () = !usernameRef
                  and set (v) = usernameRef := v

              member this.Password
                  with get () = !passwordRef
                  and set (v) = passwordRef := v

              member this.Email
                  with get () = !emailRef
                  and set (v) = emailRef := v

              member this.Nickname
                  with get () = !nicknameRef
                  and set (v) = nicknameRef := v }

    let makeTestMemberDataFactory () =
        MemberDataFactory(makeTestMemberData)

    let makeTestMemberDetailReadModel id username email nickname =
        assertMemberId id
        assertMemberUsername username
        assertMemberEmail email
        assertMemberNickname nickname

        { new IMemberDetailReadModel with
            member this.Id with get() = id
            member this.Username with get() = username
            member this.Email with get() = email
            member this.Nickname with get() = nickname }

    let makeTestMemberDetailReadModelFactory () =
        MemberDetailReadModelFactory(makeTestMemberDetailReadModel)

    let makeTestMemberListItemReadModel id nickname =
        assertMemberId id
        assertMemberNickname nickname

        { new IMemberListItemReadModel with
            member this.Id with get() = id
            member this.Nickname with get() = nickname }

    let makeTestMemberListItemReadModelFactory () =
        MemberListItemReadModelFactory(makeTestMemberListItemReadModel)

    let makeTestMemberListReadModel members =
        assert (Seq.isNotNullOrEmpty members)

        { new IMemberListReadModel with
            member this.Members with get() = members }

    let makeTestMemberListReadModelFactory () =
        MemberListReadModelFactory(makeTestMemberListReadModel)

    let testMessageDisplayReadModel (actual:IMessageDisplayReadModel) expectedHeading expectedSubHeading
        expectedStatusCode expectedSeverity expectedMessage =

        test <@ actual.Heading = expectedHeading @>
        test <@ actual.SubHeading = expectedSubHeading @>
        test <@ actual.StatusCode = expectedStatusCode @>
        test <@ actual.Severity = expectedSeverity @>
        test <@ actual.Message = expectedMessage @>

    let testMemberDetailReadModel (actual:IMemberDetailReadModel) expectedId expectedUsername expectedEmail
        expectedNickname =

        test <@ actual.Id = expectedId @>
        test <@ actual.Username = expectedUsername @>
        test <@ actual.Email = expectedEmail @>
        test <@ actual.Nickname = expectedNickname @>

    let testMemberListReadModel (actual:IMemberListReadModel) expectedMembers =

        let expectedNumberOfMembers = Seq.length expectedMembers
        test <@ Seq.length actual.Members = expectedNumberOfMembers @>
        for actualMember in actual.Members do
            let findResult = Seq.tryFind (fun (existingMember:IMemberData) ->
                existingMember.Id = actualMember.Id && existingMember.Nickname = actualMember.Nickname) expectedMembers
            test <@ findResult.IsSome @>

    let testMemberListReadModel2 (actual:IMemberListReadModel) expectedMembers =

        let expectedNumberOfMembers = Seq.length expectedMembers
        test <@ Seq.length actual.Members = expectedNumberOfMembers @>
        for actualMember in actual.Members do
            let findResult = Seq.tryFind (fun (existingMember:IMemberListItemReadModel) ->
                existingMember.Id = actualMember.Id && existingMember.Nickname = actualMember.Nickname) expectedMembers
            test <@ findResult.IsSome @>

    let testSelfServiceRegisterCommand (actual:ISelfServiceRegisterCommand) expectedId expectedUsername
        expectedPassword expectedEmail expectedNickname =

        test <@ actual.Id = expectedId @>
        test <@ actual.Username = expectedUsername @>
        test <@ actual.Password = expectedPassword @>
        test <@ actual.Email = expectedEmail @>
        test <@ actual.Nickname = expectedNickname @>

    let testSelfServiceRegisterViewModel (actual:ISelfServiceRegisterViewModel) expectedUsername
        expectedPassword expectedEmail expectedNickname =

        test <@ actual.Username = expectedUsername @>
        test <@ actual.Password = expectedPassword @>
        test <@ actual.Email = expectedEmail @>
        test <@ actual.Nickname = expectedNickname @>

    let testModelState (modelState:ModelStateDictionary) expectedKey expectedErrorMessage =
        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey(expectedKey) @>
        test <@ modelState.[expectedKey].Errors.Count = 1 @>
        test <@ modelState.[expectedKey].Errors.[0].ErrorMessage = expectedErrorMessage @>
