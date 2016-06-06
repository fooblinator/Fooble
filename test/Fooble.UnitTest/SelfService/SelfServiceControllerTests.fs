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

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let keyGenerator = makeTestKeyGenerator None
        ignore (new SelfServiceController(mock (), keyGenerator))

    [<Test>]
    let ``Calling register, returns expected result`` () =
        let emptyUsername = String.empty
        let emptyPassword = String.empty
        let emptyConfirmPassword = String.empty
        let emptyEmail = String.empty
        let emptyNickname = String.empty

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel emptyUsername emptyPassword emptyConfirmPassword emptyEmail
            emptyNickname

    [<Test>]
    let ``Calling register post, with null username, returns expected result`` () =
        let nullUsername:string = null
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is required")

        let viewModel =
            bindSelfServiceRegisterViewModel2 nullUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel nullUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is required"

    [<Test>]
    let ``Calling register post, with empty username, returns expected result`` () =
        let emptyUsername = String.empty
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is required")

        let viewModel =
            bindSelfServiceRegisterViewModel2 emptyUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel emptyUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is required"

    [<Test>]
    let ``Calling register post, with username shorter than 3 characters, returns expected result`` () =
        let shortUsername = String.random 2
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is shorter than 3 characters")

        let viewModel =
            bindSelfServiceRegisterViewModel2 shortUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel shortUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is shorter than 3 characters"

    [<Test>]
    let ``Calling register post, with username longer than 32 characters, returns expected result`` () =
        let longUsername = String.random 33
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is longer than 32 characters")

        let viewModel =
            bindSelfServiceRegisterViewModel2 longUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel longUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is longer than 32 characters"

    [<Test>]
    let ``Calling register post, with username in invalid format, returns expected result`` () =
        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username",
            "Username is not in the correct format (lowercase alphanumeric)")

        let viewModel =
            bindSelfServiceRegisterViewModel2 invalidFormatUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel invalidFormatUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is not in the correct format (lowercase alphanumeric)"

    [<Test>]
    let ``Calling register post, with null password, returns expected result`` () =
        let expectedUsername = String.random 32
        let nullPassword:string = null
        let expectedConfirmPassword = nullPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is required")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername nullPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password is required"

    [<Test>]
    let ``Calling register post, with empty password, returns expected result`` () =
        let expectedUsername = String.random 32
        let emptyPassword = String.empty
        let expectedConfirmPassword = emptyPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is required")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername emptyPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password is required"

    [<Test>]
    let ``Calling register post, with password shorter than 8 characters, returns expected result`` () =
        let expectedUsername = String.random 32
        let shortPassword = Password.random 7
        let expectedConfirmPassword = shortPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is shorter than 8 characters")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername shortPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password is shorter than 8 characters"

    [<Test>]
    let ``Calling register post, with password longer than 32 characters, returns expected result`` () =
        let expectedUsername = String.random 32
        let longPassword = Password.random 33
        let expectedConfirmPassword = longPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is longer than 32 characters")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername longPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password is longer than 32 characters"

    [<Test>]
    let ``Calling register post, with password without digits, returns expected result`` () =
        let expectedUsername = String.random 32
        let noDigitsPassword = makeBadPasswordWithoutDigits 32
        let expectedConfirmPassword = noDigitsPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password does not contain any numbers")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername noDigitsPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password does not contain any numbers"

    [<Test>]
    let ``Calling register post, with password without lower alphas, returns expected result`` () =
        let expectedUsername = String.random 32
        let noLowerAlphasPassword = makeBadPasswordWithoutLowerAlphas 32
        let expectedConfirmPassword = noLowerAlphasPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password does not contain any lower-case letters")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername noLowerAlphasPassword expectedConfirmPassword
                expectedEmail expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password does not contain any lower-case letters"

    [<Test>]
    let ``Calling register post, with password without upper alphas, returns expected result`` () =
        let expectedUsername = String.random 32
        let noUpperAlphasPassword = makeBadPasswordWithoutUpperAlphas 32
        let expectedConfirmPassword = noUpperAlphasPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password does not contain any upper-case letters")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername noUpperAlphasPassword expectedConfirmPassword
                expectedEmail expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password does not contain any upper-case letters"

    [<Test>]
    let ``Calling register post, with password without special chars, returns expected result`` () =
        let expectedUsername = String.random 32
        let noSpecialCharsPassword = makeBadPasswordWithoutSpecialChars 32
        let expectedConfirmPassword = noSpecialCharsPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password  does not contain any special characters")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername noSpecialCharsPassword expectedConfirmPassword
                expectedEmail expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password  does not contain any special characters"

    [<Test>]
    let ``Calling register post, with password without invalid chars, returns expected result`` () =
        let expectedUsername = String.random 32
        let invalidCharsPassword = makeBadPasswordWithInvalidChars 32
        let expectedConfirmPassword = invalidCharsPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password contains invalid characters")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername invalidCharsPassword expectedConfirmPassword
                expectedEmail expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password contains invalid characters"

    [<Test>]
    let ``Calling register post, with non-matching passwords, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let nonMatchingConfirmPassword = Password.random 32
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("confirmPassword", "Passwords do not match")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername expectedPassword nonMatchingConfirmPassword
                expectedEmail expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "confirmPassword" "Passwords do not match"

    [<Test>]
    let ``Calling register post, with null email, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let nullEmail:string = null
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is required")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername expectedPassword expectedConfirmPassword nullEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty nullEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is required"

    [<Test>]
    let ``Calling register post, with empty email, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let emptyEmail = String.empty
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is required")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername expectedPassword expectedConfirmPassword emptyEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty emptyEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is required"

    [<Test>]
    let ``Calling register post, with email longer than 254 characters, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let longEmail = String.random 255
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is longer than 254 characters")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername expectedPassword expectedConfirmPassword longEmail
                expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty longEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is longer than 254 characters"

    [<Test>]
    let ``Calling register post, with email in invalid format, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let invalidFormatEmail = String.random 64
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is not in the correct format")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername expectedPassword expectedConfirmPassword
                invalidFormatEmail expectedNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty invalidFormatEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is not in the correct format"

    [<Test>]
    let ``Calling register post, with null nickname, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let nullNickname:string = null

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is required")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername expectedPassword expectedConfirmPassword expectedEmail
                nullNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            nullNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "nickname" "Nickname is required"

    [<Test>]
    let ``Calling register post, with empty nickname, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let emptyNickname = String.empty

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is required")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername expectedPassword expectedConfirmPassword expectedEmail
                emptyNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            emptyNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "nickname" "Nickname is required"

    [<Test>]
    let ``Calling register post, with nickname longer than 64 characters, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let longNickname = String.random 65

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is longer than 64 characters")

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername expectedPassword expectedConfirmPassword expectedEmail
                longNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            longNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "nickname" "Nickname is longer than 64 characters"

    [<Test>]
    let ``Calling register post, with existing username in data store, returns expected result`` () =
        let existingUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mediatorMock.Object, keyGenerator)

        let viewModel =
            bindSelfServiceRegisterViewModel2 existingUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel existingUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is unavailable"

    [<Test>]
    let ``Calling register post, with existing email in data store, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let existingEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mediatorMock.Object, keyGenerator)

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername expectedPassword expectedConfirmPassword existingEmail
                expectedNickname
        let result = controller.Register viewModel

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? ISelfServiceRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        testSelfServiceRegisterViewModel actualViewModel expectedUsername String.empty String.empty existingEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is already registered"

    [<Test>]
    let ``Calling register post, with no existing username or email in data store, returns expected result`` () =
        let expectedId = Guid.random ()

        let commandResult = SelfServiceRegisterCommand.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator (Some expectedId)
        let controller = new SelfServiceController(mediatorMock.Object, keyGenerator)

        let password = Password.random 32
        let viewModel =
            bindSelfServiceRegisterViewModel2 (String.random 32) password password (EmailAddress.random 32)
                (Password.random 32)
        let result = controller.Register viewModel

        mediatorMock.Verify()

        isNull result =! false
        result :? RedirectToRouteResult =! true

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "member"

        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "detail"

        let expectedIdString = String.ofGuid expectedId

        routeValues.ContainsKey("id") =! true
        routeValues.["id"].ToString().ToLowerInvariant() =! expectedIdString
