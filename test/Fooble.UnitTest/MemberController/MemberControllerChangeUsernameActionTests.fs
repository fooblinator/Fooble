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
module MemberControllerChangeUsernameActionTests =

    [<Test>]
    let ``Calling change username, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let queryResult = MemberExistsQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let result = controller.ChangeUsername(notFoundId)

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
    let ``Calling change username, with successful parameters, returns expected result`` () =
        let id = Guid.random ()

        let queryResult = MemberExistsQuery.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)
        let result = controller.ChangeUsername(id)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel id String.empty String.empty

    [<Test>]
    let ``Calling change username post, with null new username, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let nullUsername:string = null

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newUsername", "New username is required")

        let viewModel = bindMemberChangeUsernameViewModel2 id currentPassword nullUsername
        let result = controller.ChangeUsername(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel id String.empty nullUsername

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newUsername" "New username is required"

    [<Test>]
    let ``Calling change username post, with empty new username, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let emptyUsername = String.empty

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newUsername", "New username is required")

        let viewModel = bindMemberChangeUsernameViewModel2 id currentPassword emptyUsername
        let result = controller.ChangeUsername(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel id String.empty emptyUsername

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newUsername" "New username is required"

    [<Test>]
    let ``Calling change username post, with new username shorter than 3 characters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let shortUsername = String.random 2

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newUsername", "New username is shorter than 3 characters")

        let viewModel = bindMemberChangeUsernameViewModel2 id currentPassword shortUsername
        let result = controller.ChangeUsername(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel id String.empty shortUsername

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newUsername" "New username is shorter than 3 characters"

    [<Test>]
    let ``Calling change username post, with new username longer than 32 characters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let longUsername = String.random 33

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newUsername", "New username is longer than 32 characters")

        let viewModel = bindMemberChangeUsernameViewModel2 id currentPassword longUsername
        let result = controller.ChangeUsername(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel id String.empty longUsername

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newUsername" "New username is longer than 32 characters"

    [<Test>]
    let ``Calling change username post, with new username in invalid format, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newUsername",
            "New username is not in the correct format (lowercase alphanumeric)")

        let viewModel = bindMemberChangeUsernameViewModel2 id currentPassword invalidFormatUsername
        let result = controller.ChangeUsername(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel id String.empty invalidFormatUsername

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newUsername"
            "New username is not in the correct format (lowercase alphanumeric)"

    [<Test>]
    let ``Calling change username post, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let currentPassword = Password.random 32
        let newUsername = String.random 32

        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let commandResult = MemberChangeUsernameCommand.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangeUsernameViewModel2 notFoundId currentPassword newUsername
        let result = controller.ChangeUsername(notFoundId, viewModel)

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
    let ``Calling change username post, with incorrect current password, returns expected result`` () =
        let id = Guid.random ()
        let incorrectPassword = Password.random 32
        let newUsername = String.random 32

        let commandResult = MemberChangeUsernameCommand.incorrectPasswordResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangeUsernameViewModel2 id incorrectPassword newUsername
        let result = controller.ChangeUsername(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel id String.empty newUsername

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change username post, with unavailable username, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let unavailableUsername = String.random 32

        let commandResult = MemberChangeUsernameCommand.unavailableUsernameResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangeUsernameViewModel2 id currentPassword unavailableUsername
        let result = controller.ChangeUsername(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel id String.empty unavailableUsername

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newUsername" "Username is unavailable"

    [<Test>]
    let ``Calling change username post, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let newUsername = String.random 32

        let commandResult = MemberChangeUsernameCommand.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangeUsernameViewModel2 id currentPassword newUsername
        let result = controller.ChangeUsername(id, viewModel)

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
