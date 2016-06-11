namespace Fooble.UnitTest

open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module MemberChangeEmailCommandResultTests =

    [<Test>]
    let ``Calling is success, as success result, returns true`` () =
        let commandResult = MemberChangeEmailCommand.successResult

        commandResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, as not found result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.notFoundResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, as incorrect password result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.incorrectPasswordResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, as unavailable email result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.unavailableEmailResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is not found, as success result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.successResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, as not found result, returns true`` () =
        let commandResult = MemberChangeEmailCommand.notFoundResult

        commandResult.IsNotFound =! true

    [<Test>]
    let ``Calling is not found, as incorrect password result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.incorrectPasswordResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, as unavailable email result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.unavailableEmailResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is incorrect password, as success result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.successResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is incorrect password, as not found result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.notFoundResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is incorrect password, as incorrect password result, returns true`` () =
        let commandResult = MemberChangeEmailCommand.incorrectPasswordResult

        commandResult.IsIncorrectPassword =! true

    [<Test>]
    let ``Calling is incorrect password, as unavailable email result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.unavailableEmailResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is unavailable email, as success result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.successResult

        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling is unavailable email, as not found result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.notFoundResult

        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling is unavailable email, as incorrect password result, returns false`` () =
        let commandResult = MemberChangeEmailCommand.incorrectPasswordResult

        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling is unavailable email, as unavailable email result, returns true`` () =
        let commandResult = MemberChangeEmailCommand.unavailableEmailResult

        commandResult.IsUnavailableEmail =! true

    [<Test>]
    let ``Calling add model error, as success result, returns expected read model`` () =
        let commandResult = MemberChangeEmailCommand.successResult
        let modelState = ModelStateDictionary()
        MemberChangeEmailCommandResult.addModelErrors commandResult modelState

        modelState.IsValid =! true

    [<Test>]
    let ``Calling add model error, as incorrect password result, returns expected read model`` () =
        let expectedKey = "currentPassword"
        let expectedException = "Current password is incorrect"

        let commandResult = MemberChangeEmailCommand.incorrectPasswordResult
        let modelState = ModelStateDictionary()
        MemberChangeEmailCommandResult.addModelErrors commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling add model error, as unavailable email result, returns expected read model`` () =
        let expectedKey = "newEmail"
        let expectedException = "Email is already registered"

        let commandResult = MemberChangeEmailCommand.unavailableEmailResult
        let modelState = ModelStateDictionary()
        MemberChangeEmailCommandResult.addModelErrors commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling to message display read model, as success result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let commandResult = MemberChangeEmailCommand.successResult

        testInvalidOperationException expectedMessage
            <@ MemberChangeEmailCommandResult.toMessageDisplayReadModel commandResult @>

    [<Test>]
    let ``Calling to message display read model, as not found result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Email"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let actualReadModel =
            MemberChangeEmailCommand.notFoundResult
            |> MemberChangeEmailCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling to message display read model, as incorrect password result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Email"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Supplied password is incorrect."

        let actualReadModel =
            MemberChangeEmailCommand.incorrectPasswordResult
            |> MemberChangeEmailCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling to message display read model, as unavailable email result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Email"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Supplied email is already registered."

        let actualReadModel =
            MemberChangeEmailCommand.unavailableEmailResult
            |> MemberChangeEmailCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
