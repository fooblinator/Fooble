namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module ResultsTests = 
    [<Test>]
    let ``Calling success, with null value, raises expected exception``() = 
        let expectedParamName = "value"
        let expectedMessage = "Value should not be null"
        raisesWith<ArgumentException> <@ Result.success null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling success, with valid value, returns expected result``() = 
        let expectedValue = Helper.randomGuidString()
        let result = Result.success expectedValue
        test <@ (box result) :? IResult<string, _> @>
        test <@ result.IsSuccess @>
        test <@ not result.IsFailure @>
        let actualValue = result.Value
        test <@ actualValue = expectedValue @>
    
    [<Test>]
    let ``Calling failure, with null status, raises expected exception``() = 
        let expectedParamName = "status"
        let expectedMessage = "Status should not be null"
        raisesWith<ArgumentException> <@ Result.failure null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling failure, with valid status, returns expected result``() = 
        let expectedStatus = Helper.randomGuidString()
        let result = Result.failure expectedStatus
        test <@ (box result) :? IResult<_, string> @>
        test <@ result.IsFailure @>
        test <@ not result.IsSuccess @>
        let actualStatus = result.Status
        test <@ actualStatus = expectedStatus @>
    
    [<Test>]
    let ``Calling value, as failure, raises expected exception``() = 
        let expectedMessage = "Result was not a success"
        let result = Result.failure (Helper.randomGuidString())
        raisesWith<InvalidOperationException> <@ result.Value @> (fun e -> <@ e.Message = expectedMessage @>)
    
    [<Test>]
    let ``Calling value, as success, returns expected value``() = 
        let expectedValue = Helper.randomGuidString()
        let result = Result.success expectedValue
        let actualValue = result.Value
        test <@ actualValue = expectedValue @>
    
    [<Test>]
    let ``Calling status, as success, raises expected exception``() = 
        let expectedMessage = "Result was not a failure"
        let result = Result.success (Helper.randomGuidString())
        raisesWith<InvalidOperationException> <@ result.Status @> (fun e -> <@ e.Message = expectedMessage @>)
    
    [<Test>]
    let ``Calling status, as failure, returns expected status``() = 
        let expectedStatus = Helper.randomGuidString()
        let result = Result.failure expectedStatus
        let actualStatus = result.Status
        test <@ actualStatus = expectedStatus @>
    
    [<Test>]
    let ``Calling is success, as success, returns true``() = 
        let result = Result.success (Helper.randomGuidString())
        test <@ result.IsSuccess @>
    
    [<Test>]
    let ``Calling is success, as failure, returns false``() = 
        let result = Result.failure (Helper.randomGuidString())
        test <@ not result.IsSuccess @>
    
    [<Test>]
    let ``Calling is failure, as success, returns false``() = 
        let result = Result.success (Helper.randomGuidString())
        test <@ not result.IsFailure @>
    
    [<Test>]
    let ``Calling is failure, as failure, returns true``() = 
        let result = Result.failure (Helper.randomGuidString())
        test <@ result.IsFailure @>
