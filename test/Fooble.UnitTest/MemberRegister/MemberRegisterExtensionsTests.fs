namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module MemberRegisterExtensionsTests =

    [<Test>]
    let ``Calling add model error, as success result of member register command result, returns expected read model`` () =
        let commandResult = MemberRegisterCommand.successResult
        let modelState = ModelStateDictionary()
        MemberRegisterExtensions.addModelErrorIfNotSuccess commandResult modelState

        modelState.IsValid =! true

    [<Test>]
    let ``Calling add model error, as username unavailable result of member register command result, returns expected read model`` () =
        let expectedKey = "username"
        let expectedException = "Username is unavailable"

        let commandResult = MemberRegisterCommand.usernameUnavailableResult
        let modelState = ModelStateDictionary()
        MemberRegisterExtensions.addModelErrorIfNotSuccess commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling add model error, as email unavailable result of member register command result, returns expected read model`` () =
        let expectedKey = "email"
        let expectedException = "Email is already registered"

        let commandResult = MemberRegisterCommand.emailUnavailableResult
        let modelState = ModelStateDictionary()
        MemberRegisterExtensions.addModelErrorIfNotSuccess commandResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling to message display read model, as success result of member register command result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let commandResult = MemberRegisterCommand.successResult

        raisesWith<InvalidOperationException>
            <@ MemberRegisterExtensions.toMessageDisplayReadModel commandResult @>
            (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as username unavailable result of member register command result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Register"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "Requested username is unavailable."

        let actualReadModel =
            MemberRegisterCommand.usernameUnavailableResult
            |> MemberRegisterExtensions.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage

    [<Test>]
    let ``Calling to command, as member register view model, returns expected command`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let viewModel =
            bindMemberRegisterViewModel2 expectedUsername expectedPassword expectedPassword expectedEmail
                expectedNickname

        let actualCommand = MemberRegisterExtensions.toCommand viewModel expectedId

        testMemberRegisterCommand actualCommand expectedId expectedUsername expectedPassword expectedEmail
            expectedNickname

    [<Test>]
    let ``Calling clean, as member register view model, returns expected view model`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let viewModel =
            bindMemberRegisterViewModel2 expectedUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname

        let actualViewModel = MemberRegisterExtensions.clean viewModel

        testMemberRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname