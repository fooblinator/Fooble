﻿namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Presentation
open Fooble.Presentation.Infrastructure
open Fooble.Persistence
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
        ignore <| builder.RegisterModule(CoreRegistrations())
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<IKeyGenerator>()
        ignore <| new SelfServiceController(mediator, keyGenerator)

    [<Test>]
    let ``Calling register post, with existing username in data store, returns expected result`` () =
        let existingUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(any ())).Returns(true).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ()))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<IKeyGenerator>()
        let controller = new SelfServiceController(mediator, keyGenerator)
        let result =
            SelfServiceRegisterViewModel.make existingUsername expectedEmail expectedNickname
            |> controller.Register

        contextMock.Verify()

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

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(any ())).Returns(true).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ()))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<IKeyGenerator>()
        let controller = new SelfServiceController(mediator, keyGenerator)
        let result =
            SelfServiceRegisterViewModel.make expectedUsername existingEmail expectedNickname
            |> controller.Register

        contextMock.Verify()

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

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(any ())).Returns(false).Verifiable()
        contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(any ())).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ()))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = makeTestKeyGenerator (Some expectedId)
        let controller = new SelfServiceController(mediator, keyGenerator)
        let result =
            SelfServiceRegisterViewModel.make (String.random 32) (EmailAddress.random ()) (String.random 64)
            |> controller.Register

        contextMock.Verify()

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
