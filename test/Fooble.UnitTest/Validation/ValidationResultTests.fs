namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module ValidationTests =

    [<Test>]
    let ``Calling param name, with valid validation result, raises expected exception`` () =
        let expectedMessage = "Result was not invalid"

        let result = ValidationResult.valid
        raisesWith<InvalidOperationException> <@ result.ParamName @> (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling param name, with invalid validation result, returns expected param name`` () =
        let expectedParamName = String.random 64

        let result = ValidationResult.makeInvalid expectedParamName (String.random 64)

        result.ParamName =! expectedParamName

    [<Test>]
    let ``Calling message, with valid validation result, raises expected exception`` () =
        let expectedMessage = "Result was not invalid"

        let result = ValidationResult.valid
        raisesWith<InvalidOperationException> <@ result.Message @> (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling message, with invalid validation result, returns expected message`` () =
        let expectedMessage = String.random 64

        let result = ValidationResult.makeInvalid (String.random 64) expectedMessage

        result.Message =! expectedMessage

    [<Test>]
    let ``Calling is valid, with valid result, returns true`` () =
        let result = ValidationResult.valid

        result.IsValid =! true

    [<Test>]
    let ``Calling is valid, with invalid result, returns false`` () =
        let result = ValidationResult.makeInvalid (String.random 64) (String.random 64)

        result.IsValid =! false

    [<Test>]
    let ``Calling is invalid, with valid result, returns false`` () =
        let result = ValidationResult.valid

        result.IsInvalid =! false

    [<Test>]
    let ``Calling is invalid, with invalid result, returns true`` () =
        let result = ValidationResult.makeInvalid (String.random 64) (String.random 64)

        result.IsInvalid =! true
