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
    let ``Calling is success, as username unavailable result, returns false`` () =
        let commandResult = MemberRegisterCommand.usernameUnavailableResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, as email unavailable result, returns false`` () =
        let commandResult = MemberRegisterCommand.emailUnavailableResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is username unavailable, as success result, returns false`` () =
        let commandResult = MemberRegisterCommand.successResult

        commandResult.IsUsernameUnavailable =! false

    [<Test>]
    let ``Calling is username unavailable, as username unavailable result, returns true`` () =
        let commandResult = MemberRegisterCommand.usernameUnavailableResult

        commandResult.IsUsernameUnavailable =! true

    [<Test>]
    let ``Calling is username unavailable, as email unavailable result, returns false`` () =
        let commandResult = MemberRegisterCommand.emailUnavailableResult

        commandResult.IsUsernameUnavailable =! false

    [<Test>]
    let ``Calling is email unavailable, as success result, returns false`` () =
        let commandResult = MemberRegisterCommand.successResult

        commandResult.IsEmailUnavailable =! false

    [<Test>]
    let ``Calling is email unavailable, as username unavailable result, returns false`` () =
        let commandResult = MemberRegisterCommand.usernameUnavailableResult

        commandResult.IsEmailUnavailable =! false

    [<Test>]
    let ``Calling is email unavailable, as email unavailable result, returns true`` () =
        let commandResult = MemberRegisterCommand.emailUnavailableResult

        commandResult.IsEmailUnavailable =! true

    [<Test>]
    let ``Calling add model error, as success result, returns expected read model`` () =
        let commandResult = MemberRegisterCommand.successResult
        let modelState = ModelStateDictionary()
        MemberRegisterCommandResult.addModelErrors commandResult modelState

        modelState.IsValid =! true

    [<Test>]
    let ``Calling add model error, as username unavailable result, returns expected read model`` () =
        let expectedKey = "username"
        let expectedException = "Username is unavailable"

        let commandResult = MemberRegisterCommand.usernameUnavailableResult
        let modelState = ModelStateDictionary()
        MemberRegisterCommandResult.addModelErrors commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling add model error, as email unavailable result, returns expected read model`` () =
        let expectedKey = "email"
        let expectedException = "Email is already registered"

        let commandResult = MemberRegisterCommand.emailUnavailableResult
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
    let ``Calling to message display read model, as username unavailable result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Register"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Requested username is unavailable."

        let actualReadModel =
            MemberRegisterCommand.usernameUnavailableResult
            |> MemberRegisterCommandResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
