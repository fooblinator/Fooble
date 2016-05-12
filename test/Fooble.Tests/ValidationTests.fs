namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module ValidationFailureInfoTests = 
    [<Test>]
    let ``Calling make failure info, with null param name, raises expected exception``() = 
        let expectedParamName = "paramName"
        let expectedMessage = "Param name should not be null"
        raisesWith<ArgumentException> <@ Validation.makeFailureInfo null (Helper.randomGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make failure info, with empty param name, raises expected exception``() = 
        let expectedParamName = "paramName"
        let expectedMessage = "Param name should not be empty"
        raisesWith<ArgumentException> <@ Validation.makeFailureInfo "" (Helper.randomGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make failure info, with null message, raises expected exception``() = 
        let expectedParamName = "message"
        let expectedMessage = "Message should not be null"
        raisesWith<ArgumentException> <@ Validation.makeFailureInfo (Helper.randomGuidString()) null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make failure info, with empty message, raises expected exception``() = 
        let expectedParamName = "message"
        let expectedMessage = "Message should not be empty"
        raisesWith<ArgumentException> <@ Validation.makeFailureInfo (Helper.randomGuidString()) "" @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make failure info, with valid parameters, returns expected result``() = 
        let expectedParamName = Helper.randomGuidString()
        let expectedMessage = Helper.randomGuidString()
        let result = Validation.makeFailureInfo expectedParamName expectedMessage
        test <@ (box result) :? IValidationFailureInfo @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling param name, returns expected param name``() = 
        let expectedParamName = Helper.randomGuidString()
        let result = Validation.makeFailureInfo expectedParamName (Helper.randomGuidString())
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
    
    [<Test>]
    let ``Calling message, returns expected message``() = 
        let expectedMessage = Helper.randomGuidString()
        let result = Validation.makeFailureInfo (Helper.randomGuidString()) expectedMessage
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>
