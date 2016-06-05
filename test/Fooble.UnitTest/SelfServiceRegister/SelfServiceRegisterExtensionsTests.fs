namespace Fooble.UnitTest

open Fooble.Common
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

        modelState.IsValid =! true

    [<Test>]
    let ``Calling add model error, as username unavailable result of self-service register command result, returns expected read model`` () =
        let expectedKey = "username"
        let expectedException = "Username is unavailable"

        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult
        let modelState = ModelStateDictionary()
        SelfServiceRegisterExtensions.addModelErrorIfNotSuccess commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling add model error, as email unavailable result of self-service register command result, returns expected read model`` () =
        let expectedKey = "email"
        let expectedException = "Email is already registered"

        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult
        let modelState = ModelStateDictionary()
        SelfServiceRegisterExtensions.addModelErrorIfNotSuccess commandResult modelState

        testModelState modelState expectedKey expectedException

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

        let actualReadModel =
            SelfServiceRegisterCommand.usernameUnavailableResult
            |> SelfServiceRegisterExtensions.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

//    [<Test>]
//    let ``Calling to command, as null self-service register view model, raises expected exception`` () =
//        let expectedParamName = "viewModel"
//        let expectedMessage = "View model is required"
//
//        raisesWith<ArgumentException>
//            <@ SelfServiceRegisterExtensions.toCommand null (Guid.random ()) @>
//            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling to command, as self-service register view model, returns expected command`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let viewModel =
            bindSelfServiceRegisterViewModel2 expectedUsername expectedPassword expectedPassword expectedEmail
                expectedNickname

        let actualCommand = SelfServiceRegisterExtensions.toCommand viewModel expectedId

        testSelfServiceRegisterCommand actualCommand expectedId expectedUsername expectedPassword expectedEmail
            expectedNickname