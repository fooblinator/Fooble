namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module ValidationExtensionsTests =

    [<Test>]
    let ``Calling add model error, as valid result of validation result, returns expected read model`` () =
        let validationResult = ValidationResult.valid
        let modelState = ModelStateDictionary()
        ValidationExtensions.addModelErrorIfNotValid validationResult modelState

        modelState.IsValid =! true

    [<Test>]
    let ``Calling add model error, as invalid result of validation result, returns expected read model`` () =
        let expectedKey = String.random 64
        let expectedException = String.random 64

        let validationResult = ValidationResult.makeInvalid expectedKey expectedException
        let modelState = ModelStateDictionary()
        ValidationExtensions.addModelErrorIfNotValid validationResult modelState

        testModelState modelState expectedKey expectedException

    [<Test>]
    let ``Calling to message display read model, as valid result of validation result, raises expected exception`` () =
        let expectedMessage = "Result was not invalid"

        let validationResult = ValidationResult.valid
        raisesWith<InvalidOperationException> <@ ValidationExtensions.toMessageDisplayReadModel validationResult @>
            (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as invalid result of validation result, returns expected read model`` () =
        let innerMessage = String.random 64
        let expectedHeading = "Validation"
        let expectedSubHeading = String.empty
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplayReadModel.errorSeverity
        let expectedMessage = sprintf "Validation was not successful and returned: \"%s\"" innerMessage

        let actualReadModel =
            ValidationResult.makeInvalid (String.random 64) innerMessage
            |> ValidationExtensions.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
