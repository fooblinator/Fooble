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
module MemberControllerChangePasswordActionTests =

    [<Test>]
    let ``Calling change password, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let heading = "Member"
        let subHeading = "Change Password"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let queryResult = MemberExistsQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let result = controller.ChangePassword(notFoundId)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change password, with successful parameters, returns expected result`` () =
        let id = Guid.random ()

        let queryResult = MemberExistsQuery.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)
        let result = controller.ChangePassword(id)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

    [<Test>]
    let ``Calling change password post, with null new password, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let nullPassword:string = null
        let confirmPassword = nullPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password is required")

        let viewModel = bindMemberChangePasswordViewModel2 id currentPassword nullPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password is required"

    [<Test>]
    let ``Calling change password post, with empty new password, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let emptyPassword = String.empty
        let confirmPassword = emptyPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password is required")

        let viewModel = bindMemberChangePasswordViewModel2 id currentPassword emptyPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password is required"

    [<Test>]
    let ``Calling change password post, with new password shorter than 8 characters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let shortPassword = Password.random 7
        let confirmPassword = shortPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password is shorter than 8 characters")

        let viewModel = bindMemberChangePasswordViewModel2 id currentPassword shortPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password is shorter than 8 characters"

    [<Test>]
    let ``Calling change password post, with new password longer than 32 characters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let longPassword = Password.random 33
        let confirmPassword = longPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password is longer than 32 characters")

        let viewModel = bindMemberChangePasswordViewModel2 id currentPassword longPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password is longer than 32 characters"

    [<Test>]
    let ``Calling change password post, with new password without digits, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let noDigitsPassword = makeBadPasswordWithoutDigits 32
        let confirmPassword = noDigitsPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password does not contain any numbers")

        let viewModel = bindMemberChangePasswordViewModel2 id currentPassword noDigitsPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password does not contain any numbers"

    [<Test>]
    let ``Calling change password post, with new password without lower alphas, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let noLowerAlphasPassword = makeBadPasswordWithoutLowerAlphas 32
        let confirmPassword = noLowerAlphasPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password does not contain any lower-case letters")

        let viewModel = bindMemberChangePasswordViewModel2 id currentPassword noLowerAlphasPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password does not contain any lower-case letters"

    [<Test>]
    let ``Calling change password post, with new password without upper alphas, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let noUpperAlphasPassword = makeBadPasswordWithoutUpperAlphas 32
        let confirmPassword = noUpperAlphasPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password does not contain any upper-case letters")

        let viewModel = bindMemberChangePasswordViewModel2 id currentPassword noUpperAlphasPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password does not contain any upper-case letters"

    [<Test>]
    let ``Calling change password post, with new password without special chars, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let noSpecialCharsPassword = makeBadPasswordWithoutSpecialChars 32
        let confirmPassword = noSpecialCharsPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password does not contain any special characters")

        let viewModel = bindMemberChangePasswordViewModel2 id currentPassword noSpecialCharsPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password does not contain any special characters"

    [<Test>]
    let ``Calling change password post, with new password without invalid chars, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let invalidCharsPassword = makeBadPasswordWithInvalidChars 32
        let confirmPassword = invalidCharsPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password contains invalid characters")

        let viewModel = bindMemberChangePasswordViewModel2 id currentPassword invalidCharsPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password contains invalid characters"

    [<Test>]
    let ``Calling change password post, with non-matching new passwords, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let newPassword = Password.random 32
        let nonMatchingPassword = Password.random 32

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("confirmPassword", "New passwords do not match")

        let viewModel = bindMemberChangePasswordViewModel2 id currentPassword newPassword nonMatchingPassword
        let result = controller.ChangePassword(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "confirmPassword" "New passwords do not match"

    [<Test>]
    let ``Calling change password post, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let currentPassword = Password.random 32
        let newPassword = Password.random 32
        let confirmPassword = newPassword

        let heading = "Member"
        let subHeading = "Change Password"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let commandResult = MemberChangePasswordCommand.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangePasswordViewModel2 notFoundId currentPassword newPassword confirmPassword
        let result = controller.ChangePassword(notFoundId, viewModel)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change password post, with incorrect current password, returns expected result`` () =
        let id = Guid.random ()
        let incorrectPassword = Password.random 32
        let newPassword = Password.random 32
        let confirmPassword = newPassword

        let commandResult = MemberChangePasswordCommand.incorrectPasswordResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangePasswordViewModel2 id incorrectPassword newPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change password post, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let newPassword = Password.random 32
        let confirmPassword = newPassword

        let commandResult = MemberChangePasswordCommand.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangePasswordViewModel2 id currentPassword newPassword confirmPassword
        let result = controller.ChangePassword(id, viewModel)

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
