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
    let ``Calling change password, with matching member id in data store, returns expected result`` () =
        let matchingId = Guid.random ()
        let emptyCurrentPassword = String.empty
        let emptyNewPassword = String.empty
        let emptyConfirmPassword = String.empty

        let queryResult = MemberExistsQuery.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)
        let result = controller.ChangePassword(matchingId)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel matchingId emptyCurrentPassword emptyNewPassword
            emptyConfirmPassword

    [<Test>]
    let ``Calling change password, with no matching member id in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Password"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let queryResult = MemberExistsQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let result = controller.ChangePassword(nonMatchingId)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling change password post, with null new password, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let nullNewPassword:string = null
        let expectedConfirmPassword = nullNewPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password is required")

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword nullNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password is required"

    [<Test>]
    let ``Calling change password post, with empty new password, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let emptyNewPassword = String.empty
        let expectedConfirmPassword = emptyNewPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password is required")

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword emptyNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password is required"

    [<Test>]
    let ``Calling change password post, with new password shorter than 8 characters, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let shortNewPassword = Password.random 7
        let expectedConfirmPassword = shortNewPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password is shorter than 8 characters")

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword shortNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password is shorter than 8 characters"

    [<Test>]
    let ``Calling change password post, with new password longer than 32 characters, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let longNewPassword = Password.random 33
        let expectedConfirmPassword = longNewPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password is longer than 32 characters")

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword longNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password is longer than 32 characters"

    [<Test>]
    let ``Calling change password post, with new password without digits, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let noDigitsNewPassword = makeBadPasswordWithoutDigits 32
        let expectedConfirmPassword = noDigitsNewPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password does not contain any numbers")

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword noDigitsNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password does not contain any numbers"

    [<Test>]
    let ``Calling change password post, with new password without lower alphas, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let noLowerAlphasNewPassword = makeBadPasswordWithoutLowerAlphas 32
        let expectedConfirmPassword = noLowerAlphasNewPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password does not contain any lower-case letters")

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword noLowerAlphasNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password does not contain any lower-case letters"

    [<Test>]
    let ``Calling change password post, with new password without upper alphas, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let noUpperAlphasNewPassword = makeBadPasswordWithoutUpperAlphas 32
        let expectedConfirmPassword = noUpperAlphasNewPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password does not contain any upper-case letters")

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword noUpperAlphasNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password does not contain any upper-case letters"

    [<Test>]
    let ``Calling change password post, with new password without special chars, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let noSpecialCharsNewPassword = makeBadPasswordWithoutSpecialChars 32
        let expectedConfirmPassword = noSpecialCharsNewPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password does not contain any special characters")

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword noSpecialCharsNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password does not contain any special characters"

    [<Test>]
    let ``Calling change password post, with new password without invalid chars, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let invalidCharsNewPassword = makeBadPasswordWithInvalidChars 32
        let expectedConfirmPassword = invalidCharsNewPassword

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newPassword", "New password contains invalid characters")

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword invalidCharsNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newPassword" "New password contains invalid characters"

    [<Test>]
    let ``Calling change password post, with non-matching new passwords, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let nonMatchingConfirmPassword = Password.random 32

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("confirmPassword", "New passwords do not match")

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword expectedNewPassword
                nonMatchingConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "confirmPassword" "New passwords do not match"

    [<Test>]
    let ``Calling change password post, with no matching member id in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Password"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let commandResult = MemberChangePasswordCommand.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let newPassword = Password.random 32
        let confirmPassword = newPassword
        let viewModel =
            bindMemberChangePasswordViewModel2 nonMatchingId (Password.random 32) newPassword confirmPassword
        let result = controller.ChangePassword(nonMatchingId, viewModel)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling change password post, with incorrect current password, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let commandResult = MemberChangePasswordCommand.incorrectPasswordResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword expectedNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change password post, with matching member id in data store, and correct current password, returns expected result`` () =
        let matchingId = Guid.random ()
        let matchingIdString = String.ofGuid matchingId

        let commandResult = MemberChangePasswordCommand.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let newPassword = Password.random 32
        let confirmPassword = newPassword
        let viewModel = bindMemberChangePasswordViewModel2 matchingId (Password.random 32) newPassword confirmPassword
        let result = controller.ChangePassword(matchingId, viewModel)

        mediatorMock.Verify()

        isNull result =! false
        result :? RedirectToRouteResult =! true

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "member"

        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "detail"

        routeValues.ContainsKey("id") =! true
        routeValues.["id"].ToString().ToLowerInvariant() =! matchingIdString
