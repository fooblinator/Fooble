namespace Fooble.UnitTest

open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module MemberChangePasswordCommandResultTests =

    [<Test>]
    let ``Calling is success, as success result, returns true`` () =
        let commandResult = MemberChangePasswordCommand.successResult

        commandResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, as not found result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.notFoundResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, as incorrect password result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.incorrectPasswordResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is not found, as success result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.successResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, as not found result, returns true`` () =
        let commandResult = MemberChangePasswordCommand.notFoundResult

        commandResult.IsNotFound =! true

    [<Test>]
    let ``Calling is not found, as incorrect password result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.incorrectPasswordResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is invalid, as success result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.successResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is invalid, as not found result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.notFoundResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is invalid, as incorrect password result, returns true`` () =
        let commandResult = MemberChangePasswordCommand.incorrectPasswordResult

        commandResult.IsIncorrectPassword =! true

    [<Test>]
    let ``Calling add model error, as success result, returns expected read model`` () =
        let commandResult = MemberChangePasswordCommand.successResult
        let modelState = ModelStateDictionary()
        MemberChangePasswordCommandResult.addModelErrors commandResult modelState

        modelState.IsValid =! true

    [<Test>]
    let ``Calling add model error, as incorrect password result, returns expected read model`` () =
        let expectedKey = "currentPassword"
        let expectedException = "Current password is incorrect"

        let commandResult = MemberChangePasswordCommand.incorrectPasswordResult
        let modelState = ModelStateDictionary()
        MemberChangePasswordCommandResult.addModelErrors commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling to message display read model, as success result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let commandResult = MemberChangePasswordCommand.successResult

        testInvalidOperationException expectedMessage
            <@ MemberChangePasswordCommandResult.toMessageDisplayReadModel commandResult @>

    [<Test>]
    let ``Calling to message display read model, as not found result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Password"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let actualReadModel =
            MemberChangePasswordCommand.notFoundResult
            |> MemberChangePasswordCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling to message display read model, as incorrect password result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Password"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Supplied password is incorrect."

        let actualReadModel =
            MemberChangePasswordCommand.incorrectPasswordResult
            |> MemberChangePasswordCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
