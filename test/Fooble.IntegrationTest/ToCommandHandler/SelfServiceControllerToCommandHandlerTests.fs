﻿namespace Fooble.IntegrationTest

open Autofac
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Core.Persistence
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
    let ``Calling register post, with existing username in data store, returns expected result`` () =
        let existingUsername = String.random 32
        let expectedHeading = "Self-Service"
        let expectedSubHeading = "Register"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplay.Severity.warning
        let expectedMessage = "Requested username is unavailable."

        let memberData =
            Seq.singleton (MemberData(Id = Guid.random (), Username = existingUsername, Name = String.random 64))
        let memberSetMock = makeObjectSet memberData
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(contextMock.Object))
        let container = builder.Build()

        let mediator = container.Resolve<IMediator>()

        let keyGeneratorMock = Mock<IKeyGenerator>()
        keyGeneratorMock.SetupFunc(fun x -> x.GenerateKey()).Returns(Guid.random ()).Verifiable()

        let controller = new SelfServiceController(mediator, keyGeneratorMock.Object)
        let result = controller.Register(existingUsername, String.random 64)

        contextMock.Verify()
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

        let controller = new SelfServiceController(mediator, keyGeneratorMock.Object)
        let result = controller.Register(String.random 32, String.random 64);

        contextMock.Verify()
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