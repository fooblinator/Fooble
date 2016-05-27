namespace Fooble.UnitTest.SelfServiceRegister

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module SelfServiceRegisterExtensionsTests =

    [<Test>]
    let ``Calling add model error, as success result of self-service register command result, returns expected read model`` () =
        let commandResult = SelfServiceRegister.CommandResult.success
        let modelState = ModelStateDictionary()
        SelfServiceRegister.addModelErrorIfNotSuccess commandResult modelState

        test <@ modelState.IsValid @>

    [<Test>]
    let ``Calling add model error, as username unavailable result of self-service register command result, returns expected read model`` () =
        let expectedKey = "username"
        let expectedException = "Username is unavailable"

        let commandResult = SelfServiceRegister.CommandResult.usernameUnavailable
        let modelState = ModelStateDictionary()
        SelfServiceRegister.addModelErrorIfNotSuccess commandResult modelState

        test <@ not <| modelState.IsValid @>
        test <@ modelState.ContainsKey(expectedKey) @>
        test <@ modelState.[expectedKey].Errors.Count = 1 @>
        test <@ modelState.[expectedKey].Errors.[0].ErrorMessage = expectedException @>

    [<Test>]
    let ``Calling add model error, as email unavailable result of self-service register command result, returns expected read model`` () =
        let expectedKey = "email"
        let expectedException = "Email is already registered"

        let commandResult = SelfServiceRegister.CommandResult.emailUnavailable
        let modelState = ModelStateDictionary()
        SelfServiceRegister.addModelErrorIfNotSuccess commandResult modelState

        test <@ not <| modelState.IsValid @>
        test <@ modelState.ContainsKey(expectedKey) @>
        test <@ modelState.[expectedKey].Errors.Count = 1 @>
        test <@ modelState.[expectedKey].Errors.[0].ErrorMessage = expectedException @>

    [<Test>]
    let ``Calling to message display read model, as success result of self-service register command result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let commandResult = SelfServiceRegister.CommandResult.success

        raisesWith<InvalidOperationException> <@ MessageDisplay.ofSelfServiceRegisterCommandResult commandResult @>
            (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as username unavailable result of self-service register command result, returns expected read model`` () =
        let expectedHeading = "Self-Service"
        let expectedSubHeading = "Register"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplay.Severity.warning
        let expectedMessage = "Requested username is unavailable."

        let readModel =
            SelfServiceRegister.CommandResult.usernameUnavailable |> MessageDisplay.ofSelfServiceRegisterCommandResult

        test <@ readModel.Heading = expectedHeading @>
        test <@ readModel.SubHeading = expectedSubHeading @>
        test <@ readModel.StatusCode = expectedStatusCode @>
        test <@ readModel.Severity = expectedSeverity @>
        test <@ readModel.Message = expectedMessage @>
