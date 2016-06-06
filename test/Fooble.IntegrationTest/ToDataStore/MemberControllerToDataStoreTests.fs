namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.IntegrationTest
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open Fooble.Presentation
open Fooble.Presentation.Infrastructure
open Fooble.Web.Controllers
open MediatR
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module MemberControllerToDataStoreTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        ignore (new MemberController(mediator, keyGenerator))

    [<Test>]
    let ``Calling detail, with matches in data store, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64
        let expectedRegistered = DateTime.UtcNow
        let expectedPasswordChanged = DateTime.UtcNow

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
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            memberDataFactory.Invoke(expectedId, expectedUsername, passwordData, expectedEmail, expectedNickname)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)
        let result = controller.Detail(expectedId.ToString())

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberDetailReadModel =! true

        let actualReadModel = viewResult.Model :?> IMemberDetailReadModel
        testMemberDetailReadModel actualReadModel expectedId expectedUsername expectedEmail expectedNickname
            expectedRegistered expectedPasswordChanged

    [<Test>]
    let ``Calling detail, with no matches in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedHeading = "Member"
        let expectedSubHeading = "Detail"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

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
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)
        let result = controller.Detail(nonMatchingId.ToString())

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling list, with matches in data store, returns expected result`` () =
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
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching members to the data store
        let members =
            List.init 5 (fun _ ->
                let passwordData = Crypto.hash (Password.random 32) 100
                memberDataFactory.Invoke(Guid.random (), String.random 32, passwordData, EmailAddress.random 32,
                    String.random 64))
        List.iter (fun x -> context.AddMember(x)) members

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)
        let result = controller.List()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberListReadModel =! true

        let actualReadModel = viewResult.Model :?> IMemberListReadModel
        testMemberListReadModel actualReadModel members

    [<Test>]
    let ``Calling list, with no matches in data store, returns expected result`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "List"
        let expectedStatusCode = 200
        let expectedSeverity = MessageDisplayReadModel.informationalSeverity
        let expectedMessage = "No members have yet been added."

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
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)
        let result = controller.List()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling register post, with existing username in data store, returns expected result`` () =
        let existingUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

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
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let memberData =
            let passwordData = Crypto.hash (Password.random 32) 100
            memberDataFactory.Invoke(Guid.random (), existingUsername, passwordData, EmailAddress.random 32,
                String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)

        let viewModel =
            bindMemberRegisterViewModel existingUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname
        let result = controller.Register viewModel

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
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let memberData =
            let passwordData = Crypto.hash (Password.random 32) 100
            memberDataFactory.Invoke(Guid.random (), String.random 32, passwordData, existingEmail,
                String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)

        let viewModel =
            bindMemberRegisterViewModel expectedUsername expectedPassword expectedConfirmPassword existingEmail
                expectedNickname
        let result = controller.Register viewModel

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

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let mediator = container.Resolve<IMediator>()
        let keyGenerator = makeTestKeyGenerator (Some expectedId)

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        use controller = new MemberController(mediator, keyGenerator)

        let password = Password.random 32
        let viewModel =
            bindMemberRegisterViewModel (String.random 32) password password (EmailAddress.random 32)
                (String.random 64)
        let result = controller.Register viewModel

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
