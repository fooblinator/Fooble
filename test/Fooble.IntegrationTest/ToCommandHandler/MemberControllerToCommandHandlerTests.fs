namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Persistence
open Fooble.Presentation
open Fooble.Presentation.Infrastructure
open Fooble.Web.Controllers
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module MemberControllerToCommandHandlerTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        ignore (new MemberController(mediator, keyGenerator))

    [<Test>]
    let ``Calling register post, with existing username in data store, returns expected result`` () =
        let existingUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(any ())).Returns(true).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let viewModel =
            bindMemberRegisterViewModel existingUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

        contextMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel existingUsername String.empty String.empty expectedEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is unavailable"

    [<Test>]
    let ``Calling register post, with existing email in data store, returns expected result`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let existingEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(any ())).Returns(true).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let viewModel =
            bindMemberRegisterViewModel expectedUsername expectedPassword expectedConfirmPassword existingEmail
                expectedNickname
        let result = controller.Register viewModel

        contextMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel expectedUsername String.empty String.empty existingEmail
            expectedNickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is already registered"

    [<Test>]
    let ``Calling register post, with no existing username or email in data store, returns expected result`` () =
        let expectedId = Guid.random ()

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(any ())).Returns(false).Verifiable()
        contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(any ())).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = makeTestKeyGenerator (Some expectedId)
        use controller = new MemberController(mediator, keyGenerator)

        let password = Password.random 32
        let viewModel =
            bindMemberRegisterViewModel (String.random 32) password password (EmailAddress.random 32)
                (String.random 64)
        let result = controller.Register viewModel

        contextMock.Verify()

        isNull result =! false
        result :? RedirectToRouteResult =! true

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "member"

        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "detail"

        let expectedIdString = String.ofGuid expectedId

        routeValues.ContainsKey("id") =! true
        routeValues.["id"].ToString().ToLowerInvariant() =! expectedIdString
