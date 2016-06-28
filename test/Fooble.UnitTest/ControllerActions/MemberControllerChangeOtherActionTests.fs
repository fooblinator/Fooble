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
module MemberControllerChangeOtherActionTests =

    type private QueryResult =
        | Success of Guid * string * string
        | NotFound

    type private CommandResult = NotApplicable | Success | NotFound

    let private setupForGetActionTest result =
        let result =
            match result with
            | QueryResult.Success(id, nickname, avatarData) ->
                  MemberChangeOtherViewModel.make id nickname avatarData
                  |> MemberOtherQuery.makeSuccessResult
            | QueryResult.NotFound -> MemberOtherQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberOtherQuery>())).Returns(result).End
        let controller = makeMemberController (Some(mediatorMock.Object)) None
        (controller, mediatorMock)

    let private setupForPostActionTest result id nickname avatarData =
        let mediatorMock = Mock<IMediator>()
        let controller =
            match result with
            | NotApplicable -> makeMemberController None None
            | Success ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangeOtherCommand>()))
                      .Returns(MemberChangeOtherCommand.successResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | NotFound ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangeOtherCommand>()))
                      .Returns(MemberChangeOtherCommand.notFoundResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
        let (viewModel, modelState) = bindMemberChangeOtherViewModel id nickname avatarData
        controller.ModelState.Merge(modelState)
        (controller, viewModel, mediatorMock)

    [<Test>]
    let ``Calling change other get action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, mediatorMock) = setupForGetActionTest (QueryResult.Success(id, nickname, avatarData))
        let result = controller.ChangeOther(id)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberOtherQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel viewModel id nickname avatarData

    [<Test>]
    let ``Calling change other get action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let heading = "Member"
        let subHeading = "Change Other"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, mediatorMock) = setupForGetActionTest QueryResult.NotFound
        let result = controller.ChangeOther(notFoundId)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberOtherQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change other post action, with successful parameters, and default submit, returns expected result`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, mediatorMock) = setupForPostActionTest Success id nickname avatarData
        let result = controller.ChangeOther(id, viewModel, "default")
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangeOtherCommand>())), Times.Once())
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
    let ``Calling change other post action, with successful parameters, and random submit, returns expected result`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id nickname avatarData
        let result = controller.ChangeOther(id, viewModel, "random")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        viewModel.AvatarData <>! avatarData
        testMemberChangeOtherViewModel viewModel id nickname viewModel.AvatarData

    [<Test>]
    let ``Calling change other post action, with null nickname, returns expected result`` () =
        let id = Guid.NewGuid()
        let nullNickname:string = null
        let avatarData = randomString 32
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id nullNickname avatarData
        let result = controller.ChangeOther(id, viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel viewModel id nullNickname avatarData
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "nickname" "Nickname is required"

    [<Test>]
    let ``Calling change other post action, with empty nickname, returns expected result`` () =
        let id = Guid.NewGuid()
        let emptyNickname = String.Empty
        let avatarData = randomString 32
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id emptyNickname avatarData
        let result = controller.ChangeOther(id, viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel viewModel id emptyNickname avatarData
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "nickname" "Nickname is required"

    [<Test>]
    let ``Calling change other post action, with nickname longer than 64 characters, returns expected result`` () =
        let id = Guid.NewGuid()
        let longNickname = randomString 65
        let avatarData = randomString 32
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id longNickname avatarData
        let result = controller.ChangeOther(id, viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel viewModel id longNickname avatarData
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "nickname" "Nickname is longer than 64 characters"

    [<Test>]
    let ``Calling change other post action, with null avatar data, returns expected result`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 32
        let nullAvatarData:string = null
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id nickname nullAvatarData
        let result = controller.ChangeOther(id, viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel viewModel id nickname nullAvatarData
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "avatarData" "Avatar data is required"

    [<Test>]
    let ``Calling change other post action, with empty avatar data, returns expected result`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 32
        let emptyAvatarData = String.Empty
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id nickname emptyAvatarData
        let result = controller.ChangeOther(id, viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel viewModel id nickname emptyAvatarData
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "avatarData" "Avatar data is required"

    [<Test>]
    let ``Calling change other post action, with avatar data longer than 32 characters, returns expected result`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 32
        let longAvatarData = randomString 33
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id nickname longAvatarData
        let result = controller.ChangeOther(id, viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel viewModel id nickname longAvatarData
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "avatarData" "Avatar data is longer than 32 characters"

    [<Test>]
    let ``Calling change other post action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let nickname = randomString 32
        let avatarData = randomString 32
        let heading = "Member"
        let subHeading = "Change Other"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, viewModel, mediatorMock) = setupForPostActionTest NotFound notFoundId nickname avatarData
        let result = controller.ChangeOther(notFoundId, viewModel, "default")
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangeOtherCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message
