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
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = mock ()))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<IKeyGenerator>()
        ignore <| new SelfServiceController(mediator, keyGenerator)

    [<Test>]
    let ``Calling register post, with existing id in data store, returns expected result`` () =
        let existingId = randomGuid ()
        let expectedHeading = "Self-Service Register"
        let expectedSeverity = MessageDisplay.Severity.error
        let expectedMessages = [ "Self-service register command was not successful and returned \"duplicate id\"" ]

        let memberData = MemberData(Id = existingId, Name = randomString ())
        let memberSetMock = makeObjectSet <| Seq.singleton memberData
        memberSetMock.SetupAction(fun x -> x.AddObject(any ())).Verifiable()
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = contextMock.Object))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()

        let keyGeneratorMock = Mock<IKeyGenerator>()
        keyGeneratorMock.SetupFunc(fun x -> x.GenerateKey()).Returns(existingId).Verifiable()

        let name = randomString ()
        let controller = new SelfServiceController(mediator, keyGeneratorMock.Object)
        let result = controller.Register(name);

        keyGeneratorMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "Register_DuplicateId" @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualViewModel = viewResult.Model :?> IMessageDisplayReadModel

        let actualHeading = actualViewModel.Heading
        test <@ actualHeading = expectedHeading @>

        let actualSeverity = actualViewModel.Severity
        test <@ actualSeverity = expectedSeverity @>

        let actualMessages = List.ofSeq actualViewModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling register post, with no existing id in data store, returns expected result`` () =
        let expectedId = randomGuid ()

        let memberSetMock = makeObjectSet Seq.empty<MemberData>
        memberSetMock.SetupAction(fun x -> x.AddObject(any ())).Verifiable()
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = contextMock.Object))
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
