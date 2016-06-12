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
module MemberControllerChangeUsernameActionTests =

    [<Test>]
    let ``Calling change username, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(considerDeactivated = true))

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)
        let result = controller.ChangeUsername(notFoundId)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change username, with successful parameters, returns expected result`` () =
        let id = Guid.random ()

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
        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            memberDataFactory.Invoke(id, String.random 32, passwordData, EmailAddress.random 32, String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)
        let result = controller.ChangeUsername(id)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel id String.empty String.empty

    [<Test>]
    let ``Calling change username post, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let currentPassword = Password.random 32
        let newUsername = String.random 32

        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(considerDeactivated = true))

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberChangeUsernameViewModel notFoundId currentPassword newUsername
        let result = controller.ChangeUsername(notFoundId, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change username post, with incorrect current password, returns expected result`` () =
        let id = Guid.random ()
        let incorrectPassword = Password.random 32
        let newUsername = String.random 32

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
        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            memberDataFactory.Invoke(id, String.random 32, passwordData, EmailAddress.random 32, String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberChangeUsernameViewModel id incorrectPassword newUsername
        let result = controller.ChangeUsername(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel id String.empty newUsername

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change username post, with unavailable username, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let unavailableUsername = String.random 32

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
        let passwordData = Crypto.hash currentPassword 100
        let memberData =
            memberDataFactory.Invoke(id, unavailableUsername, passwordData, EmailAddress.random 32, String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberChangeUsernameViewModel id currentPassword unavailableUsername
        let result = controller.ChangeUsername(id, viewModel)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel id String.empty unavailableUsername

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newUsername" "Username is unavailable"

    [<Test>]
    let ``Calling change username post, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let newUsername = String.random 32

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
        let passwordData = Crypto.hash currentPassword 100
        let memberData =
            memberDataFactory.Invoke(id, String.random 32, passwordData, EmailAddress.random 32, String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberChangeUsernameViewModel id currentPassword newUsername
        let result = controller.ChangeUsername(id, viewModel)

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
