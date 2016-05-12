namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MessageDisplaySeverityTests = 
    [<Test>]
    let ``Calling informational, returns expected severity``() = 
        let severity = MessageDisplay.informationalSeverity
        test <@ (box severity) :? IMessageDisplaySeverity @>
        test <@ severity.IsInformational @>
        test <@ not severity.IsWarning @>
        test <@ not severity.IsError @>
    
    [<Test>]
    let ``Calling warning, returns expected severity``() = 
        let severity = MessageDisplay.warningSeverity
        test <@ (box severity) :? IMessageDisplaySeverity @>
        test <@ not severity.IsInformational @>
        test <@ severity.IsWarning @>
        test <@ not severity.IsError @>
    
    [<Test>]
    let ``Calling error, returns expected severity``() = 
        let severity = MessageDisplay.errorSeverity
        test <@ (box severity) :? IMessageDisplaySeverity @>
        test <@ not severity.IsInformational @>
        test <@ not severity.IsWarning @>
        test <@ severity.IsError @>
    
    [<Test>]
    let ``Calling is informational, as informational, returns true``() = 
        let severity = MessageDisplay.informationalSeverity
        test <@ severity.IsInformational @>
    
    [<Test>]
    let ``Calling is informational, as warning, returns false``() = 
        let severity = MessageDisplay.warningSeverity
        test <@ not severity.IsInformational @>
    
    [<Test>]
    let ``Calling is informational, as error, returns false``() = 
        let severity = MessageDisplay.errorSeverity
        test <@ not severity.IsInformational @>
    
    [<Test>]
    let ``Calling is warning, as informational, returns false``() = 
        let severity = MessageDisplay.informationalSeverity
        test <@ not severity.IsWarning @>
    
    [<Test>]
    let ``Calling is warning, as warning, returns true``() = 
        let severity = MessageDisplay.warningSeverity
        test <@ severity.IsWarning @>
    
    [<Test>]
    let ``Calling is warning, as error, returns false``() = 
        let severity = MessageDisplay.errorSeverity
        test <@ not severity.IsWarning @>
    
    [<Test>]
    let ``Calling is error, as informational, returns false``() = 
        let severity = MessageDisplay.informationalSeverity
        test <@ not severity.IsError @>
    
    [<Test>]
    let ``Calling is error, as warning, returns false``() = 
        let severity = MessageDisplay.warningSeverity
        test <@ not severity.IsError @>
    
    [<Test>]
    let ``Calling is error, as error, returns true``() = 
        let severity = MessageDisplay.errorSeverity
        test <@ severity.IsError @>

[<TestFixture>]
module MessageDisplayReadModelTests = 
    [<Test>]
    let ``Calling validate heading, with null heading, returns expected result``() = 
        let expectedParamName = "heading"
        let expectedMessage = "Heading should not be null"
        let result = MessageDisplay.validateHeading null
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate heading, with empty heading, returns expected result``() = 
        let expectedParamName = "heading"
        let expectedMessage = "Heading should not be empty"
        let result = MessageDisplay.validateHeading ""
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate heading, with valid heading, returns no messages``() = 
        let result = MessageDisplay.validateHeading (Helper.randomGuidString())
        test <@ result.IsNone @>
    
    [<Test>]
    let ``Calling validate messages, with null messages, returns expected result``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list should not be null"
        let result = MessageDisplay.validateMessages null
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate messages, with empty messages, returns expected result``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list should not be empty"
        let result = MessageDisplay.validateMessages Seq.empty
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate messages, with not empty messages containing null message, returns expected result``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list items should not be null"
        let result = MessageDisplay.validateMessages (Seq.singleton null)
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate messages, with not empty messages containing empty message, returns expected result``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list items should not be empty"
        let result = MessageDisplay.validateMessages (Seq.singleton "")
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate messages, with valid messages, returns no messages``() = 
        let result = MessageDisplay.validateMessages (Seq.singleton (Helper.randomGuidString()))
        test <@ result.IsNone @>
    
    [<Test>]
    let ``Calling make read model, with null heading, raises expected exception``() = 
        let expectedParamName = "heading"
        let expectedMessage = "Heading should not be null"
        raisesWith<ArgumentException> 
            <@ MessageDisplay.makeReadModel null MessageDisplay.informationalSeverity 
                   (Seq.singleton (Helper.randomGuidString())) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make read model, with empty heading, raises expected exception``() = 
        let expectedParamName = "heading"
        let expectedMessage = "Heading should not be empty"
        raisesWith<ArgumentException> 
            <@ MessageDisplay.makeReadModel "" MessageDisplay.informationalSeverity 
                   (Seq.singleton (Helper.randomGuidString())) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make read model, with null messages, raises expected exception``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list should not be null"
        raisesWith<ArgumentException> 
            <@ MessageDisplay.makeReadModel (Helper.randomGuidString()) MessageDisplay.informationalSeverity null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make read model, with empty messages, raises expected exception``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list should not be empty"
        raisesWith<ArgumentException> 
            <@ MessageDisplay.makeReadModel (Helper.randomGuidString()) MessageDisplay.informationalSeverity Seq.empty @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make read model, with not empty messages containing null message, raises expected exception``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list items should not be null"
        raisesWith<ArgumentException> 
            <@ MessageDisplay.makeReadModel (Helper.randomGuidString()) MessageDisplay.informationalSeverity 
                   (Seq.singleton null) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make read model, with not empty messages containing empty message, raises expected exception``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list items should not be empty"
        raisesWith<ArgumentException> 
            <@ MessageDisplay.makeReadModel (Helper.randomGuidString()) MessageDisplay.informationalSeverity 
                   (Seq.singleton "") @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make read model, with valid parameters, returns expected result``() = 
        let expectedHeading = Helper.randomGuidString()
        let expectedSeverity = MessageDisplay.informationalSeverity
        let expectedMessages = [ Helper.randomGuidString() ]
        let readModel = MessageDisplay.makeReadModel expectedHeading expectedSeverity (Seq.ofList expectedMessages)
        test <@ (box readModel) :? IMessageDisplayReadModel @>
        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>
    
    [<Test>]
    let ``Calling heading, returns expected heading``() = 
        let expectedHeading = Helper.randomGuidString()
        let readModel = 
            MessageDisplay.makeReadModel expectedHeading MessageDisplay.informationalSeverity 
                (Seq.singleton (Helper.randomGuidString()))
        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
    
    [<Test>]
    let ``Calling severity, returns expected severity``() = 
        let expectedSeverity = MessageDisplay.informationalSeverity
        let readModel = 
            MessageDisplay.makeReadModel (Helper.randomGuidString()) expectedSeverity 
                (Seq.singleton (Helper.randomGuidString()))
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
    
    [<Test>]
    let ``Calling messages, returns expected messages``() = 
        let expectedMessages = [ Helper.randomGuidString() ]
        let readModel = 
            MessageDisplay.makeReadModel (Helper.randomGuidString()) MessageDisplay.informationalSeverity 
                expectedMessages
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>
