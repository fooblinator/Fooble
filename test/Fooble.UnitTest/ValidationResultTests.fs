namespace Fooble.UnitTest

open Fooble.Core
open Fooble.UnitTest
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module ValidationResultTests =

    [<Test>]
    let ``Calling valid, returns validation result`` () =
        let result = Validation.Result.valid

        test <@ box result :? IValidationResult @>

    [<Test>]
    let ``Calling make invalid, with valid parameters, returns validation result`` () =
        let result = Validation.Result.makeInvalid (String.random 64) (String.random 64)

        test <@ box result :? IValidationResult @>

    [<Test>]
    let ``Calling param name, with valid validation result, raises expected exception`` () =
        let expectedMessage = "Result was not invalid"

        let result = Validation.Result.valid
        raisesWith<InvalidOperationException> <@ result.ParamName @> (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling param name, with invalid validation result, returns expected param name`` () =
        let expectedParamName = String.random 64

        let result = Validation.Result.makeInvalid expectedParamName (String.random 64)

        test <@ result.ParamName = expectedParamName @>

    [<Test>]
    let ``Calling message, with valid validation result, raises expected exception`` () =
        let expectedMessage = "Result was not invalid"

        let result = Validation.Result.valid
        raisesWith<InvalidOperationException> <@ result.Message @> (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling message, with invalid validation result, returns expected message`` () =
        let expectedMessage = String.random 64

        let result = Validation.Result.makeInvalid (String.random 64) expectedMessage

        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling is valid, with valid result, returns true`` () =
        let result = Validation.Result.valid

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling is valid, with invalid result, returns false`` () =
        let result = Validation.Result.makeInvalid (String.random 64) (String.random 64)

        test <@ not <| result.IsValid @>

    [<Test>]
    let ``Calling is invalid, with valid result, returns false`` () =
        let result = Validation.Result.valid

        test <@ not <| result.IsInvalid @>

    [<Test>]
    let ``Calling is invalid, with invalid result, returns true`` () =
        let result = Validation.Result.makeInvalid (String.random 64) (String.random 64)

        test <@ result.IsInvalid @>
