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
