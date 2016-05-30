namespace Fooble.UnitTest

open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module SelfServiceRegisterExtensionsTests =

    [<Test>]
    let ``Calling add model error, as success result of self-service register command result, returns expected read model`` () =
        let commandResult = SelfServiceRegisterCommand.successResult
        let modelState = ModelStateDictionary()
        SelfServiceRegisterExtensions.addModelErrorIfNotSuccess commandResult modelState

        test <@ modelState.IsValid @>

    [<Test>]
    let ``Calling add model error, as username unavailable result of self-service register command result, returns expected read model`` () =
        let expectedKey = "username"
        let expectedException = "Username is unavailable"

        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult
        let modelState = ModelStateDictionary()
        SelfServiceRegisterExtensions.addModelErrorIfNotSuccess commandResult modelState

        test <@ not <| modelState.IsValid @>
        test <@ modelState.ContainsKey(expectedKey) @>
        test <@ modelState.[expectedKey].Errors.Count = 1 @>
        test <@ modelState.[expectedKey].Errors.[0].ErrorMessage = expectedException @>

    [<Test>]
    let ``Calling add model error, as email unavailable result of self-service register command result, returns expected read model`` () =
        let expectedKey = "email"
        let expectedException = "Email is already registered"

        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult
        let modelState = ModelStateDictionary()
        SelfServiceRegisterExtensions.addModelErrorIfNotSuccess commandResult modelState

        test <@ not <| modelState.IsValid @>
        test <@ modelState.ContainsKey(expectedKey) @>
        test <@ modelState.[expectedKey].Errors.Count = 1 @>
        test <@ modelState.[expectedKey].Errors.[0].ErrorMessage = expectedException @>

    [<Test>]
    let ``Calling to message display read model, as success result of self-service register command result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let commandResult = SelfServiceRegisterCommand.successResult

        raisesWith<InvalidOperationException>
            <@ SelfServiceRegisterExtensions.toMessageDisplayReadModel commandResult @>
            (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as username unavailable result of self-service register command result, returns expected read model`` () =
        let expectedHeading = "Self-Service"
        let expectedSubHeading = "Register"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Requested username is unavailable."

        let readModel =
            SelfServiceRegisterCommand.usernameUnavailableResult
            |> SelfServiceRegisterExtensions.toMessageDisplayReadModel

        test <@ readModel.Heading = expectedHeading @>
        test <@ readModel.SubHeading = expectedSubHeading @>
        test <@ readModel.StatusCode = expectedStatusCode @>
        test <@ readModel.Severity = expectedSeverity @>
        test <@ readModel.Message = expectedMessage @>
