namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open Fooble.Web.Controllers
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
        let expectedMessage = "Mediator parameter was null"

        let keyGenerator = KeyGenerator.make ()
        raisesWith<ArgumentException> <@ new SelfServiceController(null, keyGenerator) @> (fun x ->
            <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

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
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username parameter was null" @>

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
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username parameter was an empty string" @>

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
        test <@ modelState.["name"].Errors.[0].ErrorMessage = "Name parameter was null" @>

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
        test <@ modelState.["name"].Errors.[0].ErrorMessage = "Name parameter was an empty string" @>

    [<Test>]
    let ``Calling register post, with valid parameters, completes without exception`` () =
        let expectedId = Guid.random ()

        let keyGeneratorMock = Mock<IKeyGenerator>()
        keyGeneratorMock.SetupFunc(fun x -> x.GenerateKey()).Returns(expectedId).Verifiable()

        let controller = new SelfServiceController(mock (), keyGeneratorMock.Object)
        let result = controller.Register(String.random 32, String.random 64)

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
