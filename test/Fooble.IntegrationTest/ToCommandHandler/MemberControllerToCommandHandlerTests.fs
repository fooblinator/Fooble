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
    let ``Calling change email post, with no matching member id in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewEmail = EmailAddress.random 32
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Email"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any (), considerDeactivated = false)).Returns(None).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberChangeEmailViewModel nonMatchingId expectedCurrentPassword expectedNewEmail
        let result = controller.ChangeEmail(nonMatchingId, viewModel)

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
    let ``Calling change email post, with incorrect current password, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewEmail = EmailAddress.random 32

        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            makeTestMemberData2 expectedId (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
                false
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

        let viewModel = bindMemberChangeEmailViewModel expectedId expectedCurrentPassword expectedNewEmail
        let result = controller.ChangeEmail(expectedId, viewModel)

        contextMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel actualViewModel expectedId String.empty expectedNewEmail

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change email post, with unavailable email, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewEmail = EmailAddress.random 32

        let passwordData = Crypto.hash expectedCurrentPassword 100
        let memberData =
            makeTestMemberData2 expectedId (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
                false
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(Some(memberData)).Verifiable()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberEmail(any (), considerDeactivated = true)).Returns(true).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberChangeEmailViewModel expectedId expectedCurrentPassword expectedNewEmail
        let result = controller.ChangeEmail(expectedId, viewModel)

        contextMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel actualViewModel expectedId String.empty expectedNewEmail

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newEmail" "Email is already registered"

    [<Test>]
    let ``Calling change email post, with successful parameters, returns expected result`` () =
        let matchingId = Guid.random ()
        let matchingIdString = String.ofGuid matchingId
        let expectedCurrentPassword = Password.random 32
        let expectedNewEmail = EmailAddress.random 32

        let passwordData = Crypto.hash expectedCurrentPassword 100
        let memberData =
            makeTestMemberData2 matchingId (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
                false
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(Some(memberData)).Verifiable()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberEmail(any (), considerDeactivated = true)).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberChangeEmailViewModel matchingId expectedCurrentPassword expectedNewEmail
        let result = controller.ChangeEmail(matchingId, viewModel)

        contextMock.Verify()

        isNull result =! false
        result :? RedirectToRouteResult =! true

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "member"

        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "detail"

        routeValues.ContainsKey("id") =! true
        routeValues.["id"].ToString().ToLowerInvariant() =! matchingIdString

    [<Test>]
    let ``Calling change other post, with no matching member id in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedNickname = String.random 64
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Other"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(None).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberChangeOtherViewModel nonMatchingId expectedNickname
        let result = controller.ChangeOther(nonMatchingId, viewModel)

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
    let ``Calling change other post, with successful parameters, returns expected result`` () =
        let matchingId = Guid.random ()
        let matchingIdString = String.ofGuid matchingId
        let expectedNickname = String.random 64

        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            makeTestMemberData2 matchingId (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
                false
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

        let viewModel = bindMemberChangeOtherViewModel matchingId expectedNickname
        let result = controller.ChangeOther(matchingId, viewModel)

        contextMock.Verify()

        isNull result =! false
        result :? RedirectToRouteResult =! true

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "member"

        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "detail"

        routeValues.ContainsKey("id") =! true
        routeValues.["id"].ToString().ToLowerInvariant() =! matchingIdString

    [<Test>]
    let ``Calling change password post, with no matching member id in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Password"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any (), considerDeactivated = false)).Returns(None).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let viewModel =
            bindMemberChangePasswordViewModel nonMatchingId expectedCurrentPassword expectedNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(nonMatchingId, viewModel)

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
    let ``Calling change password post, with incorrect current password, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            makeTestMemberData2 expectedId (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
                false
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

        let viewModel =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword expectedNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(expectedId, viewModel)

        contextMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change password post, with successful parameters, returns expected result`` () =
        let matchingId = Guid.random ()
        let matchingIdString = String.ofGuid matchingId
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let passwordData = Crypto.hash expectedCurrentPassword 100
        let memberData =
            makeTestMemberData2 matchingId (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
                false
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

        let viewModel =
            bindMemberChangePasswordViewModel matchingId expectedCurrentPassword expectedNewPassword
                expectedConfirmPassword
        let result = controller.ChangePassword(matchingId, viewModel)

        contextMock.Verify()

        isNull result =! false
        result :? RedirectToRouteResult =! true

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "member"

        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "detail"

        routeValues.ContainsKey("id") =! true
        routeValues.["id"].ToString().ToLowerInvariant() =! matchingIdString

    [<Test>]
    let ``Calling change username post, with no matching member id in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewUsername = String.random 32
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Username"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any (), considerDeactivated = false)).Returns(None).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberChangeUsernameViewModel nonMatchingId expectedCurrentPassword expectedNewUsername
        let result = controller.ChangeUsername(nonMatchingId, viewModel)

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
    let ``Calling change username post, with incorrect current password, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewUsername = String.random 32

        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            makeTestMemberData2 expectedId (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
                false
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

        let viewModel = bindMemberChangeUsernameViewModel expectedId expectedCurrentPassword expectedNewUsername
        let result = controller.ChangeUsername(expectedId, viewModel)

        contextMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel expectedId String.empty expectedNewUsername

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change username post, with unavailable username, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewUsername = String.random 32

        let passwordData = Crypto.hash expectedCurrentPassword 100
        let memberData =
            makeTestMemberData2 expectedId (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
                false
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(Some(memberData)).Verifiable()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberUsername(any (), considerDeactivated = true)).Returns(true).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberChangeUsernameViewModel expectedId expectedCurrentPassword expectedNewUsername
        let result = controller.ChangeUsername(expectedId, viewModel)

        contextMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true

        let actualViewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel actualViewModel expectedId String.empty expectedNewUsername

        let actualModelState = viewResult.ViewData.ModelState
        testModelState actualModelState "newUsername" "Username is unavailable"

    [<Test>]
    let ``Calling change username post, with successful parameters, returns expected result`` () =
        let matchingId = Guid.random ()
        let matchingIdString = String.ofGuid matchingId
        let expectedCurrentPassword = Password.random 32
        let expectedNewUsername = String.random 32

        let passwordData = Crypto.hash expectedCurrentPassword 100
        let memberData =
            makeTestMemberData2 matchingId (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
                false
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(Some(memberData)).Verifiable()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberUsername(any (), considerDeactivated = true)).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = container.Resolve<KeyGenerator>()
        use controller = new MemberController(mediator, keyGenerator)

        let viewModel = bindMemberChangeUsernameViewModel matchingId expectedCurrentPassword expectedNewUsername
        let result = controller.ChangeUsername(matchingId, viewModel)

        contextMock.Verify()

        isNull result =! false
        result :? RedirectToRouteResult =! true

        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues

        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "member"

        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "detail"

        routeValues.ContainsKey("id") =! true
        routeValues.["id"].ToString().ToLowerInvariant() =! matchingIdString

    [<Test>]
    let ``Calling register post, with existing username in data store, returns expected result`` () =
        let existingUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(any (), considerDeactivated = true)).Returns(true).Verifiable()

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
        contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(any (), considerDeactivated = true)).Returns(true).Verifiable()

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
    let ``Calling register post, with successful parameters, returns expected result`` () =
        let expectedId = Guid.random ()

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberUsername(any (), considerDeactivated = true)).Returns(false).Verifiable()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberEmail(any (), considerDeactivated = true)).Returns(false).Verifiable()

        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations(contextMock.Object, mock ())))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let keyGenerator = makeTestKeyGenerator (Some(expectedId))
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
