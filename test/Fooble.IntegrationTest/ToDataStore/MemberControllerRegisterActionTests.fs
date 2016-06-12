namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open Fooble.Presentation
open Fooble.Presentation.Infrastructure
open Fooble.Web.Controllers
open MediatR
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module MemberControllerRegisterActionTests =

    [<Test>]
    let ``Calling register, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()

        use controller = new MemberController(mediator, keyGenerator)
        let result = controller.Register()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel String.empty String.empty String.empty String.empty String.empty

    [<Test>]
    let ``Calling register post, with unavailable username, returns expected result`` () =
        let unavailableUsername = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(considerDeactivated = true))

        // add matching member to the data store
        let passwordData = Crypto.hash password 100
        let memberData =
            memberDataFactory.Invoke(Guid.random (), unavailableUsername, passwordData, EmailAddress.random 32,
                String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberRegisterViewModel unavailableUsername password confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel unavailableUsername String.empty String.empty email nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "username" "Username is unavailable"

    [<Test>]
    let ``Calling register post, with unavailable email, returns expected result`` () =
        let username = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let unavailableEmail = EmailAddress.random 32
        let nickname = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(considerDeactivated = true))

        // add matching member to the data store
        let passwordData = Crypto.hash password 100
        let memberData =
            memberDataFactory.Invoke(Guid.random (), String.random 32, passwordData, unavailableEmail,
                String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberRegisterViewModel username password confirmPassword unavailableEmail nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel actualViewModel username String.empty String.empty unavailableEmail nickname

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "email" "Email is already registered"

    [<Test>]
    let ``Calling register post, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let username = String.random 32
        let password = Password.random 32
        let confirmPassword = password
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let mediator = container.Resolve<IMediator>()
        let keyGenerator = makeTestKeyGenerator (Some(id))

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(considerDeactivated = true))

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberRegisterViewModel username password confirmPassword email nickname
        let result = controller.Register viewModel

        isNull result =! false
        result :? RedirectToRouteResult =! true

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "member"

        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "detail"

        let idString = String.ofGuid id

        routeValues.ContainsKey("id") =! true
        routeValues.["id"].ToString().ToLowerInvariant() =! idString
