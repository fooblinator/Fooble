﻿namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module ValidationResultTests =

    [<Test>]
    let ``Calling valid, returns validation result`` () =
        let result = Validation.Result.valid

        test <@ box result :? IValidationResult @>

    [<Test>]
    let ``Calling make invalid, with valid parameters, returns validation result`` () =
        let result = Validation.Result.makeInvalid <|| (randomString (), randomString ())

        test <@ box result :? IValidationResult @>

    [<Test>]
    let ``Calling param name, with invalid validation result, returns expected param name`` () =
        let expectedParamName = randomString ()

        let result = Validation.Result.makeInvalid <|| (expectedParamName, randomString ())

        test <@ result.ParamName = expectedParamName @>

    [<Test>]
    let ``Calling message, with invalid validation result, returns expected message`` () =
        let expectedMessage = randomString ()

        let result = Validation.Result.makeInvalid <|| (randomString (), expectedMessage)

        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling is valid, with valid result, returns true`` () =
        let result = Validation.Result.valid

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling is valid, with invalid result, returns false`` () =
        let result = Validation.Result.makeInvalid <|| (randomString (), randomString ())

        test <@ not <| result.IsValid @>

    [<Test>]
    let ``Calling is invalid, with valid result, returns false`` () =
        let result = Validation.Result.valid

        test <@ not <| result.IsInvalid @>

    [<Test>]
    let ``Calling is invalid, with invalid result, returns true`` () =
        let result = Validation.Result.makeInvalid <|| (randomString (), randomString ())

        test <@ result.IsInvalid @>
