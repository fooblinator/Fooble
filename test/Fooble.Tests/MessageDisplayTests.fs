namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MessageDisplaySeverityTests = 
    [<Test>]
    let ``Calling informational, returns expected severity``() = 
        let severity = MessageDisplaySeverity.informational
        test <@ (box severity) :? IMessageDisplaySeverity @>
        test <@ severity.IsInformational @>
        test <@ not severity.IsWarning @>
        test <@ not severity.IsError @>
    
    [<Test>]
    let ``Calling warning, returns expected severity``() = 
        let severity = MessageDisplaySeverity.warning
        test <@ (box severity) :? IMessageDisplaySeverity @>
        test <@ not severity.IsInformational @>
        test <@ severity.IsWarning @>
        test <@ not severity.IsError @>
    
    [<Test>]
    let ``Calling error, returns expected severity``() = 
        let severity = MessageDisplaySeverity.error
        test <@ (box severity) :? IMessageDisplaySeverity @>
        test <@ not severity.IsInformational @>
        test <@ not severity.IsWarning @>
        test <@ severity.IsError @>
    
    [<Test>]
    let ``Calling is informational, with informational severity, returns true``() = 
        let severity = MessageDisplaySeverity.informational
        test <@ severity.IsInformational @>
    
    [<Test>]
    let ``Calling is informational, with warning severity, returns false``() = 
        let severity = MessageDisplaySeverity.warning
        test <@ not severity.IsInformational @>
    
    [<Test>]
    let ``Calling is informational, with error severity, returns false``() = 
        let severity = MessageDisplaySeverity.error
        test <@ not severity.IsInformational @>
    
    [<Test>]
    let ``Calling is warning, with informational severity, returns false``() = 
        let severity = MessageDisplaySeverity.informational
        test <@ not severity.IsWarning @>
    
    [<Test>]
    let ``Calling is warning, with warning severity, returns true``() = 
        let severity = MessageDisplaySeverity.warning
        test <@ severity.IsWarning @>
    
    [<Test>]
    let ``Calling is warning, with error severity, returns false``() = 
        let severity = MessageDisplaySeverity.error
        test <@ not severity.IsWarning @>
    
    [<Test>]
    let ``Calling is error, with informational severity, returns false``() = 
        let severity = MessageDisplaySeverity.informational
        test <@ not severity.IsError @>
    
    [<Test>]
    let ``Calling is error, with warning severity, returns false``() = 
        let severity = MessageDisplaySeverity.warning
        test <@ not severity.IsError @>
    
    [<Test>]
    let ``Calling is error, with error severity, returns true``() = 
        let severity = MessageDisplaySeverity.error
        test <@ severity.IsError @>

