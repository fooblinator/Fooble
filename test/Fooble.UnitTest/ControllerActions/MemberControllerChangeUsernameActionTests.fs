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
module MemberControllerChangeUsernameActionTests =

    type private QueryResult =
        | Success of Guid * string * string
        | NotFound

    type private CommandResult = NotApplicable | Success | NotFound | IncorrectPassword | UnavailableUsername

    let private setupForGetActionTest result =
        let result =
            match result with
            | QueryResult.Success(id, currentPassword, username) ->
                  MemberChangeUsernameViewModel.make id currentPassword username
                  |> MemberUsernameQuery.makeSuccessResult
            | QueryResult.NotFound -> MemberUsernameQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberUsernameQuery>())).Returns(result).End
        let controller = makeMemberController (Some(mediatorMock.Object)) None
        (controller, mediatorMock)

    let private setupForPostActionTest result id currentPassword username =
        let mediatorMock = Mock<IMediator>()
        let controller =
            match result with
            | NotApplicable -> makeMemberController None None
            | Success ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangeUsernameCommand>()))
                      .Returns(MemberChangeUsernameCommand.successResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | NotFound ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangeUsernameCommand>()))
                      .Returns(MemberChangeUsernameCommand.notFoundResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | IncorrectPassword ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangeUsernameCommand>()))
                      .Returns(MemberChangeUsernameCommand.incorrectPasswordResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | UnavailableUsername ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangeUsernameCommand>()))
                      .Returns(MemberChangeUsernameCommand.unavailableUsernameResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
        let (viewModel, modelState) = bindMemberChangeUsernameViewModel id currentPassword username
        controller.ModelState.Merge(modelState)
        (controller, viewModel, mediatorMock)

    [<Test>]
    let ``Calling change username get action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let (controller, mediatorMock) = setupForGetActionTest (QueryResult.Success(id, String.Empty, username))
        let result = controller.ChangeUsername(id)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberUsernameQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel viewModel id String.Empty username

    [<Test>]
    let ``Calling change username get action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, mediatorMock) = setupForGetActionTest QueryResult.NotFound
        let result = controller.ChangeUsername(notFoundId)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberUsernameQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change username post action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let username = randomString 32
        let (controller, viewModel, mediatorMock) = setupForPostActionTest Success id currentPassword username
        let result = controller.ChangeUsername(id, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangeUsernameCommand>())), Times.Once())
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
    let ``Calling change username post action, with null username, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let nullUsername:string = null
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id currentPassword nullUsername
        let result = controller.ChangeUsername(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel viewModel id String.Empty nullUsername
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is required"

    [<Test>]
    let ``Calling change username post action, with empty username, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let emptyUsername = String.Empty
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id currentPassword emptyUsername
        let result = controller.ChangeUsername(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel viewModel id String.Empty emptyUsername
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is required"

    [<Test>]
    let ``Calling change username post action, with username shorter than 3 characters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let shortUsername = randomString 2
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id currentPassword shortUsername
        let result = controller.ChangeUsername(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel viewModel id String.Empty shortUsername
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is shorter than 3 characters"

    [<Test>]
    let ``Calling change username post action, with username longer than 32 characters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let longUsername = randomString 33
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id currentPassword longUsername
        let result = controller.ChangeUsername(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel viewModel id String.Empty longUsername
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is longer than 32 characters"

    [<Test>]
    let ``Calling change username post action, with username in invalid format, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let invalidFormatUsername = sprintf "-%s-" (randomString 30)
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id currentPassword invalidFormatUsername
        let result = controller.ChangeUsername(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel viewModel id String.Empty invalidFormatUsername
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is not in the correct format (lowercase alphanumeric)"

    [<Test>]
    let ``Calling change username post action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let username = randomString 32
        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, viewModel, mediatorMock) = setupForPostActionTest NotFound notFoundId currentPassword username
        let result = controller.ChangeUsername(notFoundId, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangeUsernameCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change username post action, with incorrect current password, returns expected result`` () =
        let id = Guid.NewGuid()
        let incorrectPassword = randomPassword 32
        let username = randomString 32
        let (controller, viewModel, mediatorMock) =
            setupForPostActionTest IncorrectPassword id incorrectPassword username
        let result = controller.ChangeUsername(id, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangeUsernameCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel viewModel id String.Empty username
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change username post action, with unavailable username, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let unavailableUsername = randomString 32
        let (controller, viewModel, mediatorMock) =
            setupForPostActionTest UnavailableUsername id currentPassword unavailableUsername
        let result = controller.ChangeUsername(id, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangeUsernameCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel viewModel id String.Empty unavailableUsername
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is unavailable"
