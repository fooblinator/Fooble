namespace Fooble.UnitTest

open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module MemberDeactivateCommandResultTests =

    [<Test>]
    let ``Calling is success, as success result, returns true`` () =
        let commandResult = MemberDeactivateCommand.successResult

        commandResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, as not found result, returns false`` () =
        let commandResult = MemberDeactivateCommand.notFoundResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, as incorrect password result, returns false`` () =
        let commandResult = MemberDeactivateCommand.incorrectPasswordResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is not found, as success result, returns false`` () =
        let commandResult = MemberDeactivateCommand.successResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, as not found result, returns true`` () =
        let commandResult = MemberDeactivateCommand.notFoundResult

        commandResult.IsNotFound =! true

    [<Test>]
    let ``Calling is not found, as incorrect password result, returns false`` () =
        let commandResult = MemberDeactivateCommand.incorrectPasswordResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is incorrect password, as success result, returns false`` () =
        let commandResult = MemberDeactivateCommand.successResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is incorrect password, as not found result, returns false`` () =
        let commandResult = MemberDeactivateCommand.notFoundResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is incorrect password, as incorrect password result, returns true`` () =
        let commandResult = MemberDeactivateCommand.incorrectPasswordResult

        commandResult.IsIncorrectPassword =! true

    [<Test>]
    let ``Calling add model error, as success result, returns expected read model`` () =
        let commandResult = MemberDeactivateCommand.successResult
        let modelState = ModelStateDictionary()
        MemberDeactivateCommandResult.addModelErrors commandResult modelState

        modelState.IsValid =! true

    [<Test>]
    let ``Calling add model error, as incorrect password result, returns expected read model`` () =
        let expectedKey = "currentPassword"
        let expectedException = "Current password is incorrect"

        let commandResult = MemberDeactivateCommand.incorrectPasswordResult
        let modelState = ModelStateDictionary()
        MemberDeactivateCommandResult.addModelErrors commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling to message display read model, as success result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let commandResult = MemberDeactivateCommand.successResult

        testInvalidOperationException expectedMessage
            <@ MemberDeactivateCommandResult.toMessageDisplayReadModel commandResult @>

    [<Test>]
    let ``Calling to message display read model, as not found result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Deactivate"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let actualReadModel =
            MemberDeactivateCommand.notFoundResult
            |> MemberDeactivateCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling to message display read model, as incorrect password result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Deactivate"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Supplied password is incorrect."

        let actualReadModel =
            MemberDeactivateCommand.incorrectPasswordResult
            |> MemberDeactivateCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
