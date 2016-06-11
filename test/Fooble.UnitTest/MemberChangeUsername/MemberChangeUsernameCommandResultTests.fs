namespace Fooble.UnitTest

open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module MemberChangeUsernameCommandResultTests =

    [<Test>]
    let ``Calling is success, as success result, returns true`` () =
        let commandResult = MemberChangeUsernameCommand.successResult

        commandResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, as not found result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.notFoundResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, as incorrect password result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.incorrectPasswordResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, as unavailable username result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.unavailableUsernameResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is not found, as success result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.successResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, as not found result, returns true`` () =
        let commandResult = MemberChangeUsernameCommand.notFoundResult

        commandResult.IsNotFound =! true

    [<Test>]
    let ``Calling is not found, as incorrect password result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.incorrectPasswordResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, as unavailable username result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.unavailableUsernameResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is incorrect password, as success result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.successResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is incorrect password, as not found result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.notFoundResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is incorrect password, as incorrect password result, returns true`` () =
        let commandResult = MemberChangeUsernameCommand.incorrectPasswordResult

        commandResult.IsIncorrectPassword =! true

    [<Test>]
    let ``Calling is incorrect password, as unavailable username result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.unavailableUsernameResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is unavailable username, as success result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.successResult

        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling is unavailable username, as not found result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.notFoundResult

        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling is unavailable username, as incorrect password result, returns false`` () =
        let commandResult = MemberChangeUsernameCommand.incorrectPasswordResult

        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling is unavailable username, as unavailable username result, returns true`` () =
        let commandResult = MemberChangeUsernameCommand.unavailableUsernameResult

        commandResult.IsUnavailableUsername =! true

    [<Test>]
    let ``Calling add model error, as success result, returns expected read model`` () =
        let commandResult = MemberChangeUsernameCommand.successResult
        let modelState = ModelStateDictionary()
        MemberChangeUsernameCommandResult.addModelErrors commandResult modelState

        modelState.IsValid =! true

    [<Test>]
    let ``Calling add model error, as incorrect password result, returns expected read model`` () =
        let expectedKey = "currentPassword"
        let expectedException = "Current password is incorrect"

        let commandResult = MemberChangeUsernameCommand.incorrectPasswordResult
        let modelState = ModelStateDictionary()
        MemberChangeUsernameCommandResult.addModelErrors commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling add model error, as unavailable username result, returns expected read model`` () =
        let expectedKey = "newUsername"
        let expectedException = "Username is unavailable"

        let commandResult = MemberChangeUsernameCommand.unavailableUsernameResult
        let modelState = ModelStateDictionary()
        MemberChangeUsernameCommandResult.addModelErrors commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling to message display read model, as success result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let commandResult = MemberChangeUsernameCommand.successResult

        testInvalidOperationException expectedMessage
            <@ MemberChangeUsernameCommandResult.toMessageDisplayReadModel commandResult @>

    [<Test>]
    let ``Calling to message display read model, as not found result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Username"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let actualReadModel =
            MemberChangeUsernameCommand.notFoundResult
            |> MemberChangeUsernameCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling to message display read model, as incorrect password result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Username"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Supplied password is incorrect."

        let actualReadModel =
            MemberChangeUsernameCommand.incorrectPasswordResult
            |> MemberChangeUsernameCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling to message display read model, as unavailable username result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Username"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Requested username is unavailable."

        let actualReadModel =
            MemberChangeUsernameCommand.unavailableUsernameResult
            |> MemberChangeUsernameCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
