namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MessageDisplayTests =

    [<Test>]
    let ``Calling validate heading, with null heading, returns expected validation result``() =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was null"

        let result = MessageDisplay.validateHeading null

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate heading, with empty heading, returns expected validation result``() =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was empty string"

        let result = MessageDisplay.validateHeading ""

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate heading, with valid heading, returns no messages``() =
        let result = MessageDisplay.validateHeading (randomGuidString())

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling validate messages, with null messages, returns expected validation result``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter was null"

        let result = MessageDisplay.validateMessages null

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate messages, with empty messages, returns expected validation result``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter was empty sequence"

        let result = MessageDisplay.validateMessages Seq.empty

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate messages, with not empty messages containing null message, returns expected validation result``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter contained null(s)"

        let result = MessageDisplay.validateMessages (Seq.singleton null)

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate messages, with not empty messages containing empty message, returns expected validation result``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter contained empty string(s)"

        let result = MessageDisplay.validateMessages (Seq.singleton "")

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate messages, with valid messages, returns no messages``() =
        let result = MessageDisplay.validateMessages (Seq.singleton (randomGuidString()))

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling make read model, with null heading, raises expected exception``() =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was null"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel null severity (Seq.singleton (randomGuidString())) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>
    
    [<Test>]
    let ``Calling make read model, with empty heading, raises expected exception``() =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was empty string"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel "" severity (Seq.singleton (randomGuidString())) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make read model, with null messages, raises expected exception``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter was null"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel (randomGuidString()) severity null @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make read model, with empty messages, raises expected exception``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter was empty sequence"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel (randomGuidString()) severity Seq.empty @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make read model, with not empty messages containing null message, raises expected exception``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter contained null(s)"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel (randomGuidString()) severity (Seq.singleton null) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make read model, with not empty messages containing empty message, raises expected exception``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter contained empty string(s)"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel (randomGuidString()) severity (Seq.singleton "") @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make read model, with valid parameters, returns read model``() =
        let severity = MessageDisplay.informationalSeverity
        let messages = Seq.init 5 (fun _ -> randomGuidString())
        let readModel = MessageDisplay.makeReadModel (randomGuidString()) severity messages

        test <@ box readModel :? IMessageDisplayReadModel @>

    [<Test>]
    let ``Calling informational severity, returns severity``() =
        let severity = MessageDisplay.informationalSeverity

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling warning severity, returns severity``() =
        let severity = MessageDisplay.warningSeverity

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling error severity, returns severity``() =
        let severity = MessageDisplay.errorSeverity

        test <@ box severity :? IMessageDisplaySeverity @>

[<TestFixture>]
module MessageDisplaySeverityTests =

    [<Test>]
    let ``Calling make informational, returns severity``() =
        let severity = MessageDisplay.informationalSeverity

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling make warning, returns severity``() =
        let severity = MessageDisplay.warningSeverity

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling make error, returns severity``() =
        let severity = MessageDisplay.errorSeverity

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling is informational, with informational severity, returns true``() =
        let severity = MessageDisplay.informationalSeverity

        test <@ severity.IsInformational @>

    [<Test>]
    let ``Calling is informational, with warning severity, returns false``() =
        let severity = MessageDisplay.warningSeverity

        test <@ not <| severity.IsInformational @>

    [<Test>]
    let ``Calling is informational, with error severity, returns false``() =
        let severity = MessageDisplay.errorSeverity

        test <@ not <| severity.IsInformational @>

    [<Test>]
    let ``Calling is warning, with informational severity, returns false``() =
        let severity = MessageDisplay.informationalSeverity

        test <@ not <| severity.IsWarning @>

    [<Test>]
    let ``Calling is warning, with warning severity, returns true``() =
        let severity = MessageDisplay.warningSeverity

        test <@ severity.IsWarning @>

    [<Test>]
    let ``Calling is warning, with error severity, returns false``() =
        let severity = MessageDisplay.errorSeverity

        test <@ not <| severity.IsWarning @>

    [<Test>]
    let ``Calling is error, with informational severity, returns false``() =
        let severity = MessageDisplay.informationalSeverity

        test <@ not <| severity.IsError @>

    [<Test>]
    let ``Calling is error, with warning severity, returns false``() =
        let severity = MessageDisplay.warningSeverity

        test <@ not <| severity.IsError @>

    [<Test>]
    let ``Calling is error, with error severity, returns true``() =
        let severity = MessageDisplay.errorSeverity

        test <@ severity.IsError @>
 
[<TestFixture>]
module MessageDisplayReadModelTests =
 
    [<Test>]
    let ``Calling make, with valid parameters, returns read model``() =
        let severity = MessageDisplay.informationalSeverity
        let messages = Seq.init 5 (fun _ -> randomGuidString())
        let readModel = MessageDisplay.makeReadModel (randomGuidString()) severity messages

        test <@ box readModel :? IMessageDisplayReadModel @>

    [<Test>]
    let ``Calling heading, returns expected heading``() =
        let expectedHeading = randomGuidString()

        let severity = MessageDisplay.informationalSeverity
        let messages = Seq.init 5 (fun _ -> randomGuidString())
        let readModel = MessageDisplay.makeReadModel expectedHeading severity messages

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>

    [<Test>]
    let ``Calling severity, returns expected severity``() =
        let expectedSeverity = MessageDisplay.informationalSeverity

        let messages = Seq.init 5 (fun _ -> randomGuidString())
        let readModel = MessageDisplay.makeReadModel (randomGuidString()) expectedSeverity messages

        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>

    [<Test>]
    let ``Calling messages, returns expected messages``() =
        let expectedMessages = List.init 5 (fun _ -> randomGuidString())

        let severity = MessageDisplay.informationalSeverity
        let readModel = MessageDisplay.makeReadModel (randomGuidString()) severity (Seq.ofList expectedMessages)

        let actualMessages = Seq.toList readModel.Messages
        test <@ List.length actualMessages = 5 @>
        for current in actualMessages do
            let findResult = List.tryFind (fun x -> x = current) expectedMessages
            test <@ findResult.IsSome @>
