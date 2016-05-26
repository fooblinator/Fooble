namespace Fooble.UnitTest.SelfServiceRegister

open Fooble.Core
open Fooble.UnitTest
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module SelfServiceRegisterExtensionsTests =

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
