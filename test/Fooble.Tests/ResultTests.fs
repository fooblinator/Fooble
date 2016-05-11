namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module ResultsTests = 
    [<Test>]
    let ``Calling validate value, with null value, returns expected result``() = 
        let expectedParamName = "value"
        let expectedMessage = "Value should not be null"
        let result = Result.validateValue null
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate value, with valid value, returns no message``() = 
        let result = Result.validateValue (Helper.randomGuidString())
        test <@ result.IsNone @>
    
    [<Test>]
    let ``Calling validate status, with null status, returns expected result``() = 
        let expectedParamName = "status"
        let expectedMessage = "Status should not be null"
        let result = Result.validateStatus null
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate status, with valid status, returns no message``() = 
        let result = Result.validateStatus (Helper.randomGuidString())
        test <@ result.IsNone @>
    
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
    let ``Calling value, with failure result, raises expected exception``() = 
        let expectedMessage = "Result was not a success"
        let result = Result.failure (Helper.randomGuidString())
        raisesWith<InvalidOperationException> <@ result.Value @> (fun e -> <@ e.Message = expectedMessage @>)
    
    [<Test>]
    let ``Calling value, with success result, returns expected value``() = 
        let expectedValue = Helper.randomGuidString()
        let result = Result.success expectedValue
        let actualValue = result.Value
        test <@ actualValue = expectedValue @>
    
    [<Test>]
    let ``Calling status, with success result, raises expected exception``() = 
        let expectedMessage = "Result was not a failure"
        let result = Result.success (Helper.randomGuidString())
        raisesWith<InvalidOperationException> <@ result.Status @> (fun e -> <@ e.Message = expectedMessage @>)
    
    [<Test>]
    let ``Calling status, with failure result, returns expected status``() = 
        let expectedStatus = Helper.randomGuidString()
        let result = Result.failure expectedStatus
        let actualStatus = result.Status
        test <@ actualStatus = expectedStatus @>
    
    [<Test>]
    let ``Calling is success, with success result, returns true``() = 
        let result = Result.success (Helper.randomGuidString())
        test <@ result.IsSuccess @>
    
    [<Test>]
    let ``Calling is success, with failure result, returns false``() = 
        let result = Result.failure (Helper.randomGuidString())
        test <@ not result.IsSuccess @>
    
    [<Test>]
    let ``Calling is failure, with success result, returns false``() = 
        let result = Result.success (Helper.randomGuidString())
        test <@ not result.IsFailure @>
    
    [<Test>]
    let ``Calling is failure, with failure result, returns true``() = 
        let result = Result.failure (Helper.randomGuidString())
        test <@ result.IsFailure @>
