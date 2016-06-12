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
module MemberControllerDeactivateActionTests =

    [<Test>]
    let ``Calling deactivate, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let heading = "Member"
        let subHeading = "Deactivate"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let queryResult = MemberExistsQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let result = controller.Deactivate(notFoundId)

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
    let ``Calling deactivate, with successful parameters, returns expected result`` () =
        let id = Guid.random ()

        let queryResult = MemberExistsQuery.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)
        let result = controller.Deactivate(id)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberDeactivateViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberDeactivateViewModel
        testMemberDeactivateViewModel actualViewModel id String.empty

    [<Test>]
    let ``Calling deactivate post, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let currentPassword = Password.random 32

        let heading = "Member"
        let subHeading = "Deactivate"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let commandResult = MemberDeactivateCommand.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberDeactivateViewModel2 notFoundId currentPassword
        let result = controller.Deactivate(notFoundId, viewModel)

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
    let ``Calling deactivate post, with incorrect current password, returns expected result`` () =
        let id = Guid.random ()
        let incorrectPassword = Password.random 32

        let commandResult = MemberDeactivateCommand.incorrectPasswordResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberDeactivateViewModel2 id incorrectPassword
        let result = controller.Deactivate(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberDeactivateViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberDeactivateViewModel
        testMemberDeactivateViewModel actualViewModel id String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling deactivate post, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32

        let commandResult = MemberDeactivateCommand.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberDeactivateViewModel2 id currentPassword
        let result = controller.Deactivate(id, viewModel)

        mediatorMock.Verify()

        isNull result =! false
        result :? RedirectToRouteResult =! true

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "home"

        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "index"
