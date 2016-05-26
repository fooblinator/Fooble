namespace Fooble.UnitTest.SelfService

open Fooble.Core
open Fooble.UnitTest
open Fooble.Web.Controllers
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module SelfServiceControllerTests =

    [<Test>]
    let ``Constructing, with null mediator, raises expected exception`` () =
        let expectedParamName = "mediator"
        let expectedMessage = "Mediator is required"

        let keyGenerator = KeyGenerator.make ()
        raisesWith<ArgumentException> <@ new SelfServiceController(null, keyGenerator) @> (fun x ->
            <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

//    [<Test>]
//    let ``Constructing, with null key generator, raises expected exception`` () =
//        let expectedParamName = "keyGenerator"
//        let expectedMessage = "Key generator is required"
//
//        raisesWith<ArgumentException> <@ new SelfServiceController(mock (), null) @> (fun x ->
//            <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let keyGenerator = KeyGenerator.make ()
        ignore <| new SelfServiceController(mock (), keyGenerator)

    [<Test>]
    let ``Calling register, returns expected result`` () =
        let emptyUsername = String.empty
        let emptyName = String.empty

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = emptyUsername @>
        test <@ actualViewModel.Name = emptyName @>

    [<Test>]
    let ``Calling register post, with null username, returns expected result`` () =
        let nullUsername:string = null
        let expectedName = String.random 64

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(nullUsername, expectedName)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = nullUsername @>
        test <@ actualViewModel.Name = expectedName @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is required" @>

    [<Test>]
    let ``Calling register post, with empty username, returns expected result`` () =
        let emptyUsername = String.empty
        let expectedName = String.random 64

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(emptyUsername, expectedName)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = emptyUsername @>
        test <@ actualViewModel.Name = expectedName @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is required" @>

    [<Test>]
    let ``Calling register post, with username shorter than 3 characters, returns expected result`` () =
        let shortUsername = String.random 2
        let expectedName = String.random 64

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(shortUsername, expectedName)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = shortUsername @>
        test <@ actualViewModel.Name = expectedName @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is shorter than 3 characters" @>

    [<Test>]
    let ``Calling register post, with username longer than 32 characters, returns expected result`` () =
        let longUsername = String.random 33
        let expectedName = String.random 64

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(longUsername, expectedName)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = longUsername @>
        test <@ actualViewModel.Name = expectedName @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is longer than 32 characters" @>

    [<Test>]
    let ``Calling register post, with username in invalid format, returns expected result`` () =
        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        let expectedName = String.random 64

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(invalidFormatUsername, expectedName)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = invalidFormatUsername @>
        test <@ actualViewModel.Name = expectedName @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage =
            "Username is not in the correct format (lowercase alphanumeric)" @>

    [<Test>]
    let ``Calling register post, with null name, returns expected result`` () =
        let expectedUsername = String.random 32
        let nullName:string = null

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(expectedUsername, nullName)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Name = nullName @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("name") @>
        test <@ modelState.["name"].Errors.Count = 1 @>
        test <@ modelState.["name"].Errors.[0].ErrorMessage = "Name is required" @>

    [<Test>]
    let ``Calling register post, with empty name, returns expected result`` () =
        let expectedUsername = String.random 32
        let emptyName = String.empty

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(expectedUsername, emptyName)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Name = emptyName @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("name") @>
        test <@ modelState.["name"].Errors.Count = 1 @>
        test <@ modelState.["name"].Errors.[0].ErrorMessage = "Name is required" @>

    [<Test>]
    let ``Calling register post, with existing username in data store, returns expected result`` () =
        let expectedHeading = "Self-Service"
        let expectedSubHeading = "Register"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplay.Severity.warning
        let expectedMessage = "Requested username is unavailable."

        let commandResult = SelfServiceRegister.CommandResult.usernameUnavailable
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGeneratorMock = Mock<IKeyGenerator>()
        keyGeneratorMock.SetupFunc(fun x -> x.GenerateKey()).Returns(Guid.random ()).Verifiable()

        let controller = new SelfServiceController(mediatorMock.Object, keyGeneratorMock.Object)
        let result = controller.Register(String.random 32, String.random 64)

        mediatorMock.Verify()
        keyGeneratorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "MessageDisplay" @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        test <@ actualReadModel.Heading = expectedHeading @>
        test <@ actualReadModel.SubHeading = expectedSubHeading @>
        test <@ actualReadModel.StatusCode = expectedStatusCode @>
        test <@ actualReadModel.Severity = expectedSeverity @>
        test <@ actualReadModel.Message = expectedMessage @>

    [<Test>]
    let ``Calling register post, with no existing username in data store, returns expected result`` () =
        let expectedId = Guid.random ()

        let commandResult = SelfServiceRegister.CommandResult.success
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGeneratorMock = Mock<IKeyGenerator>()
        keyGeneratorMock.SetupFunc(fun x -> x.GenerateKey()).Returns(expectedId).Verifiable()

        let controller = new SelfServiceController(mediatorMock.Object, keyGeneratorMock.Object)
        let result = controller.Register(String.random 32, String.random 64)

        mediatorMock.Verify()
        keyGeneratorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? RedirectToRouteResult @>

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        test <@ routeValues.ContainsKey("controller") @>
        test <@ routeValues.["controller"].ToString().ToLowerInvariant() = "member" @>

        test <@ routeValues.ContainsKey("action") @>
        test <@ routeValues.["action"].ToString().ToLowerInvariant() = "detail" @>

        let expectedIdString = String.ofGuid expectedId

        test <@ routeValues.ContainsKey("id") @>
        test <@ routeValues.["id"].ToString().ToLowerInvariant() = expectedIdString @>
