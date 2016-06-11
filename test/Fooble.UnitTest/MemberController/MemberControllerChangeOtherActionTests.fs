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
module MemberControllerChangeOtherActionTests =

    [<Test>]
    let ``Calling change other, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let heading = "Member"
        let subHeading = "Change Other"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let queryResult = MemberExistsQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let result = controller.ChangeOther(notFoundId)

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
    let ``Calling change other, with successful parameters, returns expected result`` () =
        let id = Guid.random ()

        let queryResult = MemberExistsQuery.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)
        let result = controller.ChangeOther(id)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel actualViewModel id String.empty

    [<Test>]
    let ``Calling change other post, with null new nickname, returns expected result`` () =
        let id = Guid.random ()
        let nullNickname:string = null

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newNickname", "New nickname is required")

        let viewModel = bindMemberChangeOtherViewModel2 id nullNickname
        let result = controller.ChangeOther(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel actualViewModel id nullNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newNickname" "New nickname is required"

    [<Test>]
    let ``Calling change other post, with empty new nickname, returns expected result`` () =
        let id = Guid.random ()
        let emptyNickname = String.empty

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newNickname", "New nickname is required")

        let viewModel = bindMemberChangeOtherViewModel2 id emptyNickname
        let result = controller.ChangeOther(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel actualViewModel id emptyNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newNickname" "New nickname is required"

    [<Test>]
    let ``Calling change other post, with new nickname longer than 64 characters, returns expected result`` () =
        let id = Guid.random ()
        let longNickname = String.random 65

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mock (), keyGenerator)
        controller.ModelState.AddModelError("newNickname", "New nickname is longer than 64 characters")

        let viewModel = bindMemberChangeOtherViewModel2 id longNickname
        let result = controller.ChangeOther(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel actualViewModel id longNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newNickname" "New nickname is longer than 64 characters"

    [<Test>]
    let ``Calling change other post, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let nickname = String.random 64

        let heading = "Member"
        let subHeading = "Change Other"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let commandResult = MemberChangeOtherCommand.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangeOtherViewModel2 notFoundId nickname
        let result = controller.ChangeOther(notFoundId, viewModel)

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
    let ``Calling change other post, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let nickname = String.random 64

        let commandResult = MemberChangeOtherCommand.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)

        let viewModel = bindMemberChangeOtherViewModel2 id nickname
        let result = controller.ChangeOther(id, viewModel)

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
