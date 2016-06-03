namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open Fooble.UnitTest
open Fooble.Web.Controllers
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module SelfServiceControllerTests =

    [<Test>]
    let ``Constructing, with null mediator, raises expected exception`` () =
        let expectedParamName = "mediator"
        let expectedMessage = "Mediator is required"

        let keyGenerator = makeTestKeyGenerator None
        raisesWith<ArgumentException> <@ new SelfServiceController(null, keyGenerator) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

//    [<Test>]
//    let ``Constructing, with null key generator, raises expected exception`` () =
//        let expectedParamName = "keyGenerator"
//        let expectedMessage = "Key generator is required"
//
//        raisesWith<ArgumentException> <@ new SelfServiceController(mock (), null) @>
//            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let keyGenerator = makeTestKeyGenerator None
        ignore <| new SelfServiceController(mock (), keyGenerator)

    [<Test>]
    let ``Calling register, returns expected result`` () =
        let emptyUsername = String.empty
        let emptyPassword = String.empty
        let emptyEmail = String.empty
        let emptyNickname = String.empty

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = emptyUsername @>
        test <@ actualViewModel.Password = emptyPassword @>
        test <@ actualViewModel.Email = emptyEmail @>
        test <@ actualViewModel.Nickname = emptyNickname @>

    [<Test>]
    let ``Calling register post, with null username, returns expected result`` () =
        let nullUsername:string = null
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is required")

        let (viewModel, _) =
            Map.empty
                .Add("Username", nullUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = nullUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("username") @>
        test <@ actualModelState.["username"].Errors.Count = 1 @>
        test <@ actualModelState.["username"].Errors.[0].ErrorMessage = "Username is required" @>

    [<Test>]
    let ``Calling register post, with empty username, returns expected result`` () =
        let emptyUsername = String.empty
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is required")

        let (viewModel, _) =
            Map.empty
                .Add("Username", emptyUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = emptyUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("username") @>
        test <@ actualModelState.["username"].Errors.Count = 1 @>
        test <@ actualModelState.["username"].Errors.[0].ErrorMessage = "Username is required" @>

    [<Test>]
    let ``Calling register post, with username shorter than 3 characters, returns expected result`` () =
        let shortUsername = String.random 2
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is shorter than 3 characters")

        let (viewModel, _) =
            Map.empty
                .Add("Username", shortUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = shortUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("username") @>
        test <@ actualModelState.["username"].Errors.Count = 1 @>
        test <@ actualModelState.["username"].Errors.[0].ErrorMessage = "Username is shorter than 3 characters" @>

    [<Test>]
    let ``Calling register post, with username longer than 32 characters, returns expected result`` () =
        let longUsername = String.random 33
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is longer than 32 characters")

        let (viewModel, _) =
            Map.empty
                .Add("Username", longUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = longUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("username") @>
        test <@ actualModelState.["username"].Errors.Count = 1 @>
        test <@ actualModelState.["username"].Errors.[0].ErrorMessage = "Username is longer than 32 characters" @>

    [<Test>]
    let ``Calling register post, with username in invalid format, returns expected result`` () =
        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username",
            "Username is not in the correct format (lowercase alphanumeric)")

        let (viewModel, _) =
            Map.empty
                .Add("Username", invalidFormatUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = invalidFormatUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("username") @>
        test <@ actualModelState.["username"].Errors.Count = 1 @>
        test <@ actualModelState.["username"].Errors.[0].ErrorMessage =
            "Username is not in the correct format (lowercase alphanumeric)" @>

    [<Test>]
    let ``Calling register post, with null password, returns expected result`` () =
        let expectedUsername = String.random 32
        let nullPassword:string = null
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is required")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", nullPassword)
                .Add("ConfirmPassword", nullPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = String.empty @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("password") @>
        test <@ actualModelState.["password"].Errors.Count = 1 @>
        test <@ actualModelState.["password"].Errors.[0].ErrorMessage = "Password is required" @>

    [<Test>]
    let ``Calling register post, with empty password, returns expected result`` () =
        let expectedUsername = String.random 32
        let emptyPassword = String.empty
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is required")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", emptyPassword)
                .Add("ConfirmPassword", emptyPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = String.empty @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("password") @>
        test <@ actualModelState.["password"].Errors.Count = 1 @>
        test <@ actualModelState.["password"].Errors.[0].ErrorMessage = "Password is required" @>

    [<Test>]
    let ``Calling register post, with password shorter than 8 characters, returns expected result`` () =
        let expectedUsername = String.random 32
        let shortPassword = Password.random 7
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is shorter than 8 characters")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", shortPassword)
                .Add("ConfirmPassword", shortPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = String.empty @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("password") @>
        test <@ actualModelState.["password"].Errors.Count = 1 @>
        test <@ actualModelState.["password"].Errors.[0].ErrorMessage = "Password is shorter than 8 characters" @>

    [<Test>]
    let ``Calling register post, with password longer than 32 characters, returns expected result`` () =
        let expectedUsername = String.random 32
        let longPassword = Password.random 33
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is longer than 32 characters")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", longPassword)
                .Add("ConfirmPassword", longPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = String.empty @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("password") @>
        test <@ actualModelState.["password"].Errors.Count = 1 @>
        test <@ actualModelState.["password"].Errors.[0].ErrorMessage = "Password is longer than 32 characters" @>

    [<Test>]
    let ``Calling register post, with password without digits, returns expected result`` () =
        let expectedUsername = String.random 32
        let noDigitsPassword = makeBadPasswordWithoutDigits 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password does not contain any numbers")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", noDigitsPassword)
                .Add("ConfirmPassword", noDigitsPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = String.empty @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("password") @>
        test <@ actualModelState.["password"].Errors.Count = 1 @>
        test <@ actualModelState.["password"].Errors.[0].ErrorMessage = "Password does not contain any numbers" @>

    [<Test>]
    let ``Calling register post, with password without lower alphas, returns expected result`` () =
        let expectedUsername = String.random 32
        let noLowerAlphasPassword = makeBadPasswordWithoutLowerAlphas 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password does not contain any lower-case letters")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", noLowerAlphasPassword)
                .Add("ConfirmPassword", noLowerAlphasPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = String.empty @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("password") @>
        test <@ actualModelState.["password"].Errors.Count = 1 @>
        test <@ actualModelState.["password"].Errors.[0].ErrorMessage = "Password does not contain any lower-case letters" @>

    [<Test>]
    let ``Calling register post, with password without upper alphas, returns expected result`` () =
        let expectedUsername = String.random 32
        let noUpperAlphasPassword = makeBadPasswordWithoutUpperAlphas 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password does not contain any upper-case letters")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", noUpperAlphasPassword)
                .Add("ConfirmPassword", noUpperAlphasPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = String.empty @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("password") @>
        test <@ actualModelState.["password"].Errors.Count = 1 @>
        test <@ actualModelState.["password"].Errors.[0].ErrorMessage = "Password does not contain any upper-case letters" @>

    [<Test>]
    let ``Calling register post, with password without special chars, returns expected result`` () =
        let expectedUsername = String.random 32
        let noSpecialCharsPassword = makeBadPasswordWithoutSpecialChars 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password  does not contain any special characters")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", noSpecialCharsPassword)
                .Add("ConfirmPassword", noSpecialCharsPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = String.empty @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("password") @>
        test <@ actualModelState.["password"].Errors.Count = 1 @>
        test <@ actualModelState.["password"].Errors.[0].ErrorMessage =
            "Password  does not contain any special characters" @>

    [<Test>]
    let ``Calling register post, with password without invalid chars, returns expected result`` () =
        let expectedUsername = String.random 32
        let invalidCharsPassword = makeBadPasswordWithInvalidChars 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password contains invalid characters")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", invalidCharsPassword)
                .Add("ConfirmPassword", invalidCharsPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = String.empty @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("password") @>
        test <@ actualModelState.["password"].Errors.Count = 1 @>
        test <@ actualModelState.["password"].Errors.[0].ErrorMessage = "Password contains invalid characters" @>

    [<Test>]
    let ``Calling register post, with non-matching passwords, returns expected result`` () =
        let expectedUsername = String.random 32
        let nonMatchingConfirmPassword = Password.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("confirmPassword", "Passwords do not match")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", Password.random 32)
                .Add("ConfirmPassword", nonMatchingConfirmPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = String.empty @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("confirmPassword") @>
        test <@ actualModelState.["confirmPassword"].Errors.Count = 1 @>
        test <@ actualModelState.["confirmPassword"].Errors.[0].ErrorMessage = "Passwords do not match" @>

    [<Test>]
    let ``Calling register post, with null email, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let nullEmail:string = null
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is required")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", nullEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = nullEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("email") @>
        test <@ actualModelState.["email"].Errors.Count = 1 @>
        test <@ actualModelState.["email"].Errors.[0].ErrorMessage = "Email is required" @>

    [<Test>]
    let ``Calling register post, with empty email, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let emptyEmail = String.empty
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is required")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", emptyEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = emptyEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("email") @>
        test <@ actualModelState.["email"].Errors.Count = 1 @>
        test <@ actualModelState.["email"].Errors.[0].ErrorMessage = "Email is required" @>

    [<Test>]
    let ``Calling register post, with email longer than 254 characters, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let longEmail = String.random 255
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is longer than 254 characters")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", longEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = longEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("email") @>
        test <@ actualModelState.["email"].Errors.Count = 1 @>
        test <@ actualModelState.["email"].Errors.[0].ErrorMessage = "Email is longer than 254 characters" @>

    [<Test>]
    let ``Calling register post, with email in invalid format, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let invalidFormatEmail = String.random 64
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is not in the correct format")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", invalidFormatEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = invalidFormatEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("email") @>
        test <@ actualModelState.["email"].Errors.Count = 1 @>
        test <@ actualModelState.["email"].Errors.[0].ErrorMessage = "Email is not in the correct format" @>

    [<Test>]
    let ``Calling register post, with null nickname, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random ()
        let nullNickname:string = null

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is required")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", nullNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = nullNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("nickname") @>
        test <@ actualModelState.["nickname"].Errors.Count = 1 @>
        test <@ actualModelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is required" @>

    [<Test>]
    let ``Calling register post, with empty nickname, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random ()
        let emptyNickname = String.empty

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is required")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", emptyNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = emptyNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("nickname") @>
        test <@ actualModelState.["nickname"].Errors.Count = 1 @>
        test <@ actualModelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is required" @>

    [<Test>]
    let ``Calling register post, with nickname longer than 64 characters, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random ()
        let longNickname = String.random 65

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is longer than 64 characters")

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", longNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = longNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("nickname") @>
        test <@ actualModelState.["nickname"].Errors.Count = 1 @>
        test <@ actualModelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is longer than 64 characters" @>

    [<Test>]
    let ``Calling register post, with existing username in data store, returns expected result`` () =
        let existingUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mediatorMock.Object, keyGenerator)

        let (viewModel, _) =
            Map.empty
                .Add("Username", existingUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        mediatorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = existingUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("username") @>
        test <@ actualModelState.["username"].Errors.Count = 1 @>
        test <@ actualModelState.["username"].Errors.[0].ErrorMessage = "Username is unavailable" @>

    [<Test>]
    let ``Calling register post, with existing email in data store, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let existingEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mediatorMock.Object, keyGenerator)

        let (viewModel, _) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Password", expectedPassword)
                .Add("ConfirmPassword", expectedPassword)
                .Add("Email", existingEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        mediatorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Password = expectedPassword @>
        test <@ actualViewModel.Email = existingEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let actualModelState = viewResult.ViewData.ModelState
        test <@ not (actualModelState.IsValid) @>
        test <@ actualModelState.ContainsKey("email") @>
        test <@ actualModelState.["email"].Errors.Count = 1 @>
        test <@ actualModelState.["email"].Errors.[0].ErrorMessage = "Email is already registered" @>

    [<Test>]
    let ``Calling register post, with no existing username or email in data store, returns expected result`` () =
        let expectedId = Guid.random ()

        let commandResult = SelfServiceRegisterCommand.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator (Some expectedId)
        let controller = new SelfServiceController(mediatorMock.Object, keyGenerator)

        let password = Password.random 32
        let (viewModel, _) =
            Map.empty
                .Add("Username", String.random 32)
                .Add("Password", password)
                .Add("ConfirmPassword", password)
                .Add("Email", EmailAddress.random ())
                .Add("Nickname", Password.random 32)
            |> bindModel<ISelfServiceRegisterViewModel>
        let result = controller.Register viewModel

        mediatorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? RedirectToRouteResult @>

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        test <@ routeValues.ContainsKey("controller") @>
        test <@ routeValues.["controller"].ToString().ToLowerInvariant() = "member" @>

        test <@ routeValues.ContainsKey("action") @>
        test <@ routeValues.["action"].ToString().ToLowerInvariant() = "detail" @>

        let expectedIdString = String.ofGuid expectedId

        test <@ routeValues.ContainsKey("id") @>
        test <@ routeValues.["id"].ToString().ToLowerInvariant() = expectedIdString @>
