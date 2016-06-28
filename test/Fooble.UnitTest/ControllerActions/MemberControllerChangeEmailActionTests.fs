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
module MemberControllerChangeEmailActionTests =

    type private QueryResult =
        | Success of Guid * string * string
        | NotFound

    type private CommandResult = NotApplicable | Success | NotFound | IncorrectPassword | UnavailableEmail

    let private setupForGetActionTest result =
        let result =
            match result with
            | QueryResult.Success(id, currentPassword, email) ->
                  MemberChangeEmailViewModel.make id currentPassword email
                  |> MemberEmailQuery.makeSuccessResult
            | QueryResult.NotFound -> MemberEmailQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberEmailQuery>())).Returns(result).End
        let controller = makeMemberController (Some(mediatorMock.Object)) None
        (controller, mediatorMock)

    let private setupForPostActionTest result id currentPassword email =
        let mediatorMock = Mock<IMediator>()
        let controller =
            match result with
            | NotApplicable -> makeMemberController None None
            | Success ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangeEmailCommand>()))
                      .Returns(MemberChangeEmailCommand.successResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | NotFound ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangeEmailCommand>()))
                      .Returns(MemberChangeEmailCommand.notFoundResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | IncorrectPassword ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangeEmailCommand>()))
                      .Returns(MemberChangeEmailCommand.incorrectPasswordResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
            | UnavailableEmail ->
                  mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberChangeEmailCommand>()))
                      .Returns(MemberChangeEmailCommand.unavailableEmailResult).End
                  makeMemberController (Some(mediatorMock.Object)) None
        let (viewModel, modelState) = bindMemberChangeEmailViewModel id currentPassword email
        controller.ModelState.Merge(modelState)
        (controller, viewModel, mediatorMock)

    [<Test>]
    let ``Calling change email get action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let email = randomEmail 32
        let (controller, mediatorMock) = setupForGetActionTest (QueryResult.Success(id, String.Empty, email))
        let result = controller.ChangeEmail(id)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberEmailQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel viewModel id String.Empty email

    [<Test>]
    let ``Calling change email get action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let heading = "Member"
        let subHeading = "Change Email"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, mediatorMock) = setupForGetActionTest QueryResult.NotFound
        let result = controller.ChangeEmail(notFoundId)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberEmailQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change email post action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let email = randomEmail 32
        let (controller, viewModel, mediatorMock) = setupForPostActionTest Success id currentPassword email
        let result = controller.ChangeEmail(id, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangeEmailCommand>())), Times.Once())
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
    let ``Calling change email post action, with null email, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let nullEmail:string = null
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id currentPassword nullEmail
        let result = controller.ChangeEmail(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel viewModel id String.Empty nullEmail
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is required"

    [<Test>]
    let ``Calling change email post action, with empty email, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let emptyEmail = String.Empty
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id currentPassword emptyEmail
        let result = controller.ChangeEmail(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel viewModel id String.Empty emptyEmail
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is required"

    [<Test>]
    let ``Calling change email post action, with email longer than 256 characters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let longEmail = randomEmail 257
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id currentPassword longEmail
        let result = controller.ChangeEmail(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel viewModel id String.Empty longEmail
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is longer than 256 characters"

    [<Test>]
    let ``Calling change email post action, with email in invalid format, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let invalidFormatEmail = randomString 32
        let (controller, viewModel, _) = setupForPostActionTest NotApplicable id currentPassword invalidFormatEmail
        let result = controller.ChangeEmail(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel viewModel id String.Empty invalidFormatEmail
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is not in the correct format"

    [<Test>]
    let ``Calling change email post action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let email = randomEmail 32
        let heading = "Member"
        let subHeading = "Change Email"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, viewModel, mediatorMock) = setupForPostActionTest NotFound notFoundId currentPassword email
        let result = controller.ChangeEmail(notFoundId, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangeEmailCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change email post action, with incorrect current password, returns expected result`` () =
        let id = Guid.NewGuid()
        let incorrectPassword = randomPassword 32
        let email = randomEmail 32
        let (controller, viewModel, mediatorMock) =
            setupForPostActionTest IncorrectPassword id incorrectPassword email
        let result = controller.ChangeEmail(id, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangeEmailCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel viewModel id String.Empty email
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change email post action, with unavailable email, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let unavailableEmail = randomEmail 32
        let (controller, viewModel, mediatorMock) =
            setupForPostActionTest UnavailableEmail id currentPassword unavailableEmail
        let result = controller.ChangeEmail(id, viewModel)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberChangeEmailCommand>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel viewModel id String.Empty unavailableEmail
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is already registered"
