namespace Fooble.Tests.Integration

open Autofac
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Core.Persistence
open Fooble.Tests
open Fooble.Web.Controllers
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module SelfServiceControllerToCommandHandlerTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let context = Mock.Of<IFoobleContext>()
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(context))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<IKeyGenerator>()
        ignore <| new SelfServiceController(mediator, keyGenerator)

    [<Test>]
    let ``Calling register post, returns expected result`` () =
        let expectedId = randomGuid ()

        let memberSetMock = makeObjectSet Seq.empty<MemberData>
        memberSetMock.SetupAction(fun x -> x.AddObject(any ())).Verifiable()
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(contextMock.Object))
        let container = builder.Build()

        let mediator = container.Resolve<IMediator>()

        let keyGeneratorMock = Mock<IKeyGenerator>()
        keyGeneratorMock.SetupFunc(fun x -> x.GenerateKey()).Returns(expectedId).Verifiable()

        let name = randomString ()
        let controller = new SelfServiceController(mediator, keyGeneratorMock.Object)
        let result = controller.Register(name);

        keyGeneratorMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? RedirectToRouteResult @>

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        test <@ routeValues.ContainsKey("controller") @>
        test <@ routeValues.["controller"].ToString().ToLowerInvariant() = "member" @>

        test <@ routeValues.ContainsKey("action") @>
        test <@ routeValues.["action"].ToString().ToLowerInvariant() = "detail" @>

        let expectedIdString = expectedId.ToString()

        test <@ routeValues.ContainsKey("id") @>
        test <@ routeValues.["id"].ToString().ToLowerInvariant() = expectedIdString @>
