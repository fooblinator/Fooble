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
open System.Web.Mvc

[<TestFixture>]
module MemberControllerToDataStoreTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()

        ignore <| new MemberController(mediator)

    [<Test>]
    let ``Calling detail, with matches in data store, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let mediator = container.Resolve<IMediator>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            memberDataFactory.Invoke(expectedId, expectedUsername, passwordData, expectedEmail, expectedNickname)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let controller = new MemberController(mediator)
        let result = controller.Detail(expectedId.ToString())

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? IMemberDetailReadModel @>

        let actualReadModel = viewResult.Model :?> IMemberDetailReadModel
        testMemberDetailReadModel actualReadModel expectedId expectedUsername expectedEmail expectedNickname

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
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let mediator = container.Resolve<IMediator>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        let controller = new MemberController(mediator)
        let result = controller.Detail(nonMatchingId.ToString())

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "MessageDisplay" @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling list, with matches in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let mediator = container.Resolve<IMediator>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching members to the data store
        let members =
            List.init 5 (fun _ ->
                let passwordData = Crypto.hash (Password.random 32) 100
                memberDataFactory.Invoke(Guid.random (), String.random 32, passwordData, EmailAddress.random (),
                    String.random 64))
        List.iter (fun x -> context.AddMember(x)) members

        // persist changes to the data store
        context.SaveChanges()

        let controller = new MemberController(mediator)
        let result = controller.List()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? IMemberListReadModel @>

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
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let mediator = container.Resolve<IMediator>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        let controller = new MemberController(mediator)
        let result = controller.List()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "MessageDisplay" @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
