namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module MemberControllerRegisterActionTests =

    type private Result = NotApplicable | Success | UnavailableUsername | UnavailableEmail

    let private setupForGetActionTest () = makeMemberController None None

    let private setupForPostActionTest result idGeneratorResult username password confirmPassword email nickname avatarData =
        let mediatorMock = Mock<IMediator>()
        let controller =
            match result with
            | NotApplicable -> makeMemberController None None
            | Success ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberRegisterCommand>()))
                      .Returns(MemberRegisterCommand.successResult).End
                  makeMemberController (Some(mediatorMock.Object)) idGeneratorResult
            | UnavailableUsername ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberRegisterCommand>()))
                      .Returns(MemberRegisterCommand.unavailableUsernameResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | UnavailableEmail ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberRegisterCommand>()))
                      .Returns(MemberRegisterCommand.unavailableEmailResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword email nickname avatarData
        controller.ModelState.Merge(modelState)
        (controller, viewModel, mediatorMock)

    [<Test>]
    let ``Calling register get action, returns expected result`` () =
        let controller = setupForGetActionTest ()
        let result = controller.Register()
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        viewModel.AvatarData <>! String.Empty
        testMemberRegisterViewModel viewModel String.Empty String.Empty String.Empty String.Empty String.Empty None

    [<Test>]
    let ``Calling register post action, with successful parameters, and default submit, returns expected result`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, mediatorMock) =
            setupForPostActionTest Success (Some(id)) username password confirmPassword email nickname avatarData
        let result = controller.Register(viewModel, "default")
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberRegisterCommand>())), Times.Once())
        isNull result =! false
        result :? RedirectToRouteResult =! true
        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues
        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "member"
        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "detail"
        routeValues.ContainsKey("id") =! true
        routeValues.["id"].ToString().ToLowerInvariant() =! string id

    [<Test>]
    let ``Calling register post action, with successful parameters, and random submit, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password confirmPassword email nickname avatarData
        let result = controller.Register(viewModel, "random")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        viewModel.AvatarData <>! avatarData
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname None

    [<Test>]
    let ``Calling register post action, with null username, returns expected result`` () =
        let nullUsername:string = null
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None nullUsername password confirmPassword email nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel nullUsername String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is required"

    [<Test>]
    let ``Calling register post action, with empty username, returns expected result`` () =
        let emptyUsername = String.Empty
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None emptyUsername password confirmPassword email nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel emptyUsername String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is required"

    [<Test>]
    let ``Calling register post action, with username shorter than 3 characters, returns expected result`` () =
        let shortUsername = randomString 2
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None shortUsername password confirmPassword email nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel shortUsername String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is shorter than 3 characters"

    [<Test>]
    let ``Calling register post action, with username longer than 32 characters, returns expected result`` () =
        let longUsername = randomString 33
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None longUsername password confirmPassword email nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel longUsername String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is longer than 32 characters"

    [<Test>]
    let ``Calling register post action, with username in invalid format, returns expected result`` () =
        let invalidFormatUsername = sprintf "-%s-" (randomString 30)
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None invalidFormatUsername password confirmPassword email nickname
                avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel invalidFormatUsername String.Empty String.Empty email nickname
            (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is not in the correct format (lowercase alphanumeric)"

    [<Test>]
    let ``Calling register post action, with null password, returns expected result`` () =
        let username = randomString 32
        let nullPassword:string = null
        let confirmPassword = nullPassword
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username nullPassword confirmPassword email nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Calling register post action, with empty password, returns expected result`` () =
        let username = randomString 32
        let emptyPassword = String.Empty
        let confirmPassword = emptyPassword
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username emptyPassword confirmPassword email nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Calling register post action, with password shorter than 8 characters, returns expected result`` () =
        let username = randomString 32
        let shortPassword = randomString 7
        let confirmPassword = shortPassword
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username shortPassword confirmPassword email nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password is shorter than 8 characters"

    [<Test>]
    let ``Calling register post action, with password longer than 32 characters, returns expected result`` () =
        let username = randomString 32
        let longPassword = randomString 33
        let confirmPassword = longPassword
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username longPassword confirmPassword email nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password is longer than 32 characters"

    [<Test>]
    let ``Calling register post action, with password without digits, returns expected result`` () =
        let username = randomString 32
        let noDigitsPassword = randomPasswordWithoutDigitChars 32
        let confirmPassword = noDigitsPassword
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username noDigitsPassword confirmPassword email nickname
                avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password does not contain any numbers"

    [<Test>]
    let ``Calling register post action, with password without lower alphas, returns expected result`` () =
        let username = randomString 32
        let noLowerAlphasPassword = randomPasswordWithoutLowercaseChars 32
        let confirmPassword = noLowerAlphasPassword
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username noLowerAlphasPassword confirmPassword email nickname
                avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password does not contain any lower-case letters"

    [<Test>]
    let ``Calling register post action, with password without upper alphas, returns expected result`` () =
        let username = randomString 32
        let noUpperAlphasPassword = randomPasswordWithoutUppercaseChars 32
        let confirmPassword = noUpperAlphasPassword
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username noUpperAlphasPassword confirmPassword email nickname
                avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password does not contain any upper-case letters"

    [<Test>]
    let ``Calling register post action, with password without special chars, returns expected result`` () =
        let username = randomString 32
        let noSpecialCharsPassword = randomPasswordWithoutSpecialChars 32
        let confirmPassword = noSpecialCharsPassword
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username noSpecialCharsPassword confirmPassword email nickname
                avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password does not contain any special characters"

    [<Test>]
    let ``Calling register post action, with password in invalid chars, returns expected result`` () =
        let username = randomString 32
        let invalidCharsPassword = randomPasswordWithInvalidChars 32
        let confirmPassword = invalidCharsPassword
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username invalidCharsPassword confirmPassword email nickname
                avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password contains invalid characters"

    [<Test>]
    let ``Calling register post action, with non-matching new passwords, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let nonMatchingPassword = randomPassword 32
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password nonMatchingPassword email nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "confirmPassword" "Passwords do not match"

    [<Test>]
    let ``Calling register post action, with null email, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let nullEmail:string = null
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password confirmPassword nullEmail nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty nullEmail nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is required"

    [<Test>]
    let ``Calling register post action, with empty email, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let emptyEmail = String.Empty
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password confirmPassword emptyEmail nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty emptyEmail nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is required"

    [<Test>]
    let ``Calling register post action, with email longer than 256 characters, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let longEmail = randomEmail 257
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password confirmPassword longEmail nickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty longEmail nickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is longer than 256 characters"

    [<Test>]
    let ``Calling register post action, with email in invalid format, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let invalidFormatEmail = randomString 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password confirmPassword invalidFormatEmail nickname
                avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty invalidFormatEmail nickname
            (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is not in the correct format"

    [<Test>]
    let ``Calling register post action, with null nickname, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nullNickname:string = null
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password confirmPassword email nullNickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nullNickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "nickname" "Nickname is required"

    [<Test>]
    let ``Calling register post action, with empty nickname, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let emptyNickname = String.Empty
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password confirmPassword email emptyNickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email emptyNickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "nickname" "Nickname is required"

    [<Test>]
    let ``Calling register post action, with nickname longer than 64 characters, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let longNickname = randomString 65
        let avatarData = randomString 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password confirmPassword email longNickname avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email longNickname (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "nickname" "Nickname is longer than 64 characters"

    [<Test>]
    let ``Calling register post action, with null avatar data, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let nullAvatarData:string = null
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password confirmPassword email nickname nullAvatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(nullAvatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "avatarData" "Avatar data is required"

    [<Test>]
    let ``Calling register post action, with empty avatar data, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let emptyAvatarData = String.Empty
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password confirmPassword email nickname emptyAvatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(emptyAvatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "avatarData" "Avatar data is required"

    [<Test>]
    let ``Calling register post action, with avatar data longer than 32 characters, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let longAvatarData = randomString 33
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable None username password confirmPassword email nickname longAvatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(longAvatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "avatarData" "Avatar data is longer than 32 characters"

    [<Test>]
    let ``Calling change username post action, with unavailable username, returns expected result`` () =
        let unavailableUsername = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, mediatorMock) =
            setupForPostActionTest UnavailableUsername None unavailableUsername password confirmPassword email nickname
                avatarData
        let result = controller.Register(viewModel, "default")
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberRegisterCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel unavailableUsername String.Empty String.Empty email nickname
            (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is unavailable"

    [<Test>]
    let ``Calling change email post action, with unavailable email, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let unavailableEmail = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, mediatorMock) =
            setupForPostActionTest UnavailableEmail None username password confirmPassword unavailableEmail nickname
                avatarData
        let result = controller.Register(viewModel, "default")
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberRegisterCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty unavailableEmail nickname
            (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is already registered"
