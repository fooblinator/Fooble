namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
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

        let keyGenerator = makeTestKeyGenerator None
        raisesWith<ArgumentException> <@ new SelfServiceController(null, keyGenerator) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

//    [<Test>]
//    let ``Constructing, with null key generator, raises expected exception`` () =
//        let expectedParamName = "keyGenerator"
//        let expectedMessage = "Key generator is required"
//
//        raisesWith<ArgumentException> <@ new SelfServiceController(mock (), null) @>
//            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let keyGenerator = makeTestKeyGenerator None
        ignore <| new SelfServiceController(mock (), keyGenerator)

    [<Test>]
    let ``Calling register, returns expected result`` () =
        let emptyUsername = String.empty
        let emptyEmail = String.empty
        let emptyNickname = String.empty

        let keyGenerator = makeTestKeyGenerator None
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
        test <@ actualViewModel.Email = emptyEmail @>
        test <@ actualViewModel.Nickname = emptyNickname @>

    [<Test>]
    let ``Calling register post, with null username, returns expected result`` () =
        let nullUsername:string = null
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is required")
        let result =
            SelfServiceRegisterViewModel.make nullUsername expectedEmail expectedNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = nullUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is required" @>

    [<Test>]
    let ``Calling register post, with empty username, returns expected result`` () =
        let emptyUsername = String.empty
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is required")
        let result =
            SelfServiceRegisterViewModel.make emptyUsername expectedEmail expectedNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = emptyUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is required" @>

    [<Test>]
    let ``Calling register post, with username shorter than 3 characters, returns expected result`` () =
        let shortUsername = String.random 2
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is shorter than 3 characters")
        let result =
            SelfServiceRegisterViewModel.make shortUsername expectedEmail expectedNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = shortUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is shorter than 3 characters" @>

    [<Test>]
    let ``Calling register post, with username longer than 32 characters, returns expected result`` () =
        let longUsername = String.random 33
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username", "Username is longer than 32 characters")
        let result =
            SelfServiceRegisterViewModel.make longUsername expectedEmail expectedNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = longUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is longer than 32 characters" @>

    [<Test>]
    let ``Calling register post, with username in invalid format, returns expected result`` () =
        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("username",
            "Username is not in the correct format (lowercase alphanumeric)")
        let result =
            SelfServiceRegisterViewModel.make invalidFormatUsername expectedEmail expectedNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = invalidFormatUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage =
            "Username is not in the correct format (lowercase alphanumeric)" @>

    [<Test>]
    let ``Calling register post, with null email, returns expected result`` () =
        let expectedUsername = String.random 32
        let nullEmail:string = null
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is required")
        let result =
            SelfServiceRegisterViewModel.make expectedUsername nullEmail expectedNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = nullEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("email") @>
        test <@ modelState.["email"].Errors.Count = 1 @>
        test <@ modelState.["email"].Errors.[0].ErrorMessage = "Email is required" @>

    [<Test>]
    let ``Calling register post, with empty email, returns expected result`` () =
        let expectedUsername = String.random 32
        let emptyEmail = String.empty
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is required")
        let result =
            SelfServiceRegisterViewModel.make expectedUsername emptyEmail expectedNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = emptyEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("email") @>
        test <@ modelState.["email"].Errors.Count = 1 @>
        test <@ modelState.["email"].Errors.[0].ErrorMessage = "Email is required" @>

    [<Test>]
    let ``Calling register post, with email longer than 254 characters, returns expected result`` () =
        let expectedUsername = String.random 32
        let longEmail = String.random 255
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is longer than 254 characters")
        let result =
            SelfServiceRegisterViewModel.make expectedUsername longEmail expectedNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = longEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("email") @>
        test <@ modelState.["email"].Errors.Count = 1 @>
        test <@ modelState.["email"].Errors.[0].ErrorMessage = "Email is longer than 254 characters" @>

    [<Test>]
    let ``Calling register post, with email in invalid format, returns expected result`` () =
        let expectedUsername = String.random 32
        let invalidFormatEmail = String.random 64
        let expectedNickname = String.random 64

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("email", "Email is not in the correct format")
        let result =
            SelfServiceRegisterViewModel.make expectedUsername invalidFormatEmail expectedNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = invalidFormatEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("email") @>
        test <@ modelState.["email"].Errors.Count = 1 @>
        test <@ modelState.["email"].Errors.[0].ErrorMessage = "Email is not in the correct format" @>

    [<Test>]
    let ``Calling register post, with null nickname, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let nullNickname:string = null

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is required")
        let result =
            SelfServiceRegisterViewModel.make expectedUsername expectedEmail nullNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = nullNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("nickname") @>
        test <@ modelState.["nickname"].Errors.Count = 1 @>
        test <@ modelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is required" @>

    [<Test>]
    let ``Calling register post, with empty nickname, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let emptyNickname = String.empty

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is required")
        let result =
            SelfServiceRegisterViewModel.make expectedUsername expectedEmail emptyNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = emptyNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("nickname") @>
        test <@ modelState.["nickname"].Errors.Count = 1 @>
        test <@ modelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is required" @>

    [<Test>]
    let ``Calling register post, with nickname longer than 64 characters, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let longNickname = String.random 65

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mock (), keyGenerator)
        controller.ModelState.AddModelError("nickname", "Nickname is longer than 64 characters")
        let result =
            SelfServiceRegisterViewModel.make expectedUsername expectedEmail longNickname
            |> controller.Register

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = longNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("nickname") @>
        test <@ modelState.["nickname"].Errors.Count = 1 @>
        test <@ modelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is longer than 64 characters" @>

    [<Test>]
    let ``Calling register post, with existing username in data store, returns expected result`` () =
        let existingUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mediatorMock.Object, keyGenerator)
        let result =
            SelfServiceRegisterViewModel.make existingUsername expectedEmail expectedNickname
            |> controller.Register

        mediatorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = existingUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is unavailable" @>

    [<Test>]
    let ``Calling register post, with existing email in data store, returns expected result`` () =
        let expectedUsername = String.random 32
        let existingEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new SelfServiceController(mediatorMock.Object, keyGenerator)
        let result =
            SelfServiceRegisterViewModel.make expectedUsername existingEmail expectedNickname
            |> controller.Register

        mediatorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = existingEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("email") @>
        test <@ modelState.["email"].Errors.Count = 1 @>
        test <@ modelState.["email"].Errors.[0].ErrorMessage = "Email is already registered" @>

    [<Test>]
    let ``Calling register post, with no existing username or email in data store, returns expected result`` () =
        let expectedId = Guid.random ()

        let commandResult = SelfServiceRegisterCommand.successResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(commandResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator (Some expectedId)
        let controller = new SelfServiceController(mediatorMock.Object, keyGenerator)
        let result =
            SelfServiceRegisterViewModel.make (String.random 32) (EmailAddress.random ()) (String.random 64)
            |> controller.Register

        mediatorMock.Verify()

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
