namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module ValidationResultTests =

    [<Test>]
    let ``Calling valid, returns expected result``() =
        let result = ValidationResult.valid

        test <@ result.IsValid @>
        test <@ not result.IsInvalid @>

    [<Test>]
    let ``Calling invalid, with valid parameters, returns expected result``() =
        let expectedParamName = randomGuidString()
        let expectedMessage = randomGuidString()

        let result = ValidationResult.invalid expectedParamName expectedMessage

        test <@ result.IsInvalid @>
        test <@ not result.IsValid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling param name, as invalid validation result, returns expected param name``() =
        let expectedParamName = randomGuidString()

        let result = ValidationResult.invalid expectedParamName (randomGuidString())

        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>

    [<Test>]
    let ``Calling param name, as valid validation result, raises expected exception``() =
        let expectedMessage = "Result was not invalid"

        let result = ValidationResult.valid
        raisesWith<InvalidOperationException> <@ result.ParamName @> <| fun e -> <@ e.Message = expectedMessage @>

    [<Test>]
    let ``Calling message, as invalid validation result, returns expected message``() =
        let expectedMessage = randomGuidString()

        let result = ValidationResult.invalid (randomGuidString()) expectedMessage

        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling message, as valid validation result, raises expected exception``() =
        let expectedMessage = "Result was not invalid"

        let result = ValidationResult.valid
        raisesWith<InvalidOperationException> <@ result.Message @> <| fun e -> <@ e.Message = expectedMessage @>

    [<Test>]
    let ``Calling is valid, as valid validation result, returns true``() =
        let result = ValidationResult.valid
        test <@ result.IsValid @>

    [<Test>]
    let ``Calling is valid, as invalid validation result, returns false``() =
        let result = ValidationResult.invalid (randomGuidString()) (randomGuidString())
        test <@ not result.IsValid @>

    [<Test>]
    let ``Calling is invalid, as valid validation result, returns false``() =
        let result = ValidationResult.valid
        test <@ not result.IsInvalid @>

    [<Test>]
    let ``Calling is invalid, as invalid validation result, returns true``() =
        let result = ValidationResult.invalid (randomGuidString()) (randomGuidString())
        test <@ result.IsInvalid @>
