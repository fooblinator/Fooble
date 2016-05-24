namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open Fooble.Web.Controllers
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
        raisesWith<ArgumentException> <@ new SelfServiceController(null, keyGenerator) @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let keyGenerator = KeyGenerator.make ()
        ignore <| new SelfServiceController(mock (), keyGenerator)

    [<Test>]
    let ``Calling register, returns expected result`` () =
        let expectedViewModel = SelfServiceRegister.ViewModel.empty

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel = expectedViewModel @>

    [<Test>]
    let ``Calling register post, with null name, returns expected result`` () =
        let nullName = null
        let expectedViewModel = SelfServiceRegister.ViewModel.make(nullName);

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(nullName)

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel = expectedViewModel @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("name") @>
        test <@ modelState.["name"].Errors.Count = 1 @>
        test <@ modelState.["name"].Errors.[0].ErrorMessage = "Name parameter was null" @>

    [<Test>]
    let ``Calling register post, with empty name, returns expected result`` () =
        let emptyName = String.empty
        let expectedViewModel = SelfServiceRegister.ViewModel.make(emptyName);

        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(emptyName)

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? ISelfServiceRegisterViewModel @>

        let actualViewModel = viewResult.Model :?> ISelfServiceRegisterViewModel
        test <@ actualViewModel = expectedViewModel @>

        let modelState = viewResult.ViewData.ModelState

        test <@ modelState.ContainsKey("name") @>
        test <@ modelState.["name"].Errors.Count = 1 @>
        test <@ modelState.["name"].Errors.[0].ErrorMessage = "Name parameter was an empty string" @>

    [<Test>]
    let ``Calling register post, with valid name, completes without exception`` () =
        let name = randomString ()
        let keyGenerator = KeyGenerator.make ()
        let controller = new SelfServiceController(mock (), keyGenerator)
        let result = controller.Register(name)

        test <@ notIsNull result @>
        test <@ result :? RedirectToRouteResult @>

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        test <@ routeValues.ContainsKey("controller") @>
        test <@ routeValues.["controller"].ToString().ToLowerInvariant() = "member" @>

        test <@ routeValues.ContainsKey("action") @>
        test <@ routeValues.["action"].ToString().ToLowerInvariant() = "detail" @>
