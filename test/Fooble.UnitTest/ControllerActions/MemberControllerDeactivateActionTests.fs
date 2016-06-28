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
module MemberControllerDeactivateActionTests =

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

    let private setupForPostActionTest result id currentPassword =
        let mediatorMock = Mock<IMediator>()
        let controller =
            match result with
            | NotApplicable -> makeMemberController None None
            | Success ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberDeactivateCommand>()))
                      .Returns(MemberDeactivateCommand.successResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | NotFound ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberDeactivateCommand>()))
                      .Returns(MemberDeactivateCommand.notFoundResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | IncorrectPassword ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberDeactivateCommand>()))
                      .Returns(MemberDeactivateCommand.incorrectPasswordResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
        let (viewModel, modelState) = bindMemberDeactivateViewModel id currentPassword
        controller.ModelState.Merge(modelState)
        (controller, viewModel, mediatorMock)

    [<Test>]
    let ``Calling deactivate get action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let (controller, mediatorMock) = setupForGetActionTest (QueryResult.Success(id))
        let result = controller.Deactivate(id)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberExistsQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberDeactivateViewModel =! true
        let viewModel = viewResult.Model :?> IMemberDeactivateViewModel
        testMemberDeactivateViewModel viewModel id String.Empty

    [<Test>]
    let ``Calling deactivate get action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let heading = "Member"
        let subHeading = "Deactivate"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, mediatorMock) = setupForGetActionTest QueryResult.NotFound
        let result = controller.Deactivate(notFoundId)
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
    let ``Calling deactivate post action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let (controller, viewModel, mediatorMock) = setupForPostActionTest Success id currentPassword
        let result = controller.Deactivate(id, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberDeactivateCommand>())), Times.Once())
        isNull result =! false
        result :? RedirectToRouteResult =! true
        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues
        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "home"
        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "index"

    [<Test>]
    let ``Calling deactivate post action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let heading = "Member"
        let subHeading = "Deactivate"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, viewModel, mediatorMock) = setupForPostActionTest NotFound notFoundId currentPassword
        let result = controller.Deactivate(notFoundId, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberDeactivateCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling deactivate post action, with incorrect current password, returns expected result`` () =
        let id = Guid.NewGuid()
        let incorrectPassword = randomPassword 32
        let (controller, viewModel, mediatorMock) = setupForPostActionTest IncorrectPassword id incorrectPassword
        let result = controller.Deactivate(id, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberDeactivateCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberDeactivateViewModel =! true
        let viewModel = viewResult.Model :?> IMemberDeactivateViewModel
        testMemberDeactivateViewModel viewModel id String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "currentPassword" "Current password is incorrect"
