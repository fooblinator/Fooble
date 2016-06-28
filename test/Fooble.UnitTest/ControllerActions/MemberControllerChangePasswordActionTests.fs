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
module MemberControllerChangePasswordActionTests =

    type private QueryResult =
        | Success of Guid
        | NotFound

    type private CommandResult = NotApplicable | Success | NotFound | IncorrectPassword

    let private setupForGetActionTest result =
        let result =
            match result with
            | QueryResult.Success(_) -> MemberExistsQuery.successResult
            | QueryResult.NotFound -> MemberExistsQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberExistsQuery>())).Returns(result).End
        let controller = makeMemberController (Some(mediatorMock.Object)) None
        (controller, mediatorMock)

    let private setupForPostActionTest result id currentPassword password confirmPassword =
        let mediatorMock = Mock<IMediator>()
        let controller =
            match result with
            | NotApplicable -> makeMemberController None None
            | Success ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangePasswordCommand>()))
                      .Returns(MemberChangePasswordCommand.successResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | NotFound ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangePasswordCommand>()))
                      .Returns(MemberChangePasswordCommand.notFoundResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | IncorrectPassword ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangePasswordCommand>()))
                      .Returns(MemberChangePasswordCommand.incorrectPasswordResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
        let (viewModel, modelState) = bindMemberChangePasswordViewModel id currentPassword password confirmPassword
        controller.ModelState.Merge(modelState)
        (controller, viewModel, mediatorMock)

    [<Test>]
    let ``Calling change password get action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let (controller, mediatorMock) = setupForGetActionTest (QueryResult.Success(id))
        let result = controller.ChangePassword(id)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberExistsQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty

    [<Test>]
    let ``Calling change password get action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let heading = "Member"
        let subHeading = "Change Password"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, mediatorMock) = setupForGetActionTest QueryResult.NotFound
        let result = controller.ChangePassword(notFoundId)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberExistsQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change password post action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let password = randomPassword 32
        let confirmPassword = password
        let (controller, viewModel, mediatorMock) =
            setupForPostActionTest Success id currentPassword password confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangePasswordCommand>())), Times.Once())
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
    let ``Calling change password post action, with null password, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let nullPassword:string = null
        let confirmPassword = nullPassword
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable id currentPassword nullPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Calling change password post action, with empty password, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let emptyPassword = String.Empty
        let confirmPassword = emptyPassword
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable id currentPassword emptyPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Calling change password post action, with password shorter than 8 characters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let shortPassword = randomString 7
        let confirmPassword = shortPassword
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable id currentPassword shortPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password is shorter than 8 characters"

    [<Test>]
    let ``Calling change password post action, with password longer than 32 characters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let longPassword = randomString 33
        let confirmPassword = longPassword
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable id currentPassword longPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password is longer than 32 characters"

    [<Test>]
    let ``Calling change password post action, with password without digits, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let noDigitsPassword = randomPasswordWithoutDigitChars 32
        let confirmPassword = noDigitsPassword
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable id currentPassword noDigitsPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password does not contain any numbers"

    [<Test>]
    let ``Calling change password post action, with password without lower alphas, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let noLowerAlphasPassword = randomPasswordWithoutLowercaseChars 32
        let confirmPassword = noLowerAlphasPassword
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable id currentPassword noLowerAlphasPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password does not contain any lower-case letters"

    [<Test>]
    let ``Calling change password post action, with password without upper alphas, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let noUpperAlphasPassword = randomPasswordWithoutUppercaseChars 32
        let confirmPassword = noUpperAlphasPassword
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable id currentPassword noUpperAlphasPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password does not contain any upper-case letters"

    [<Test>]
    let ``Calling change password post action, with password without special chars, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let noSpecialCharsPassword = randomPasswordWithoutSpecialChars 32
        let confirmPassword = noSpecialCharsPassword
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable id currentPassword noSpecialCharsPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password does not contain any special characters"

    [<Test>]
    let ``Calling change password post action, with password in invalid chars, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let invalidCharsPassword = randomPasswordWithInvalidChars 32
        let confirmPassword = invalidCharsPassword
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable id currentPassword invalidCharsPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "password" "Password contains invalid characters"

    [<Test>]
    let ``Calling change password post action, with non-matching new passwords, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let password = randomPassword 32
        let nonMatchingPassword = randomPassword 32
        let (controller, viewModel, _) =
            setupForPostActionTest NotApplicable id currentPassword password nonMatchingPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "confirmPassword" "Passwords do not match"

    [<Test>]
    let ``Calling change password post action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let password = randomPassword 32
        let confirmPassword = password
        let heading = "Member"
        let subHeading = "Change Password"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, viewModel, mediatorMock) =
            setupForPostActionTest NotFound notFoundId currentPassword password confirmPassword
        let result = controller.ChangePassword(notFoundId, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangePasswordCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change password post action, with incorrect current password, returns expected result`` () =
        let id = Guid.NewGuid()
        let incorrectPassword = randomPassword 32
        let password = randomPassword 32
        let confirmPassword = password
        let (controller, viewModel, mediatorMock) =
            setupForPostActionTest IncorrectPassword id incorrectPassword password confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangePasswordCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "currentPassword" "Current password is incorrect"
