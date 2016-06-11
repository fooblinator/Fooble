namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open Fooble.Web.Controllers
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module MemberControllerRegisterActionTests =

    [<Test>]
    let ``Calling register, returns expected result`` () =
        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        let result = controller.Register()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel String.empty String.empty String.empty String.empty String.empty

    [<Test>]
    let ``Calling register post, with null username, returns expected result`` () =
        let nullUsername:string = null
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is required")

        let viewModel = bindMemberRegisterViewModel2 nullUsername password confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel nullUsername String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is required"

    [<Test>]
    let ``Calling register post, with empty username, returns expected result`` () =
        let emptyUsername = String.empty
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is required")

        let viewModel = bindMemberRegisterViewModel2 emptyUsername password confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel emptyUsername String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is required"

    [<Test>]
    let ``Calling register post, with username shorter than 3 characters, returns expected result`` () =
        let shortUsername = String.random 2
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is shorter than 3 characters")

        let viewModel = bindMemberRegisterViewModel2 shortUsername password confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel shortUsername String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is shorter than 3 characters"

    [<Test>]
    let ``Calling register post, with username longer than 32 characters, returns expected result`` () =
        let longUsername = String.random 33
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is longer than 32 characters")

        let viewModel = bindMemberRegisterViewModel2 longUsername password confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel longUsername String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is longer than 32 characters"

    [<Test>]
    let ``Calling register post, with username in invalid format, returns expected result`` () =
        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username",
            "Username is not in the correct format (lowercase alphanumeric)")

        let viewModel = bindMemberRegisterViewModel2 invalidFormatUsername password confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel invalidFormatUsername String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is not in the correct format (lowercase alphanumeric)"

    [<Test>]
    let ``Calling register post, with null password, returns expected result`` () =
        let username = String.random 32
        let nullPassword:string = null
        let confirmPassword = nullPassword
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is required")

        let viewModel = bindMemberRegisterViewModel2 username nullPassword confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password is required"

    [<Test>]
    let ``Calling register post, with empty password, returns expected result`` () =
        let username = String.random 32
        let emptyPassword = String.empty
        let confirmPassword = emptyPassword
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is required")

        let viewModel = bindMemberRegisterViewModel2 username emptyPassword confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password is required"

    [<Test>]
    let ``Calling register post, with password shorter than 8 characters, returns expected result`` () =
        let username = String.random 32
        let shortPassword = Password.random 7
        let confirmPassword = shortPassword
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is shorter than 8 characters")

        let viewModel = bindMemberRegisterViewModel2 username shortPassword confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password is shorter than 8 characters"

    [<Test>]
    let ``Calling register post, with password longer than 32 characters, returns expected result`` () =
        let username = String.random 32
        let longPassword = Password.random 33
        let confirmPassword = longPassword
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password is longer than 32 characters")

        let viewModel = bindMemberRegisterViewModel2 username longPassword confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password is longer than 32 characters"

    [<Test>]
    let ``Calling register post, with password without digits, returns expected result`` () =
        let username = String.random 32
        let noDigitsPassword = makeBadPasswordWithoutDigits 32
        let confirmPassword = noDigitsPassword
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password does not contain any numbers")

        let viewModel = bindMemberRegisterViewModel2 username noDigitsPassword confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password does not contain any numbers"

    [<Test>]
    let ``Calling register post, with password without lower alphas, returns expected result`` () =
        let username = String.random 32
        let noLowerAlphasPassword = makeBadPasswordWithoutLowerAlphas 32
        let confirmPassword = noLowerAlphasPassword
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password does not contain any lower-case letters")

        let viewModel = bindMemberRegisterViewModel2 username noLowerAlphasPassword confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password does not contain any lower-case letters"

    [<Test>]
    let ``Calling register post, with password without upper alphas, returns expected result`` () =
        let username = String.random 32
        let noUpperAlphasPassword = makeBadPasswordWithoutUpperAlphas 32
        let confirmPassword = noUpperAlphasPassword
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password does not contain any upper-case letters")

        let viewModel = bindMemberRegisterViewModel2 username noUpperAlphasPassword confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password does not contain any upper-case letters"

    [<Test>]
    let ``Calling register post, with password without special chars, returns expected result`` () =
        let username = String.random 32
        let noSpecialCharsPassword = makeBadPasswordWithoutSpecialChars 32
        let confirmPassword = noSpecialCharsPassword
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password  does not contain any special characters")

        let viewModel = bindMemberRegisterViewModel2 username noSpecialCharsPassword confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password  does not contain any special characters"

    [<Test>]
    let ``Calling register post, with password without invalid chars, returns expected result`` () =
        let username = String.random 32
        let invalidCharsPassword = makeBadPasswordWithInvalidChars 32
        let confirmPassword = invalidCharsPassword
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("password", "Password contains invalid characters")

        let viewModel = bindMemberRegisterViewModel2 username invalidCharsPassword confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "password" "Password contains invalid characters"

    [<Test>]
    let ``Calling register post, with non-matching passwords, returns expected result`` () =
        let username = String.random 32
        let password = Password.random 32
        let nonMatchingPassword = Password.random 32
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("confirmPassword", "Passwords do not match")

        let viewModel = bindMemberRegisterViewModel2 username password nonMatchingPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "confirmPassword" "Passwords do not match"

    [<Test>]
    let ``Calling register post, with null email, returns expected result`` () =
        let username = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let nullEmail:string = null
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is required")

        let viewModel = bindMemberRegisterViewModel2 username password confirmPassword nullEmail nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty nullEmail nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is required"

    [<Test>]
    let ``Calling register post, with empty email, returns expected result`` () =
        let username = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let emptyEmail = String.empty
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is required")

        let viewModel = bindMemberRegisterViewModel2 username password confirmPassword emptyEmail nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty emptyEmail nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is required"

    [<Test>]
    let ``Calling register post, with email longer than 254 characters, returns expected result`` () =
        let username = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let longEmail = String.random 255
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is longer than 254 characters")

        let viewModel = bindMemberRegisterViewModel2 username password confirmPassword longEmail nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty longEmail nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is longer than 254 characters"

    [<Test>]
    let ``Calling register post, with email in invalid format, returns expected result`` () =
        let username = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let invalidFormatEmail = String.random 64
        let nickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is not in the correct format")

        let viewModel = bindMemberRegisterViewModel2 username password confirmPassword invalidFormatEmail nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty invalidFormatEmail nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is not in the correct format"

    [<Test>]
    let ``Calling register post, with null nickname, returns expected result`` () =
        let username = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let nullNickname:string = null

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is required")

        let viewModel = bindMemberRegisterViewModel2 username password confirmPassword email nullNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email nullNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "nickname" "Nickname is required"

    [<Test>]
    let ``Calling register post, with empty nickname, returns expected result`` () =
        let username = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let emptyNickname = String.empty

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is required")

        let viewModel = bindMemberRegisterViewModel2 username password confirmPassword email emptyNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email emptyNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "nickname" "Nickname is required"

    [<Test>]
    let ``Calling register post, with nickname longer than 64 characters, returns expected result`` () =
        let username = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let longNickname = String.random 65

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is longer than 64 characters")

        let viewModel = bindMemberRegisterViewModel2 username password confirmPassword email longNickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty email longNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "nickname" "Nickname is longer than 64 characters"

    [<Test>]
    let ``Calling register post, with unavailable email, returns expected result`` () =
        let username = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let unavailableEmail = EmailAddress.random 32
        let nickname = String.random 64

        let commandResult = MemberRegisterCommand.unavailableEmailResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberRegisterViewModel2 username password confirmPassword unavailableEmail nickname
        let result = controller.Register viewModel

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty unavailableEmail nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is already registered"

    [<Test>]
    let ``Calling register post, with unavailable username, returns expected result`` () =
        let unavailableUsername = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let commandResult = MemberRegisterCommand.unavailableUsernameResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberRegisterViewModel2 unavailableUsername password confirmPassword email nickname
        let result = controller.Register viewModel

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel unavailableUsername String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is unavailable"

    [<Test>]
    let ``Calling register post, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let username = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let commandResult = MemberRegisterCommand.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator (Some(id))
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberRegisterViewModel2 username password confirmPassword email nickname
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

        let idString = String.ofGuid id

        routeValues.ContainsKey("id") =! true
        routeValues.["id"].ToString().ToLowerInvariant() =! idString
