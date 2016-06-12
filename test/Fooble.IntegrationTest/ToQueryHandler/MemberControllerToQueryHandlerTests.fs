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
open System
open System.Web.Mvc

[<TestFixture>]
module MemberControllerToQueryHandlerTests =

    [<Test>]
    let ``Calling change email, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let heading = "Member"
        let subHeading = "Change Email"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), considerDeactivated = false)).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.ChangeEmail(notFoundId)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change email, with successful parameters, returns expected result`` () =
        let id = Guid.random ()

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), considerDeactivated = false)).Returns(true).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.ChangeEmail(id)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel actualViewModel id String.empty String.empty

    [<Test>]
    let ``Calling change other, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let heading = "Member"
        let subHeading = "Change Other"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), considerDeactivated = false)).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.ChangeOther(notFoundId)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change other, with successful parameters, returns expected result`` () =
        let id = Guid.random ()

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), considerDeactivated = false)).Returns(true).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.ChangeOther(id)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel actualViewModel id String.empty

    [<Test>]
    let ``Calling change password, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let heading = "Member"
        let subHeading = "Change Password"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), considerDeactivated = false)).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.ChangePassword(notFoundId)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change password, with successful parameters, returns expected result`` () =
        let id = Guid.random ()

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), considerDeactivated = false)).Returns(true).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.ChangePassword(id)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel id String.empty String.empty String.empty

    [<Test>]
    let ``Calling change username, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), considerDeactivated = false)).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
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

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), considerDeactivated = false)).Returns(true).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
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
    let ``Calling detail, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.random ()
        let heading = "Member"
        let subHeading = "Detail"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), considerDeactivated = false)).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.Detail(notFoundId)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling detail, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let username = String.random 32
        let email = EmailAddress.random 32
        let nickname = String.random 64
        let registered = DateTime.UtcNow
        let passwordChanged = DateTime.UtcNow
        let isDeactivated = false

        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            makeTestMemberData id username passwordData email nickname registered passwordChanged isDeactivated
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(Some(memberData)).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.Detail(id)

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberDetailReadModel =! true

        let actualReadModel = viewResult.Model :?> IMemberDetailReadModel
        testMemberDetailReadModel actualReadModel id username email nickname registered passwordChanged

    [<Test>]
    let ``Calling list, with no members in data store, returns expected result`` () =
        let heading = "Member"
        let subHeading = "List"
        let statusCode = 200
        let severity = MessageDisplayReadModel.informationalSeverity
        let message = "No members have yet been added."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMembers(considerDeactivated = false)).Returns([]).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.List()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling list, with members in data store, returns expected result`` () =
        let memberCount = 5

        let members =
            List.init memberCount (fun _ ->
                let passwordData = Crypto.hash (Password.random 32) 100
                makeTestMemberData2 (Guid.random ()) (String.random 32) passwordData (EmailAddress.random 32)
                    (String.random 64) false)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMembers(considerDeactivated = false)).Returns(members).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.List()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberListReadModel =! true

        let actualReadModel = viewResult.Model :?> IMemberListReadModel
        testMemberListReadModel actualReadModel members memberCount

    [<Test>]
    let ``Calling register, returns expected result`` () =
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), considerDeactivated = false)).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
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
