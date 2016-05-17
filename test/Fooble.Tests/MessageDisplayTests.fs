namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MessageDisplayTests =

    [<Test>]
    let ``Calling validate heading, with null heading, returns expected result``() =
        let expectedParamName = "heading"
        let expectedMessage = "Heading was null"

        let result = MessageDisplay.validateHeading null

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate heading, with empty heading, returns expected result``() =
        let expectedParamName = "heading"
        let expectedMessage = "Heading was empty string"

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
    let ``Calling validate messages, with null messages, returns expected result``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages was null"

        let result = MessageDisplay.validateMessages null

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate messages, with empty messages, returns expected result``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages was empty sequence"

        let result = MessageDisplay.validateMessages Seq.empty

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate messages, with not empty messages containing null message, returns expected result``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages contained null(s)"

        let result = MessageDisplay.validateMessages (Seq.singleton null)

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate messages, with not empty messages containing empty message, returns expected result``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages contained empty string(s)"

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

[<TestFixture>]
module MessageDisplaySeverityTests =

    [<Test>]
    let ``Calling informational, returns expected severity``() =
        let severity = MessageDisplay.Severity.informational

        test <@ severity.IsInformational @>
        test <@ not severity.IsWarning @>
        test <@ not severity.IsError @>

    [<Test>]
    let ``Calling warning, returns expected severity``() =
        let severity = MessageDisplay.Severity.warning

        test <@ not severity.IsInformational @>
        test <@ severity.IsWarning @>
        test <@ not severity.IsError @>

    [<Test>]
    let ``Calling error, returns expected severity``() =
        let severity = MessageDisplay.Severity.error

        test <@ not severity.IsInformational @>
        test <@ not severity.IsWarning @>
        test <@ severity.IsError @>

    [<Test>]
    let ``Calling is informational, as informational severity, returns true``() =
        let severity = MessageDisplay.Severity.informational

        test <@ severity.IsInformational @>

    [<Test>]
    let ``Calling is informational, as warning severity, returns false``() =
        let severity = MessageDisplay.Severity.warning

        test <@ not severity.IsInformational @>

    [<Test>]
    let ``Calling is informational, as error severity, returns false``() =
        let severity = MessageDisplay.Severity.error

        test <@ not severity.IsInformational @>

    [<Test>]
    let ``Calling is warning, as informational severity, returns false``() =
        let severity = MessageDisplay.Severity.informational

        test <@ not severity.IsWarning @>

    [<Test>]
    let ``Calling is warning, as warning severity, returns true``() =
        let severity = MessageDisplay.Severity.warning

        test <@ severity.IsWarning @>

    [<Test>]
    let ``Calling is warning, as error severity, returns false``() =
        let severity = MessageDisplay.Severity.error

        test <@ not severity.IsWarning @>

    [<Test>]
    let ``Calling is error, as informational severity, returns false``() =
        let severity = MessageDisplay.Severity.informational

        test <@ not severity.IsError @>

    [<Test>]
    let ``Calling is error, as warning severity, returns false``() =
        let severity = MessageDisplay.Severity.warning

        test <@ not severity.IsError @>

    [<Test>]
    let ``Calling is error, as error severity, returns true``() =
        let severity = MessageDisplay.Severity.error

        test <@ severity.IsError @>

[<TestFixture>]
module MessageDisplayReadModelTests =

    [<Test>]
    let ``Calling make, with null heading, raises expected exception``() =
        let expectedParamName = "heading"
        let expectedMessage = "Heading was null"

        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make null MessageDisplay.Severity.informational (Seq.singleton (randomGuidString())) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>
    
    [<Test>]
    let ``Calling make, with empty heading, raises expected exception``() =
        let expectedParamName = "heading"
        let expectedMessage = "Heading was empty string"
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make "" MessageDisplay.Severity.informational (Seq.singleton (randomGuidString())) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with null messages, raises expected exception``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages was null"

        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (randomGuidString()) MessageDisplay.Severity.informational null @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with empty messages, raises expected exception``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages was empty sequence"

        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (randomGuidString()) MessageDisplay.Severity.informational Seq.empty @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with not empty messages containing null message, raises expected exception``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages contained null(s)"

        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (randomGuidString()) MessageDisplay.Severity.informational (Seq.singleton null) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with not empty messages containing empty message, raises expected exception``() =
        let expectedParamName = "messages"
        let expectedMessage = "Messages contained empty string(s)"

        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (randomGuidString()) MessageDisplay.Severity.informational (Seq.singleton "") @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() =
        let expectedHeading = randomGuidString()
        let expectedSeverity = MessageDisplay.Severity.informational
        let expectedMessages = [ randomGuidString() ]

        let readModel = MessageDisplay.ReadModel.make expectedHeading expectedSeverity (Seq.ofList expectedMessages)

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling heading, returns expected heading``() =
        let expectedHeading = randomGuidString()

        let readModel =
            MessageDisplay.ReadModel.make expectedHeading MessageDisplay.Severity.informational
                (Seq.singleton (randomGuidString()))

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>

    [<Test>]
    let ``Calling severity, returns expected severity``() =
        let expectedSeverity = MessageDisplay.Severity.informational

        let readModel =
            MessageDisplay.ReadModel.make (randomGuidString()) expectedSeverity
                (Seq.singleton (randomGuidString()))

        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>

    [<Test>]
    let ``Calling messages, returns expected messages``() =
        let expectedMessages = [ randomGuidString() ]

        let readModel =
            MessageDisplay.ReadModel.make (randomGuidString()) MessageDisplay.Severity.informational
                expectedMessages

        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>
