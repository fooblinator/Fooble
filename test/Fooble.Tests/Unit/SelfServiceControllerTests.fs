namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open Fooble.Web.Controllers
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System
open System.Linq
open System.Web.Mvc

[<TestFixture>]
module SelfServiceControllerTests =

    [<Test>]
    let ``Constructing, with null mediator, raises expected exception`` () =
        let expectedParamName = "mediator"
        let expectedMessage = "Mediator should not be null"

        raisesWith<ArgumentException> <@ new SelfServiceController(null) @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore <| new SelfServiceController(mock ())

    [<Test>]
    let ``Calling register, returns expected result`` () =
        let expectedViewModel = SelfServiceRegister.ReadModel.empty

        let controller = new SelfServiceController(mock ())
        let result = controller.Register()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterReadModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterReadModel
        test <@ actualViewModel = expectedViewModel @>

    [<Test>]
    let ``Calling register post, with null name, returns expected result`` () =
        let nullName = null
        let expectedViewModel = SelfServiceRegister.ReadModel.make(nullName);

        let controller = new SelfServiceController(mock ())
        let result = controller.Register(nullName)

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterReadModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterReadModel
        test <@ actualViewModel = expectedViewModel @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("name") @>
        test <@ modelState.["name"].Errors.Count = 1 @>
        test <@ modelState.["name"].Errors.[0].ErrorMessage = "Name parameter was null" @>

    [<Test>]
    let ``Calling register post, with empty name, returns expected result`` () =
        let emptyName = String.empty
        let expectedViewModel = SelfServiceRegister.ReadModel.make(emptyName);

        let controller = new SelfServiceController(mock ())
        let result = controller.Register(emptyName)

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterReadModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterReadModel
        test <@ actualViewModel = expectedViewModel @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("name") @>
        test <@ modelState.["name"].Errors.Count = 1 @>
        test <@ modelState.["name"].Errors.[0].ErrorMessage = "Name parameter was an empty string" @>

    [<Test>]
    let ``Calling register post, with existing id in data store, returns expected result`` () =
        let expectedHeading = "Self-Service Register"
        let expectedSeverity = MessageDisplay.Severity.error
        let expectedMessages = [ "Self-service register command was not successful and returned \"duplicate id\"" ]

        let commandResult = SelfServiceRegister.CommandResult.duplicateId
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(any ())).Returns(commandResult).Verifiable()

        let name = randomString ()
        let controller = new SelfServiceController(mediatorMock.Object)
        let result = controller.Register(name)

        mediatorMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "Register_DuplicateId" @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualViewModel = viewResult.Model :?> IMessageDisplayReadModel

        let actualHeading = actualViewModel.Heading
        test <@ actualHeading = expectedHeading @>

        let actualSeverity = actualViewModel.Severity
        test <@ actualSeverity = expectedSeverity @>

        let actualMessages = List.ofSeq actualViewModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling register post, with no existing id in data store, returns expected result`` () =
        let commandResult = SelfServiceRegister.CommandResult.success
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(any ())).Returns(commandResult).Verifiable()

        let name = randomString ()
        let controller = new SelfServiceController(mediatorMock.Object)
        let result = controller.Register(name)

        mediatorMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? RedirectToRouteResult @>

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        test <@ routeValues.ContainsKey("controller") @>
        test <@ routeValues.["controller"].ToString().ToLowerInvariant() = "member" @>

        test <@ routeValues.ContainsKey("action") @>
        test <@ routeValues.["action"].ToString().ToLowerInvariant() = "detail" @>
