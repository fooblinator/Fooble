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
        let emptyNickname = String.empty

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
        test <@ actualViewModel.Nickname = emptyNickname @>

    [<Test>]
    let ``Calling register post, with null username, returns expected result`` () =
        let nullUsername:string = null
        let expectedNickname = String.random 64

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(nullUsername, expectedNickname)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = nullUsername @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is required" @>

    [<Test>]
    let ``Calling register post, with empty username, returns expected result`` () =
        let emptyUsername = String.empty
        let expectedNickname = String.random 64

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(emptyUsername, expectedNickname)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = emptyUsername @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is required" @>

    [<Test>]
    let ``Calling register post, with username shorter than 3 characters, returns expected result`` () =
        let shortUsername = String.random 2
        let expectedNickname = String.random 64

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(shortUsername, expectedNickname)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = shortUsername @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is shorter than 3 characters" @>

    [<Test>]
    let ``Calling register post, with username longer than 32 characters, returns expected result`` () =
        let longUsername = String.random 33
        let expectedNickname = String.random 64

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(longUsername, expectedNickname)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = longUsername @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is longer than 32 characters" @>

    [<Test>]
    let ``Calling register post, with username in invalid format, returns expected result`` () =
        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        let expectedNickname = String.random 64

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(invalidFormatUsername, expectedNickname)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = invalidFormatUsername @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage =
            "Username is not in the correct format (lowercase alphanumeric)" @>

    [<Test>]
    let ``Calling register post, with null nickname, returns expected result`` () =
        let expectedUsername = String.random 32
        let nullNickname:string = null

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(expectedUsername, nullNickname)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Nickname = nullNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("nickname") @>
        test <@ modelState.["nickname"].Errors.Count = 1 @>
        test <@ modelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is required" @>

    [<Test>]
    let ``Calling register post, with empty nickname, returns expected result`` () =
        let expectedUsername = String.random 32
        let emptyNickname = String.empty

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(expectedUsername, emptyNickname)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Nickname = emptyNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("nickname") @>
        test <@ modelState.["nickname"].Errors.Count = 1 @>
        test <@ modelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is required" @>

    [<Test>]
    let ``Calling register post, with nickname longer than 64 characters, returns expected result`` () =
        let expectedUsername = String.random 32
        let longNickname = String.random 65

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(expectedUsername, longNickname)

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Nickname = longNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("nickname") @>
        test <@ modelState.["nickname"].Errors.Count = 1 @>
        test <@ modelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is longer than 64 characters" @>

    [<Test>]
    let ``Calling register post, with existing username in data store, returns expected result`` () =
        let existingUsername = String.random 32
        let expectedNickname = String.random 64

        let commandResult = SelfServiceRegister.CommandResult.usernameUnavailable
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mediatorMock.Object, keyGenerator)
        let result = controller.Register(existingUsername, expectedNickname)

        mediatorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = existingUsername @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is unavailable" @>

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