[<TestFixture>]
module MessageDisplayReadModelTests = 
    [<Test>]
    let ``Calling validate heading, with null heading, returns expected messages``() = 
        let expectedMessage = "Heading should not be null"
        let messages = MessageDisplayReadModel.validateHeading null
        let messageCount = Seq.length messages
        test <@ messageCount = 1 @>
        let actualMessage = Seq.head messages
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate heading, with empty heading, returns expected messages``() = 
        let expectedMessage = "Heading should not be empty"
        let messages = MessageDisplayReadModel.validateHeading ""
        let messageCount = Seq.length messages
        test <@ messageCount = 1 @>
        let actualMessage = Seq.head messages
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate heading, with valid heading, returns no messages``() = 
        let messages = MessageDisplayReadModel.validateHeading (Helper.randomGuidString())
        test <@ Seq.isEmpty messages @>
    
    [<Test>]
    let ``Calling validate messages, with null messages, returns expected messages``() = 
        let expectedMessage = "Message list should not be null"
        let messages = MessageDisplayReadModel.validateMessages null
        let messageCount = Seq.length messages
        test <@ messageCount = 1 @>
        let actualMessage = Seq.head messages
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate messages, with empty messages, returns expected messages``() = 
        let expectedMessage = "Message list should not be empty"
        let messages = MessageDisplayReadModel.validateMessages Seq.empty
        let messageCount = Seq.length messages
        test <@ messageCount = 1 @>
        let actualMessage = Seq.head messages
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate messages, with not empty messages containing null message, returns expected messages``() = 
        let expectedMessage = "Message list item should not be null"
        let messages = MessageDisplayReadModel.validateMessages (Seq.singleton null)
        let messageCount = Seq.length messages
        test <@ messageCount = 1 @>
        let actualMessage = Seq.head messages
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate messages, with not empty messages containing empty message, returns expected messages``() = 
        let expectedMessage = "Message list item should not be empty"
        let messages = MessageDisplayReadModel.validateMessages (Seq.singleton "")
        let messageCount = Seq.length messages
        test <@ messageCount = 1 @>
        let actualMessage = Seq.head messages
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate messages, with valid messages, returns no messages``() = 
        let messages = MessageDisplayReadModel.validateMessages (Seq.singleton (Helper.randomGuidString()))
        test <@ Seq.isEmpty messages @>
    
    [<Test>]
    let ``Calling make, with null heading, raises expected exception``() = 
        let expectedParamName = "heading"
        let expectedMessage = "Heading should not be null"
        raisesWith<ArgumentException> 
            <@ MessageDisplayReadModel.make null MessageDisplaySeverity.informational 
                   (Seq.singleton (Helper.randomGuidString())) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with empty heading, raises expected exception``() = 
        let expectedParamName = "heading"
        let expectedMessage = "Heading should not be empty"
        raisesWith<ArgumentException> 
            <@ MessageDisplayReadModel.make "" MessageDisplaySeverity.informational 
                   (Seq.singleton (Helper.randomGuidString())) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with null messages, raises expected exception``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list should not be null"
        raisesWith<ArgumentException> 
            <@ MessageDisplayReadModel.make (Helper.randomGuidString()) MessageDisplaySeverity.informational null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with empty messages, raises expected exception``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list should not be empty"
        raisesWith<ArgumentException> 
            <@ MessageDisplayReadModel.make (Helper.randomGuidString()) MessageDisplaySeverity.informational Seq.empty @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with not empty messages containing null message, raises expected exception``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list item should not be null"
        raisesWith<ArgumentException> 
            <@ MessageDisplayReadModel.make (Helper.randomGuidString()) MessageDisplaySeverity.informational 
                   (Seq.singleton null) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with not empty messages containing empty message, raises expected exception``() = 
        let expectedParamName = "messages"
        let expectedMessage = "Message list item should not be empty"
        raisesWith<ArgumentException> 
            <@ MessageDisplayReadModel.make (Helper.randomGuidString()) MessageDisplaySeverity.informational 
                   (Seq.singleton "") @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() = 
        let expectedHeading = Helper.randomGuidString()
        let expectedSeverity = MessageDisplaySeverity.informational
        let expectedMessages = Seq.singleton (Helper.randomGuidString())
        let readModel = MessageDisplayReadModel.make expectedHeading expectedSeverity expectedMessages
        test <@ (box readModel) :? IMessageDisplayReadModel @>
        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = readModel.Messages
        test <@ actualMessages = expectedMessages @>
    
    [<Test>]
    let ``Calling heading, returns expected heading``() = 
        let expectedHeading = Helper.randomGuidString()
        let readModel = 
            MessageDisplayReadModel.make expectedHeading MessageDisplaySeverity.informational 
                (Seq.singleton (Helper.randomGuidString()))
        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
    
    [<Test>]
    let ``Calling severity, returns expected severity``() = 
        let expectedSeverity = MessageDisplaySeverity.informational
        let readModel = 
            MessageDisplayReadModel.make (Helper.randomGuidString()) expectedSeverity 
                (Seq.singleton (Helper.randomGuidString()))
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
    
    [<Test>]
    let ``Calling messages, returns expected messages``() = 
        let expectedMessages = Seq.singleton (Helper.randomGuidString())
        let readModel = 
            MessageDisplayReadModel.make (Helper.randomGuidString()) MessageDisplaySeverity.informational 
                expectedMessages
        let actualMessages = readModel.Messages
        test <@ actualMessages = expectedMessages @>
