namespace Fooble.IntegrationTest

open Fooble.Common
open Fooble.Core
open FSharp.Configuration
open Fooble.Persistence
open Fooble.Presentation
open Moq
open Moq.FSharp.Extensions
open Swensen.Unquote
open System
open System.Collections.Specialized
open System.Web
open System.Web.Mvc

type internal Settings = AppSettings<"App.config">

[<AutoOpen>]
module internal IntegrationTestHelpers =

    let private bindModel<'T> formValues =
        let formValues =
            (NameValueCollection(), formValues)
            ||> fun x y -> Map.iter (fun k v -> x.Add(k, v)) y; x

        let bindingContext =
            ModelBindingContext(ModelName = typeof<'T>.Name,
                ValueProvider = NameValueCollectionValueProvider(formValues, null),
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof<'T>))

        let httpRequestMock = Mock<HttpRequestBase>()
        httpRequestMock.SetupFunc(fun x -> x.Form).Returns(formValues).End

        let httpContextMock = Mock<HttpContextBase>()
        httpContextMock.SetupFunc(fun x -> x.Request).Returns(httpRequestMock.Object).End

        let controllerContext = ControllerContext()
        controllerContext.HttpContext <- httpContextMock.Object

        FoobleModelBinder().BindModel(controllerContext, bindingContext) :?> 'T

    let bindMemberChangePasswordViewModel currentPassword newPassword confirmPassword =
        Map.empty
            .Add("CurrentPassword", currentPassword)
            .Add("NewPassword", newPassword)
            .Add("ConfirmPassword", confirmPassword)
        |> bindModel<IMemberChangePasswordViewModel>

    let bindMemberRegisterViewModel username password confirmPassword email nickname =
        Map.empty
            .Add("Username", username)
            .Add("Password", password)
            .Add("ConfirmPassword", confirmPassword)
            .Add("Email", email)
            .Add("Nickname", nickname)
        |> bindModel<IMemberRegisterViewModel>

    let makeTestKeyGenerator key =
        match key with
        | Some x -> KeyGenerator(fun () -> x)
        | None -> KeyGenerator(fun () -> Guid.random ())

    let makeTestMemberData id username passwordData email nickname registered passwordChanged =
        assertMemberId id
        assertMemberUsername username
        assertMemberPasswordData passwordData
        assertMemberEmail email
        assertMemberNickname nickname

        let idRef = ref id
        let usernameRef = ref username
        let passwordDataRef = ref passwordData
        let emailRef = ref email
        let nicknameRef = ref nickname
        let registeredRef = ref registered
        let passwordChangedRef = ref passwordChanged

        { new IMemberData with

              member this.Id
                  with get () = !idRef
                  and set (x) = idRef := x

              member this.Username
                  with get () = !usernameRef
                  and set (v) = usernameRef := v

              member this.PasswordData
                  with get () = !passwordDataRef
                  and set (x) = passwordDataRef := x

              member this.Email
                  with get () = !emailRef
                  and set (x) = emailRef := x

              member this.Nickname
                  with get () = !nicknameRef
                  and set (x) = nicknameRef := x

              member this.Registered
                  with get() = !registeredRef
                  and set(x) = registeredRef := x

              member this.PasswordChanged
                  with get() = !passwordChangedRef
                  and set(x) = passwordChangedRef := x }

    let makeTestMemberData2 id username passwordData email nickname =
        makeTestMemberData id username passwordData email nickname DateTime.UtcNow DateTime.UtcNow

    let testMessageDisplayReadModel (actual:IMessageDisplayReadModel) expectedHeading expectedSubHeading
        expectedStatusCode expectedSeverity expectedMessage =

        actual.Heading =! expectedHeading
        actual.SubHeading =! expectedSubHeading
        actual.StatusCode =! expectedStatusCode
        actual.Severity =! expectedSeverity
        actual.Message =! expectedMessage

    let testMemberDetailReadModel (actual:IMemberDetailReadModel) expectedId expectedUsername expectedEmail
        expectedNickname (expectedRegistered:DateTime) (expectedPasswordChanged:DateTime) =

        actual.Id =! expectedId
        actual.Username =! expectedUsername
        actual.Email =! expectedEmail
        actual.Nickname =! expectedNickname

        let actualRegistered = actual.Registered
        let actualPasswordChanged = actual.PasswordChanged

        actualRegistered.Date =! expectedRegistered.Date
        actualPasswordChanged.Date =! expectedPasswordChanged.Date

    let testMemberListReadModel (actual:IMemberListReadModel) expectedMembers =

        Seq.length actual.Members =! Seq.length expectedMembers
        for actualMember in actual.Members do
            let findResult = Seq.tryFind (fun (existingMember:IMemberData) ->
                existingMember.Id = actualMember.Id && existingMember.Nickname = actualMember.Nickname) expectedMembers
            findResult.IsSome =! true

    let testMemberChangePasswordViewModel (actual:IMemberChangePasswordViewModel) expectedCurrentPassword
        expectedNewPassword expectedConfirmPassword =

        actual.CurrentPassword =! expectedCurrentPassword
        actual.NewPassword =! expectedNewPassword
        actual.ConfirmPassword =! expectedConfirmPassword

    let testMemberRegisterViewModel (actual:IMemberRegisterViewModel) expectedUsername expectedPassword
        expectedConfirmPassword expectedEmail expectedNickname =

        actual.Username =! expectedUsername
        actual.Password =! expectedPassword
        actual.ConfirmPassword =! expectedConfirmPassword
        actual.Email =! expectedEmail
        actual.Nickname =! expectedNickname

    let testModelState (modelState:ModelStateDictionary) expectedKey expectedErrorMessage =

        modelState.IsValid =! false
        modelState.ContainsKey(expectedKey) =! true
        modelState.[expectedKey].Errors.Count =! 1
        modelState.[expectedKey].Errors.[0].ErrorMessage =! expectedErrorMessage
