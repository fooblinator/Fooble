namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module MemberChangePasswordExtensionsTests =

    [<Test>]
    let ``Calling add model error, as success result of member change password command result, returns expected read model`` () =
        let commandResult = MemberChangePasswordCommand.successResult
        let modelState = ModelStateDictionary()
        MemberChangePasswordExtensions.addModelErrors commandResult modelState

        modelState.IsValid =! true

    [<Test>]
    let ``Calling add model error, as invalid result of member change password command result, returns expected read model`` () =
        let expectedKey = "currentPassword"
        let expectedException = "Current password is incorrect"

        let commandResult = MemberChangePasswordCommand.invalidResult
        let modelState = ModelStateDictionary()
        MemberChangePasswordExtensions.addModelErrors commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling to message display read model, as success result of member change password command result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let commandResult = MemberChangePasswordCommand.successResult

        raisesWith<InvalidOperationException>
            <@ MemberChangePasswordExtensions.toMessageDisplayReadModel commandResult @>
            (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as not found result of member change password command result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Password"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let actualReadModel =
            MemberChangePasswordCommand.notFoundResult
            |> MemberChangePasswordExtensions.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling to message display read model, as invalid result of member change password command result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Password"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Supplied password is invalid."

        let actualReadModel =
            MemberChangePasswordCommand.invalidResult
            |> MemberChangePasswordExtensions.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling to command, as member change password view model, returns expected command`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword expectedNewPassword
                expectedConfirmPassword

        let actualCommand = MemberChangePasswordExtensions.toCommand viewModel

        testMemberChangePasswordCommand actualCommand expectedId expectedCurrentPassword expectedNewPassword

    [<Test>]
    let ``Calling clean, as member change password view model, returns expected view model`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword expectedNewPassword
                expectedConfirmPassword

        let actualViewModel = MemberChangePasswordExtensions.clean viewModel

        testMemberChangePasswordViewModel actualViewModel expectedId String.empty String.empty String.empty