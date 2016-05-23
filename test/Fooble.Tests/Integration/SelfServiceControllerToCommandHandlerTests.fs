namespace Fooble.Tests.Integration

open Autofac
open Fooble.Core.Infrastructure
open Fooble.Web.Controllers
open MediatR
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceControllerToCommandHandlerTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = mock ()))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        ignore <| new SelfServiceController(mediator)

    [<Test>]
    let ``Calling register post, with existing id in data store, returns expected result`` () =
        // TODO: once key generator service is created and used by self-service controller, use that to mock and capture generated id
        ()

    [<Test>]
    let ``Calling register post, with no existing id in data store, returns expected result`` () =
        // TODO: once key generator service is created and used by self-service controller, use that to mock and capture generated id
        ()