namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MessageDisplayTests =

    [<Test>]
    let ``Calling validate heading, with null heading, returns expected validation result`` () =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was null"

        let result = MessageDisplay.validateHeading null

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate heading, with empty heading, returns expected validation result`` () =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was empty string"

        let result = MessageDisplay.validateHeading String.empty

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate heading, with valid heading, returns no messages`` () =
        let result = MessageDisplay.validateHeading <| randomString ()

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling validate messages, with null messages, returns expected validation result`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter was null"

        let result = MessageDisplay.validateMessages null

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate messages, with empty messages, returns expected validation result`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter was empty sequence"

        let result = MessageDisplay.validateMessages Seq.empty

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate messages, with not empty messages containing null message, returns expected validation result`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter contained null(s)"

        let result = MessageDisplay.validateMessages <| Seq.singleton null

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate messages, with not empty messages containing empty message, returns expected validation result`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter contained empty string(s)"

        let result = MessageDisplay.validateMessages <| Seq.singleton String.empty

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate messages, with valid messages, returns no messages`` () =
        let result = MessageDisplay.validateMessages (Seq.singleton <| randomString ())

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling make read model, with null heading, raises expected exception`` () =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was null"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel <||| (null, severity, Seq.singleton <| randomString ()) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>
    
    [<Test>]
    let ``Calling make read model, with empty heading, raises expected exception`` () =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was empty string"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel <||| (String.empty, severity, Seq.singleton <| randomString ()) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make read model, with null messages, raises expected exception`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter was null"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel <||| (randomString (), severity, null) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make read model, with empty messages, raises expected exception`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter was empty sequence"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel <||| (randomString (), severity, Seq.empty) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make read model, with not empty messages containing null message, raises expected exception`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter contained null(s)"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel <||| (randomString (), severity, Seq.singleton null) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make read model, with not empty messages containing empty message, raises expected exception`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter contained empty string(s)"
        
        let severity = MessageDisplay.informationalSeverity
        raisesWith<ArgumentException>
            <@ MessageDisplay.makeReadModel <||| (randomString (), severity, Seq.singleton String.empty) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make read model, with valid parameters, returns read model`` () =
        let severity = MessageDisplay.informationalSeverity
        let messages = Seq.init 5 <| fun _ -> randomString ()
        let readModel = MessageDisplay.makeReadModel <||| (randomString (), severity, messages)

        test <@ box readModel :? IMessageDisplayReadModel @>

    [<Test>]
    let ``Calling informational severity, returns severity`` () =
        let severity = MessageDisplay.informationalSeverity

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling warning severity, returns severity`` () =
        let severity = MessageDisplay.warningSeverity

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling error severity, returns severity`` () =
        let severity = MessageDisplay.errorSeverity

        test <@ box severity :? IMessageDisplaySeverity @>
