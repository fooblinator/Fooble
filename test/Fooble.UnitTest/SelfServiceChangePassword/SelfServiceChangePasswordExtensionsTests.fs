namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module SelfServiceChangePasswordExtensionsTests =

    [<Test>]
    let ``Calling add model error, as success result of self-service change password command result, returns expected read model`` () =
        let commandResult = SelfServiceChangePasswordCommand.successResult
        let modelState = ModelStateDictionary()
        SelfServiceChangePasswordExtensions.addModelErrorIfNotSuccess commandResult modelState

        modelState.IsValid =! true

    [<Test>]
    let ``Calling add model error, as invalid result of self-service change password command result, returns expected read model`` () =
        let expectedKey = "currentPassword"
        let expectedException = "Password is invalid"

        let commandResult = SelfServiceChangePasswordCommand.invalidResult
        let modelState = ModelStateDictionary()
        SelfServiceChangePasswordExtensions.addModelErrorIfNotSuccess commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling to message display read model, as success result of self-service change password command result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let commandResult = SelfServiceChangePasswordCommand.successResult

        raisesWith<InvalidOperationException>
            <@ SelfServiceChangePasswordExtensions.toMessageDisplayReadModel commandResult @>
            (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as not found result of self-service change password command result, returns expected read model`` () =
        let expectedHeading = "Self-Service"
        let expectedSubHeading = "Change Password"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let actualReadModel =
            SelfServiceChangePasswordCommand.notFoundResult
            |> SelfServiceChangePasswordExtensions.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling to message display read model, as invalid result of self-service change password command result, returns expected read model`` () =
        let expectedHeading = "Self-Service"
        let expectedSubHeading = "Change Password"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Supplied password is invalid."

        let actualReadModel =
            SelfServiceChangePasswordCommand.invalidResult
            |> SelfServiceChangePasswordExtensions.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling to command, as self-service change password view model, returns expected command`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let viewModel =
            bindSelfServiceChangePasswordViewModel2 expectedCurrentPassword expectedNewPassword expectedConfirmPassword

        let actualCommand = SelfServiceChangePasswordExtensions.toCommand viewModel expectedId

        testSelfServiceChangePasswordCommand actualCommand expectedId expectedCurrentPassword expectedNewPassword

    [<Test>]
    let ``Calling clean, as self-service change password view model, returns expected view model`` () =
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let viewModel =
            bindSelfServiceChangePasswordViewModel2 expectedCurrentPassword expectedNewPassword expectedConfirmPassword

        let actualViewModel = SelfServiceChangePasswordExtensions.clean viewModel

        testSelfServiceChangePasswordViewModel actualViewModel String.empty String.empty String.empty