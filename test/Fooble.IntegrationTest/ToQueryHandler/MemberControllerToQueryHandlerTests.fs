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
    let ``Constructing, with valid parameters, returns expected result`` () =
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        ignore (new MemberController(mediator, keyGenerator))

    [<Test>]
    let ``Calling change password, with matching member id in data store, returns expected result`` () =
        let matchingId = Guid.random ()
        let emptyCurrentPassword = String.empty
        let emptyNewPassword = String.empty
        let emptyConfirmPassword = String.empty

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberId(any ())).Returns(true).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.ChangePassword(matchingId)

        contextMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel emptyCurrentPassword emptyNewPassword emptyConfirmPassword

    [<Test>]
    let ``Calling change password, with no matching member id in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Password"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberId(any ())).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let result = controller.ChangePassword(nonMatchingId)

        contextMock.Verify()

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
    let ``Calling detail, with match in data store, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64
        let expectedRegistered = DateTime.UtcNow
        let expectedPasswordChanged = DateTime.UtcNow

        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData = makeTestMemberData2 expectedId expectedUsername passwordData expectedEmail expectedNickname
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(Some(memberData)).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)
        let result = controller.Detail(expectedId)

        contextMock.Verify()

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
    let ``Calling detail, with no match in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedHeading = "Member"
        let expectedSubHeading = "Detail"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(None).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)
        let result = controller.Detail(nonMatchingId)

        contextMock.Verify()

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
        let expectedMemberCount = 5

        let members =
            List.init expectedMemberCount (fun _ ->
                let passwordData = Crypto.hash (Password.random 32) 100
                makeTestMemberData2 (Guid.random ()) (String.random 32) passwordData (EmailAddress.random 32)
                    (String.random 64))
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMembers()).Returns(members).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)
        let result = controller.List()

        contextMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberListReadModel =! true

        let actualReadModel = viewResult.Model :?> IMemberListReadModel
        testMemberListReadModel actualReadModel members expectedMemberCount

    [<Test>]
    let ``Calling list, with no matches in data store, returns expected result`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "List"
        let expectedStatusCode = 200
        let expectedSeverity = MessageDisplayReadModel.informationalSeverity
        let expectedMessage = "No members have yet been added."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMembers()).Returns([]).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)
        let result = controller.List()

        contextMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
