﻿namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
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

[<AutoOpen>]
module internal UnitTestHelpers =

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

        (FoobleModelBinder().BindModel(controllerContext, bindingContext) :?> 'T, bindingContext.ModelState)

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

    let bindMemberChangePasswordViewModel2 id currentPassword newPassword confirmPassword =
        fst (bindMemberChangePasswordViewModel id currentPassword newPassword confirmPassword)

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

    let bindMemberRegisterViewModel2 username password confirmPassword email nickname =
        fst (bindMemberRegisterViewModel username password confirmPassword email nickname)

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
        fun len ->
            assert (len > 0)
            generate len

    let makeBadPasswordWithoutLowerAlphas =
        let charset =
            Set.ofList [ 'a' .. 'z' ]
            |> Set.difference Password.charset
        let rec generate len =
            let res = makeBadPasswordWith charset len
            match (Password.hasDigits res, Password.hasUpperAlphas res, Password.hasSpecialChars res) with
            | (true, true, true) -> res
            | _ -> generate len
        fun len ->
            assert (len > 0)
            generate len

    let makeBadPasswordWithoutUpperAlphas =
        let charset =
            Set.ofList [ 'A' .. 'Z' ]
            |> Set.difference Password.charset
        let rec generate len =
            let res = makeBadPasswordWith charset len
            match (Password.hasDigits res, Password.hasLowerAlphas res, Password.hasSpecialChars res) with
            | (true, true, true) -> res
            | _ -> generate len
        fun len ->
            assert (len > 0)
            generate len

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
        fun len ->
            assert (len > 0)
            generate len

    let makeBadPasswordWithInvalidChars len =
        Password.random (len - 2) |> sprintf "%c%s" Char.MinValue

    let makeTestKeyGenerator key =
        match key with
        | Some(x) -> KeyGenerator(fun () -> x)
        | None -> KeyGenerator(fun () -> Guid.random ())

    let makeTestMemberData id username passwordData email nickname registered passwordChanged =
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
                  and set (x) = passwordChanged <- x }

    let makeTestMemberData2 id username passwordData email nickname =
        makeTestMemberData id username passwordData email nickname DateTime.UtcNow DateTime.UtcNow

    let makeTestMemberDataFactory () =
        MemberDataFactory(makeTestMemberData2)

    let makeTestMemberDetailReadModel id username email nickname registered passwordChanged =
#if DEBUG
        assertWith (validateMemberId id)
        assertWith (validateMemberUsername username)
        assertWith (validateMemberEmail email)
        assertWith (validateMemberNickname nickname)
#endif

        { new IMemberDetailReadModel with
              member __.Id with get() = id
              member __.Username with get() = username
              member __.Email with get() = email
              member __.Nickname with get() = nickname
              member __.Registered with get() = registered
              member __.PasswordChanged with get() = passwordChanged }

    let makeTestMemberDetailReadModelFactory () =
        MemberDetailReadModelFactory(makeTestMemberDetailReadModel)

    let makeTestMemberListItemReadModel id nickname =
#if DEBUG
        assertWith (validateMemberId id)
        assertWith (validateMemberNickname nickname)
#endif

        { new IMemberListItemReadModel with
              member __.Id with get() = id
              member __.Nickname with get() = nickname }

    let makeTestMemberListItemReadModelFactory () =
        MemberListItemReadModelFactory(makeTestMemberListItemReadModel)

    let makeTestMemberListReadModel members =
#if DEBUG
        assertOn members "members" [ (Seq.isNotNullOrEmpty), "Members is required" ]
#endif

        { new IMemberListReadModel with
              member __.Members with get() = members }

    let makeTestMemberListReadModelFactory () =
        MemberListReadModelFactory(makeTestMemberListReadModel)

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

    let testMemberListReadModel2 (actual:IMemberListReadModel) expectedMembers =

        Seq.length actual.Members =! Seq.length expectedMembers
        for actualMember in actual.Members do
            let findResult = Seq.tryFind (fun (existingMember:IMemberListItemReadModel) ->
                existingMember.Id = actualMember.Id && existingMember.Nickname = actualMember.Nickname) expectedMembers
            findResult.IsSome =! true

    let testMemberChangePasswordCommand (actual:IMemberChangePasswordCommand) expectedId
        expectedCurrentPassword expectedNewPassword =

        actual.Id =! expectedId
        actual.CurrentPassword =! expectedCurrentPassword
        actual.NewPassword =! expectedNewPassword

    let testMemberChangePasswordViewModel (actual:IMemberChangePasswordViewModel) expectedId expectedCurrentPassword
        expectedNewPassword expectedConfirmPassword =

        actual.Id =! expectedId
        actual.CurrentPassword =! expectedCurrentPassword
        actual.NewPassword =! expectedNewPassword
        actual.ConfirmPassword =! expectedConfirmPassword

    let testMemberRegisterCommand (actual:IMemberRegisterCommand) expectedId expectedUsername
        expectedPassword expectedEmail expectedNickname =

        actual.Id =! expectedId
        actual.Username =! expectedUsername
        actual.Password =! expectedPassword
        actual.Email =! expectedEmail
        actual.Nickname =! expectedNickname

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
