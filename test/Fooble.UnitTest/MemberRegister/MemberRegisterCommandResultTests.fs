namespace Fooble.UnitTest

open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module MemberRegisterCommandResultTests =

    [<Test>]
    let ``Calling is success, as success result, returns true`` () =
        let commandResult = MemberRegisterCommand.successResult

        commandResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, as unavailable username result, returns false`` () =
        let commandResult = MemberRegisterCommand.unavailableUsernameResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, as unavailable email result, returns false`` () =
        let commandResult = MemberRegisterCommand.unavailableEmailResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is unavailable username, as success result, returns false`` () =
        let commandResult = MemberRegisterCommand.successResult

        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling is unavailable username, as unavailable username result, returns true`` () =
        let commandResult = MemberRegisterCommand.unavailableUsernameResult

        commandResult.IsUnavailableUsername =! true

    [<Test>]
    let ``Calling is unavailable username, as unavailable email result, returns false`` () =
        let commandResult = MemberRegisterCommand.unavailableEmailResult

        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling is unavailable email, as success result, returns false`` () =
        let commandResult = MemberRegisterCommand.successResult

        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling is unavailable email, as unavailable username result, returns false`` () =
        let commandResult = MemberRegisterCommand.unavailableUsernameResult

        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling is unavailable email, as unavailable email result, returns true`` () =
        let commandResult = MemberRegisterCommand.unavailableEmailResult

        commandResult.IsUnavailableEmail =! true

    [<Test>]
    let ``Calling add model error, as success result, returns expected read model`` () =
        let commandResult = MemberRegisterCommand.successResult
        let modelState = ModelStateDictionary()
        MemberRegisterCommandResult.addModelErrors commandResult modelState

        modelState.IsValid =! true

    [<Test>]
    let ``Calling add model error, as unavailable username result, returns expected read model`` () =
        let expectedKey = "username"
        let expectedException = "Username is unavailable"

        let commandResult = MemberRegisterCommand.unavailableUsernameResult
        let modelState = ModelStateDictionary()
        MemberRegisterCommandResult.addModelErrors commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling add model error, as unavailable email result, returns expected read model`` () =
        let expectedKey = "email"
        let expectedException = "Email is already registered"

        let commandResult = MemberRegisterCommand.unavailableEmailResult
        let modelState = ModelStateDictionary()
        MemberRegisterCommandResult.addModelErrors commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling to message display read model, as success result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let commandResult = MemberRegisterCommand.successResult

        testInvalidOperationException expectedMessage
            <@ MemberRegisterCommandResult.toMessageDisplayReadModel commandResult @>

    [<Test>]
    let ``Calling to message display read model, as unavailable username result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Register"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Requested username is unavailable."

        let actualReadModel =
            MemberRegisterCommand.unavailableUsernameResult
            |> MemberRegisterCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
