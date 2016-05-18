namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

//[<TestFixture>]
//module ResultsTests =
//
//    [<Test>]
//    let ``Calling success, with valid value, returns expected result``() =
//        let expectedValue = randomGuidString()
//
//        let result = Result.success expectedValue
//
//        test <@ result.IsSuccess @>
//        test <@ not result.IsFailure @>
//        let actualValue = result.Value
//        test <@ actualValue = expectedValue @>
//
//    [<Test>]
//    let ``Calling failure, with valid status, returns expected result``() =
//        let expectedStatus = randomGuidString()
//
//        let result = Result.failure expectedStatus
//
//        test <@ result.IsFailure @>
//        test <@ not result.IsSuccess @>
//        let actualStatus = result.Status
//        test <@ actualStatus = expectedStatus @>
//
//    [<Test>]
//    let ``Calling value, as failure result, raises expected exception``() =
//        let expectedMessage = "Result was not a success"
//
//        let result = Result.failure (randomGuidString())
//        raisesWith<InvalidOperationException> <@ result.Value @> <| fun e -> <@ e.Message = expectedMessage @>
//
//    [<Test>]
//    let ``Calling value, as success result, returns expected value``() =
//        let expectedValue = randomGuidString()
//
//        let result = Result.success expectedValue
//        let actualValue = result.Value
//        test <@ actualValue = expectedValue @>
//
//    [<Test>]
//    let ``Calling status, as success result, raises expected exception``() =
//        let expectedMessage = "Result was not a failure"
//
//        let result = Result.success (randomGuidString())
//        raisesWith<InvalidOperationException> <@ result.Status @> <| fun e -> <@ e.Message = expectedMessage @>
//
//    [<Test>]
//    let ``Calling status, as failure result, returns expected status``() =
//        let expectedStatus = randomGuidString()
//
//        let result = Result.failure expectedStatus
//        let actualStatus = result.Status
//        test <@ actualStatus = expectedStatus @>
//
//    [<Test>]
//    let ``Calling is success, as success result, returns true``() =
//        let result = Result.success (randomGuidString())
//        test <@ result.IsSuccess @>
//
//    [<Test>]
//    let ``Calling is success, as failure result, returns false``() =
//        let result = Result.failure (randomGuidString())
//        test <@ not result.IsSuccess @>
//
//    [<Test>]
//    let ``Calling is failure, as success result, returns false``() =
//        let result = Result.success (randomGuidString())
//        test <@ not result.IsFailure @>
//
//    [<Test>]
//    let ``Calling is failure, as failure result, returns true``() =
//        let result = Result.failure (randomGuidString())
//        test <@ result.IsFailure @>
