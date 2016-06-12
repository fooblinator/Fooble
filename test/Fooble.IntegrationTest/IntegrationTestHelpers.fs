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
open System.Web.Routing

type internal Settings = AppSettings<"App.config">

[<AutoOpen>]
module internal IntegrationTestHelpers =

    let private bindModel<'T> routeValues formValues =
        let routeData =
            (RouteData(), routeValues)
            ||> fun x y -> Map.iter (fun k v -> x.Values.Add(k, v)) y; x

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
        controllerContext.RouteData <- routeData

        FoobleModelBinder().BindModel(controllerContext, bindingContext) :?> 'T

    let bindMemberChangeEmailViewModel id currentPassword newEmail =
        let routeValues =
            Map.empty
                .Add("Id", String.ofGuid id)

        let formValues =
            Map.empty
                .Add("CurrentPassword", currentPassword)
                .Add("NewEmail", newEmail)

        bindModel<IMemberChangeEmailViewModel> routeValues formValues

    let bindMemberChangeOtherViewModel id newNickname =
        let routeValues =
            Map.empty
                .Add("Id", String.ofGuid id)

        let formValues =
            Map.empty
                .Add("NewNickname", newNickname)

        bindModel<IMemberChangeOtherViewModel> routeValues formValues

    let bindMemberChangePasswordViewModel id currentPassword newPassword confirmPassword =
        let routeValues =
            Map.empty
                .Add("Id", String.ofGuid id)

        let formValues =
            Map.empty
                .Add("CurrentPassword", currentPassword)
                .Add("NewPassword", newPassword)
                .Add("ConfirmPassword", confirmPassword)

        bindModel<IMemberChangePasswordViewModel> routeValues formValues

    let bindMemberChangeUsernameViewModel id currentPassword newUsername =
        let routeValues =
            Map.empty
                .Add("Id", String.ofGuid id)

        let formValues =
            Map.empty
                .Add("CurrentPassword", currentPassword)
                .Add("NewUsername", newUsername)

        bindModel<IMemberChangeUsernameViewModel> routeValues formValues

    let bindMemberDeactivateViewModel id currentPassword =
        let routeValues =
            Map.empty
                .Add("Id", String.ofGuid id)

        let formValues =
            Map.empty
                .Add("CurrentPassword", currentPassword)

        bindModel<IMemberDeactivateViewModel> routeValues formValues

    let bindMemberRegisterViewModel username password confirmPassword email nickname =
        let routeValues = Map.empty

        let formValues =
            Map.empty
                .Add("Username", username)
                .Add("Password", password)
                .Add("ConfirmPassword", confirmPassword)
                .Add("Email", email)
                .Add("Nickname", nickname)

        bindModel<IMemberRegisterViewModel> routeValues formValues

    let makeTestKeyGenerator key =
        match key with
        | Some(x) -> KeyGenerator(fun () -> x)
        | None -> KeyGenerator(fun () -> Guid.random ())

    let makeTestMemberData id username passwordData email nickname registered passwordChanged isDeactivated =
#if DEBUG
        assertWith (validateMemberId id)
        assertWith (validateMemberUsername username)
        assertWith (validateMemberEmail email)
        assertWith (validateMemberNickname nickname)
#endif

        let mutable id = id
        let mutable username = username
        let mutable passwordData = passwordData
        let mutable email = email
        let mutable nickname = nickname
        let mutable registered = registered
        let mutable passwordChanged = passwordChanged
        let mutable isDeactivated = isDeactivated

        { new IMemberData with

              member __.Id
                  with get () = id
                  and set (x) = id <- x

              member __.Username
                  with get () = username
                  and set (x) = username <- x

              member __.PasswordData
                  with get () = passwordData
                  and set (x) = passwordData <- x

              member __.Email
                  with get () = email
                  and set (x) = email <- x

              member __.Nickname
                  with get () = nickname
                  and set (x) = nickname <- x

              member __.Registered
                  with get() = registered
                  and set (x) = registered <- x

              member __.PasswordChanged
                  with get() = passwordChanged
                  and set (x) = passwordChanged <- x

              member __.IsDeactivated
                  with get() = isDeactivated
                  and set (x) = isDeactivated <- x }

    let makeTestMemberData2 id username passwordData email nickname =
        makeTestMemberData id username passwordData email nickname DateTime.UtcNow DateTime.UtcNow

    let testMemberChangeEmailViewModel (actual:IMemberChangeEmailViewModel) expectedId expectedCurrentPassword
        expectedNewEmail =

        actual.Id =! expectedId
        actual.CurrentPassword =! expectedCurrentPassword
        actual.NewEmail =! expectedNewEmail

    let testMemberChangeOtherViewModel (actual:IMemberChangeOtherViewModel) expectedId expectedNewNickname =
        actual.Id =! expectedId
        actual.NewNickname =! expectedNewNickname

    let testMemberChangePasswordViewModel (actual:IMemberChangePasswordViewModel) expectedId expectedCurrentPassword
        expectedNewPassword expectedConfirmPassword =

        actual.Id =! expectedId
        actual.CurrentPassword =! expectedCurrentPassword
        actual.NewPassword =! expectedNewPassword
        actual.ConfirmPassword =! expectedConfirmPassword

    let testMemberChangeUsernameViewModel (actual:IMemberChangeUsernameViewModel) expectedId expectedCurrentPassword
        expectedNewUsername =

        actual.Id =! expectedId
        actual.CurrentPassword =! expectedCurrentPassword
        actual.NewUsername =! expectedNewUsername

    let testMemberDeactivateViewModel (actual:IMemberDeactivateViewModel) expectedId expectedCurrentPassword =

        actual.Id =! expectedId
        actual.CurrentPassword =! expectedCurrentPassword

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

    let testMemberListReadModel (actual:IMemberListReadModel) expectedMembers expectedMemberCount =

        Seq.length actual.Members =! Seq.length expectedMembers
        Seq.length actual.Members =! expectedMemberCount
        actual.MemberCount =! Seq.length expectedMembers
        actual.MemberCount =! expectedMemberCount
        for actualMember in actual.Members do
            let findResult = Seq.tryFind (fun (existingMember:IMemberData) ->
                existingMember.Id = actualMember.Id && existingMember.Nickname = actualMember.Nickname) expectedMembers
            findResult.IsSome =! true

    let testMemberRegisterViewModel (actual:IMemberRegisterViewModel) expectedUsername expectedPassword
        expectedConfirmPassword expectedEmail expectedNickname =

        actual.Username =! expectedUsername
        actual.Password =! expectedPassword
        actual.ConfirmPassword =! expectedConfirmPassword
        actual.Email =! expectedEmail
        actual.Nickname =! expectedNickname

    let testMessageDisplayReadModel (actual:IMessageDisplayReadModel) expectedHeading expectedSubHeading
        expectedStatusCode expectedSeverity expectedMessage =

        actual.Heading =! expectedHeading
        actual.SubHeading =! expectedSubHeading
        actual.StatusCode =! expectedStatusCode
        actual.Severity =! expectedSeverity
        actual.Message =! expectedMessage

    let testModelState (modelState:ModelStateDictionary) expectedKey expectedErrorMessage =

        modelState.IsValid =! false
        modelState.ContainsKey(expectedKey) =! true
        modelState.[expectedKey].Errors.Count =! 1
        modelState.[expectedKey].Errors.[0].ErrorMessage =! expectedErrorMessage
