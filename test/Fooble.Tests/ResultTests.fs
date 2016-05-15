namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module ResultsTests =

    [<Test>]
    let ``Calling success, with valid value, returns expected result``() =
        let expectedValue = randomGuidString()

        let result = successResult expectedValue

        test <@ result.IsSuccess @>
        test <@ not result.IsFailure @>
        let actualValue = result.Value
        test <@ actualValue = expectedValue @>

    [<Test>]
    let ``Calling failure, with valid status, returns expected result``() =
        let expectedStatus = randomGuidString()

        let result = failureResult expectedStatus

        test <@ result.IsFailure @>
        test <@ not result.IsSuccess @>
        let actualStatus = result.Status
        test <@ actualStatus = expectedStatus @>

    [<Test>]
    let ``Calling value, as failure, raises expected exception``() =
        let expectedMessage = "Result was not a success"

        let result = failureResult (randomGuidString())
        raisesWith<InvalidOperationException> <@ result.Value @> <| fun e -> <@ e.Message = expectedMessage @>

    [<Test>]
    let ``Calling value, as success, returns expected value``() =
        let expectedValue = randomGuidString()

        let result = successResult expectedValue
        let actualValue = result.Value
        test <@ actualValue = expectedValue @>

    [<Test>]
    let ``Calling status, as success, raises expected exception``() =
        let expectedMessage = "Result was not a failure"

        let result = successResult (randomGuidString())
        raisesWith<InvalidOperationException> <@ result.Status @> <| fun e -> <@ e.Message = expectedMessage @>

    [<Test>]
    let ``Calling status, as failure, returns expected status``() =
        let expectedStatus = randomGuidString()

        let result = failureResult expectedStatus
        let actualStatus = result.Status
        test <@ actualStatus = expectedStatus @>

    [<Test>]
    let ``Calling is success, as success, returns true``() =
        let result = successResult (randomGuidString())
        test <@ result.IsSuccess @>

    [<Test>]
    let ``Calling is success, as failure, returns false``() =
        let result = failureResult (randomGuidString())
        test <@ not result.IsSuccess @>

    [<Test>]
    let ``Calling is failure, as success, returns false``() =
        let result = successResult (randomGuidString())
        test <@ not result.IsFailure @>

    [<Test>]
    let ``Calling is failure, as failure, returns true``() =
        let result = failureResult (randomGuidString())
        test <@ result.IsFailure @>
