﻿namespace Fooble.UnitTest.Validation

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
        Validation.addModelErrorIfNotValid validationResult modelState

        test <@ modelState.IsValid @>

    [<Test>]
    let ``Calling add model error, as invalid result of validation result, returns expected read model`` () =
        let expectedKey = String.random 64
        let expectedException = String.random 64

        let validationResult = ValidationResult.makeInvalid expectedKey expectedException
        let modelState = ModelStateDictionary()
        Validation.addModelErrorIfNotValid validationResult modelState

        test <@ not <| modelState.IsValid @>
        test <@ modelState.ContainsKey(expectedKey) @>
        test <@ modelState.[expectedKey].Errors.Count = 1 @>
        test <@ modelState.[expectedKey].Errors.[0].ErrorMessage = expectedException @>

    [<Test>]
    let ``Calling to message display read model, as valid result of validation result, raises expected exception`` () =
        let expectedMessage = "Result was not invalid"

        let validationResult = ValidationResult.valid
        raisesWith<InvalidOperationException> <@ Validation.toMessageDisplayReadModel validationResult @> (fun x ->
            <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as invalid result of validation result, returns expected read model`` () =
        let innerMessage = String.random 64
        let expectedHeading = "Validation"
        let expectedSubHeading = String.empty
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplay.errorSeverity
        let expectedMessage = sprintf "Validation was not successful and returned: \"%s\"" innerMessage

        let readModel =
            ValidationResult.makeInvalid (String.random 64) innerMessage |> Validation.toMessageDisplayReadModel

        test <@ readModel.Heading = expectedHeading @>
        test <@ readModel.SubHeading = expectedSubHeading @>
        test <@ readModel.StatusCode = expectedStatusCode @>
        test <@ readModel.Severity = expectedSeverity @>
        test <@ readModel.Message = expectedMessage @>
