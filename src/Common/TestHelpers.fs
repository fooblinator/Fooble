namespace Fooble.Common

open Fooble.Core
open Fooble.Persistence
open Fooble.Presentation
open Moq
open Moq.FSharp.Extensions
open Swensen.Unquote
open System
open System.Collections.Specialized
open System.Text.RegularExpressions
open System.Web
open System.Web.Mvc
open System.Web.Routing

[<AutoOpen>]
module internal TestHelpers =

    let private makeBindModelParams<'T> routeValues formValues =
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

        (controllerContext, bindingContext)

    let bindMemberChangeEmailViewModel (id:Guid) currentPassword email =
        let routeValues =
            Map.empty
                .Add("Id", string id)

        let formValues =
            Map.empty
                .Add("CurrentPassword", currentPassword)
                .Add("Email", email)

        let (controllerContext, bindingContext) =
            makeBindModelParams<IMemberChangeEmailViewModel> routeValues formValues

        let viewModel =
            MemberChangeEmailViewModelBinder(
                MemberChangeEmailViewModelFactory(MemberChangeEmailViewModel.make))
                .BindModel(controllerContext, bindingContext) :?> IMemberChangeEmailViewModel

        (viewModel, bindingContext.ModelState)

    let bindMemberChangeOtherViewModel (id:Guid) nickname avatarData =
        let routeValues =
            Map.empty
                .Add("Id", string id)

        let formValues =
            Map.empty
                .Add("Nickname", nickname)
                .Add("AvatarData", avatarData)

        let (controllerContext, bindingContext) =
            makeBindModelParams<IMemberChangeOtherViewModel> routeValues formValues

        let viewModel =
            MemberChangeOtherViewModelBinder(
                MemberChangeOtherViewModelFactory(MemberChangeOtherViewModel.make))
                .BindModel(controllerContext, bindingContext) :?> IMemberChangeOtherViewModel

        (viewModel, bindingContext.ModelState)

    let bindMemberChangePasswordViewModel (id:Guid) currentPassword password confirmPassword =
        let routeValues =
            Map.empty
                .Add("Id", string id)

        let formValues =
            Map.empty
                .Add("CurrentPassword", currentPassword)
                .Add("Password", password)
                .Add("ConfirmPassword", confirmPassword)

        let (controllerContext, bindingContext) =
            makeBindModelParams<IMemberChangePasswordViewModel> routeValues formValues

        let viewModel =
            MemberChangePasswordViewModelBinder(
                MemberChangePasswordViewModelFactory(MemberChangePasswordViewModel.make))
                .BindModel(controllerContext, bindingContext) :?> IMemberChangePasswordViewModel

        (viewModel, bindingContext.ModelState)

    let bindMemberChangeUsernameViewModel (id:Guid) currentPassword username =
        let routeValues =
            Map.empty
                .Add("Id", string id)

        let formValues =
            Map.empty
                .Add("CurrentPassword", currentPassword)
                .Add("Username", username)

        let (controllerContext, bindingContext) =
            makeBindModelParams<IMemberChangeUsernameViewModel> routeValues formValues

        let viewModel =
            MemberChangeUsernameViewModelBinder(
                MemberChangeUsernameViewModelFactory(MemberChangeUsernameViewModel.make))
                .BindModel(controllerContext, bindingContext) :?> IMemberChangeUsernameViewModel

        (viewModel, bindingContext.ModelState)

    let bindMemberDeactivateViewModel (id:Guid) currentPassword =
        let routeValues =
            Map.empty
                .Add("Id", string id)

        let formValues =
            Map.empty
                .Add("CurrentPassword", currentPassword)

        let (controllerContext, bindingContext) =
            makeBindModelParams<IMemberDeactivateViewModel> routeValues formValues

        let viewModel =
            MemberDeactivateViewModelBinder(
                MemberDeactivateViewModelFactory(MemberDeactivateViewModel.make))
                .BindModel(controllerContext, bindingContext) :?> IMemberDeactivateViewModel

        (viewModel, bindingContext.ModelState)

    let bindMemberRegisterViewModel username password confirmPassword email nickname avatarData =
        let routeValues = Map.empty

        let formValues =
            Map.empty
                .Add("Username", username)
                .Add("Password", password)
                .Add("ConfirmPassword", confirmPassword)
                .Add("Email", email)
                .Add("Nickname", nickname)
                .Add("AvatarData", avatarData)

        let (controllerContext, bindingContext) =
            makeBindModelParams<IMemberRegisterViewModel> routeValues formValues

        let viewModel =
            MemberRegisterViewModelBinder(
                MemberRegisterViewModelFactory(MemberRegisterViewModel.make))
                .BindModel(controllerContext, bindingContext) :?> IMemberRegisterViewModel

        (viewModel, bindingContext.ModelState)

    let randomEmail len =
        assert (len > 3)
        match len with
        | 3 -> sprintf "%s@%s" (randomString 1) (randomString 1)
        | 4 -> sprintf "%s@%s" (randomString 1) (randomString 2)
        | 5 -> sprintf "%s@%s.%s" (randomString 1) (randomString 1) (randomString 1)
        | _ ->
        let ext = 2 + ((len - 4) % 2)
        let len = (len - 4) / 2
        sprintf "%s@%s.%s" (randomString len) (randomString len) (randomString ext)

    let randomPasswordWith includeDigitChars includeLowercaseChars includeUppercaseChars includeSpecialChars =
        let chars =
            Set.empty
            |> fun xs ->
                match includeDigitChars with
                | true -> Set.union xs (Set.ofList ['0'..'9'])
                | _ -> xs
            |> fun xs ->
                match includeLowercaseChars with
                | true -> Set.union xs (Set.ofList ['a'..'z'])
                | _ -> xs
            |> fun xs ->
                match includeUppercaseChars with
                | true -> Set.union xs (Set.ofList ['A'..'Z'])
                | _ -> xs
            |> fun xs ->
                match includeSpecialChars with
                | true -> Set.union xs (Set.ofArray (specialPasswordCharsPattern.ToCharArray()))
                | _ -> xs
            |> Set.toList
        let random = Random()
        let rec generate len =
            let result = String([| for _ in 0..len - 1 -> chars.[random.Next(chars.Length)] |])
            match includeDigitChars && not (Regex.IsMatch(result, "[0-9]")) with
            | true -> generate len
            | _ ->
            match includeLowercaseChars && not (Regex.IsMatch(result, "[a-z]")) with
            | true -> generate len
            | _ ->
            match includeUppercaseChars && not (Regex.IsMatch(result, "[A-Z]")) with
            | true -> generate len
            | _ ->
            match includeSpecialChars && not (Regex.IsMatch(result, sprintf "[%s]" specialPasswordCharsPattern)) with
            | true -> generate len
            | _ ->
            result
        fun len -> assert (len > 0); generate len

    let randomPassword = randomPasswordWith true true true true

    let makeIdGenerator idGeneratorResult =
        match idGeneratorResult with
        | Some(x) -> IdGenerator(fun () -> x)
        | None -> IdGenerator(fun () -> Guid.NewGuid())

    let testArgumentException paramName message expression =
        assert (not (isNull paramName))
        assert (not (isNull message))
        raisesWith<ArgumentException> expression (fun x ->
            let actualParamName = x.ParamName
            let actualMessage = x.Message.Remove(x.Message.IndexOf("Parameter name: ")).Trim()
            <@ actualParamName = paramName && actualMessage = message @>)

    let testMemberChangeEmailViewModel (actual:IMemberChangeEmailViewModel) id currentPassword email =
        actual.Id =! id
        actual.CurrentPassword =! currentPassword
        actual.Email =! email

    let testMemberChangeOtherViewModel (actual:IMemberChangeOtherViewModel) id nickname avatarData =
        actual.Id =! id
        actual.Nickname =! nickname
        actual.AvatarData =! avatarData

    let testMemberChangePasswordViewModel (actual:IMemberChangePasswordViewModel) id currentPassword password
        confirmPassword =

        actual.Id =! id
        actual.CurrentPassword =! currentPassword
        actual.Password =! password
        actual.ConfirmPassword =! confirmPassword

    let testMemberChangeUsernameViewModel (actual:IMemberChangeUsernameViewModel) id currentPassword username =
        actual.Id =! id
        actual.CurrentPassword =! currentPassword
        actual.Username =! username

    let testMemberDeactivateViewModel (actual:IMemberDeactivateViewModel) id currentPassword =
        actual.Id =! id
        actual.CurrentPassword =! currentPassword

    let testMemberDetailQuery (actual:IMemberDetailQuery) id =
        actual.Id =! id

    let testMemberDetailReadModel (actual:IMemberDetailReadModel) id username email nickname avatarData
        (registeredOn:DateTime) (passwordChangedOn:DateTime) =

        actual.Id =! id
        actual.Username =! username
        actual.Email =! email
        actual.Nickname =! nickname
        actual.AvatarData =! avatarData
        let actualRegisteredOn = actual.Registered
        let actualPasswordChangedOn = actual.PasswordChanged
        actualRegisteredOn.Date =! registeredOn.Date
        actualPasswordChangedOn.Date =! passwordChangedOn.Date

    let testMemberEmailQuery (actual:IMemberEmailQuery) id =
        actual.Id =! id

    let testMemberExistsQuery (actual:IMemberExistsQuery) id =
        actual.Id =! id

    let testMemberListReadModel (actual:IMemberListReadModel) members memberCount =
        assert (not (isNull members))
        Seq.length actual.Members =! Seq.length members
        Seq.length actual.Members =! memberCount
        actual.MemberCount =! Seq.length members
        actual.MemberCount =! memberCount
        for actualMember in actual.Members do
            let findResult =
                Seq.tryFind
                    (fun (id, nickname, avatarData) ->
                         actualMember.Id = id &&
                         actualMember.Nickname = nickname &&
                         actualMember.AvatarData = avatarData) members
            findResult.IsSome =! true

    let testMemberOtherQuery (actual:IMemberOtherQuery) id =
        actual.Id =! id

    let testMemberRegisterViewModel (actual:IMemberRegisterViewModel) username password confirmPassword email nickname
        avatarData =

        actual.Username =! username
        actual.Password =! password
        actual.ConfirmPassword =! confirmPassword
        actual.Email =! email
        actual.Nickname =! nickname
        match avatarData with
        | Some(x) -> actual.AvatarData =! x
        | None -> ()

    let testMemberUsernameQuery (actual:IMemberUsernameQuery) id =
        actual.Id =! id

    let testMessageDisplayReadModel (actual:IMessageDisplayReadModel) heading subHeading statusCode severity message =
        actual.Heading =! heading
        actual.SubHeading =! subHeading
        actual.StatusCode =! statusCode
        actual.Severity =! severity
        actual.Message =! message

    let testModelState (modelState:ModelStateDictionary) errorKey errorMessage =
        assert (not (isNull modelState))
        modelState.IsValid =! false
        modelState.ContainsKey(errorKey) =! true
        modelState.[errorKey].Errors.Count =! 1
        modelState.[errorKey].Errors.[0].ErrorMessage =! errorMessage
