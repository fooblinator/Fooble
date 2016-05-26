namespace Fooble.UnitTest.MemberDetail

open Fooble.Core
open Fooble.UnitTest
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module ValidationExtensionsTests =

    [<Test>]
    let ``Calling to message display read model, as valid result of validation result, raises expected exception`` () =
        let expectedMessage = "Result was not invalid"

        let validationResult = Validation.Result.valid

        raisesWith<InvalidOperationException> <@ MessageDisplay.ofValidationResult validationResult @> (fun x ->
            <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as invalid result of validation result, returns expected read model`` () =
        let innerMessage = String.random 64
        let expectedHeading = "Validation"
        let expectedSubHeading = String.empty
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplay.Severity.error
        let expectedMessage = sprintf "Validation was not successful and returned: \"%s\"" innerMessage

        let readModel =
            Validation.Result.makeInvalid (String.random 64) innerMessage |> MessageDisplay.ofValidationResult

        test <@ readModel.Heading = expectedHeading @>
        test <@ readModel.SubHeading = expectedSubHeading @>
        test <@ readModel.StatusCode = expectedStatusCode @>
        test <@ readModel.Severity = expectedSeverity @>
        test <@ readModel.Message = expectedMessage @>
