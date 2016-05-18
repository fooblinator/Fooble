namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module ValidationQueryResultTests =

    [<Test>]
    let ``Calling valid, returns validation result``() =
        let result = Validation.validResult

        test <@ box result :? IValidationResult @>

    [<Test>]
    let ``Calling make invalid, with valid parameters, returns validation result``() =
        let result = Validation.makeInvalidResult (randomGuidString()) (randomGuidString())

        test <@ box result :? IValidationResult @>

    [<Test>]
    let ``Calling param name, with invalid validation result, returns expected param name``() =
        let expectedParamName = randomGuidString()

        let result = Validation.makeInvalidResult expectedParamName (randomGuidString())

        test <@ result.ParamName = expectedParamName @>

    [<Test>]
    let ``Calling message, with invalid validation result, returns expected message``() =
        let expectedMessage = randomGuidString()

        let result = Validation.makeInvalidResult (randomGuidString()) expectedMessage

        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling is valid, with valid result, returns true``() =
        let result = Validation.validResult

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling is valid, with invalid result, returns false``() =
        let result = Validation.makeInvalidResult (randomGuidString()) (randomGuidString())

        test <@ not <| result.IsValid @>

    [<Test>]
    let ``Calling is invalid, with valid result, returns false``() =
        let result = Validation.validResult

        test <@ not <| result.IsInvalid @>

    [<Test>]
    let ``Calling is invalid, with invalid result, returns true``() =
        let result = Validation.makeInvalidResult (randomGuidString()) (randomGuidString())

        test <@ result.IsInvalid @>
