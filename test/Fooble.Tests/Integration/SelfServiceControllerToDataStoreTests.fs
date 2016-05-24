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
module SelfServiceControllerToDataStoreTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(ConnectionString = connectionString))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<IKeyGenerator>()
        ignore <| new SelfServiceController(mediator, keyGenerator)

    [<Test>]
    let ``Calling register post, with no existing id in data store, returns expected result`` () =
        let expectedId = randomGuid ()

        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = context))
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
