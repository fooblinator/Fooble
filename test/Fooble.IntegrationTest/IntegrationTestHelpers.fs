namespace Fooble.IntegrationTest

open Fooble.Common
open Fooble.Core
open FSharp.Configuration
open Fooble.Persistence
open Fooble.Presentation
open Moq
open Moq.FSharp.Extensions
open Swensen.Unquote
open System.Collections.Specialized
open System.Diagnostics
open System.Web
open System.Web.Mvc

type internal Settings = AppSettings<"App.config">

[<DebuggerStepThrough>]
[<AutoOpen>]
module internal IntegrationTestHelpers =

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

        binder.BindModel(controllerContext, bindingContext) :?> 'T

    let bindSelfServiceRegisterViewModel username password confirmPassword email nickname =
        Map.empty
            .Add("Username", username)
            .Add("Password", password)
            .Add("ConfirmPassword", confirmPassword)
            .Add("Email", email)
            .Add("Nickname", nickname)
        |> bindModel<ISelfServiceRegisterViewModel>

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
