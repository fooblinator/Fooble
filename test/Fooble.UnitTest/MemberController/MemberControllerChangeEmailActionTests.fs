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
module MemberControllerChangeEmailActionTests =

    [<Test>]
    let ``Calling change email, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let heading = "Member"
        let subHeading = "Change Email"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let queryResult = MemberExistsQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let result = controller.ChangeEmail(notFoundId)

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
    let ``Calling change email, with successful parameters, returns expected result`` () =
        let id = Guid.random ()

        let queryResult = MemberExistsQuery.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)
        let result = controller.ChangeEmail(id)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel actualViewModel id String.empty String.empty

    [<Test>]
    let ``Calling change email post, with null new email, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let nullEmail:string = null

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newEmail", "New email is required")

        let viewModel = bindMemberChangeEmailViewModel2 id currentPassword nullEmail
        let result = controller.ChangeEmail(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel actualViewModel id String.empty nullEmail

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newEmail" "New email is required"

    [<Test>]
    let ``Calling change email post, with empty new email, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let emptyEmail = String.empty

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newEmail", "New email is required")

        let viewModel = bindMemberChangeEmailViewModel2 id currentPassword emptyEmail
        let result = controller.ChangeEmail(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel actualViewModel id String.empty emptyEmail

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newEmail" "New email is required"

    [<Test>]
    let ``Calling change email post, with new email longer than 254 characters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let longEmail = EmailAddress.random 255

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newEmail", "New email is longer than 254 characters")

        let viewModel = bindMemberChangeEmailViewModel2 id currentPassword longEmail
        let result = controller.ChangeEmail(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel actualViewModel id String.empty longEmail

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newEmail" "New email is longer than 254 characters"

    [<Test>]
    let ``Calling change email post, with new email in invalid format, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let invalidFormatEmail = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newEmail", "New email is not in the correct format")

        let viewModel = bindMemberChangeEmailViewModel2 id currentPassword invalidFormatEmail
        let result = controller.ChangeEmail(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel actualViewModel id String.empty invalidFormatEmail

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newEmail" "New email is not in the correct format"

    [<Test>]
    let ``Calling change email post, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let currentPassword = Password.random 32
        let newEmail = EmailAddress.random 32

        let heading = "Member"
        let subHeading = "Change Email"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let commandResult = MemberChangeEmailCommand.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangeEmailViewModel2 notFoundId currentPassword newEmail
        let result = controller.ChangeEmail(notFoundId, viewModel)

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
    let ``Calling change email post, with incorrect current password, returns expected result`` () =
        let id = Guid.random ()
        let incorrectPassword = Password.random 32
        let newEmail = EmailAddress.random 32

        let commandResult = MemberChangeEmailCommand.incorrectPasswordResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangeEmailViewModel2 id incorrectPassword newEmail
        let result = controller.ChangeEmail(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel actualViewModel id String.empty newEmail

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change email post, with unavailable email, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let unavailableEmail = EmailAddress.random 32

        let commandResult = MemberChangeEmailCommand.unavailableEmailResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangeEmailViewModel2 id currentPassword unavailableEmail
        let result = controller.ChangeEmail(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel actualViewModel id String.empty unavailableEmail

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newEmail" "Email is already registered"

    [<Test>]
    let ``Calling change email post, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let newEmail = EmailAddress.random 32

        let commandResult = MemberChangeEmailCommand.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangeEmailViewModel2 id currentPassword newEmail
        let result = controller.ChangeEmail(id, viewModel)

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
