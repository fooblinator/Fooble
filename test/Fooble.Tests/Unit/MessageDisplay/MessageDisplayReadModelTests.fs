namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
open System
 
[<TestFixture>]
module MessageDisplayReadModelTests =

    [<Test>]
    let ``Calling make, with null heading, raises expected exception`` () =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was null"
        
        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make <||| (null, severity, Seq.singleton <| randomString ()) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>
    
    [<Test>]
    let ``Calling make, with empty heading, raises expected exception`` () =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was empty string"
        
        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make <||| (String.empty, severity, Seq.singleton <| randomString ()) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with null messages, raises expected exception`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter was null"
        
        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make <||| (randomString (), severity, null) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with empty messages, raises expected exception`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter was empty sequence"
        
        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make <||| (randomString (), severity, Seq.empty) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with not empty messages containing null message, raises expected exception`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter contained null(s)"
        
        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make <||| (randomString (), severity, Seq.singleton null) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with not empty messages containing empty message, raises expected exception`` () =
        let expectedParamName = "messages"
        let expectedMessage = "Messages parameter contained empty string(s)"
        
        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make <||| (randomString (), severity, Seq.singleton String.empty) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let severity = MessageDisplay.Severity.informational
        let messages = Seq.init 5 <| fun _ -> randomString ()
        let readModel = MessageDisplay.ReadModel.make <||| (randomString (), severity, messages)

        test <@ box readModel :? IMessageDisplayReadModel @>

    [<Test>]
    let ``Calling heading, returns expected heading`` () =
        let expectedHeading = randomString ()

        let severity = MessageDisplay.Severity.informational
        let messages = Seq.init 5 <| fun _ -> randomString ()
        let readModel = MessageDisplay.ReadModel.make expectedHeading severity messages

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>

    [<Test>]
    let ``Calling severity, returns expected severity`` () =
        let expectedSeverity = MessageDisplay.Severity.informational

        let messages = Seq.init 5 <| fun _ -> randomString ()
        let readModel = MessageDisplay.ReadModel.make <||| (randomString (), expectedSeverity, messages)

        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>

    [<Test>]
    let ``Calling messages, returns expected messages`` () =
        let expectedMessages = List.init 5 <| fun _ -> randomString ()

        let severity = MessageDisplay.Severity.informational
        let readModel = MessageDisplay.ReadModel.make <||| (randomString (), severity, Seq.ofList expectedMessages)

        let actualMessages = Seq.toList readModel.Messages
        test <@ List.length actualMessages = 5 @>
        for current in actualMessages do
            let findResult = List.tryFind (fun x -> x = current) expectedMessages
            test <@ findResult.IsSome @>
