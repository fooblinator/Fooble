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
            <@ MessageDisplay.ReadModel.make null (randomString ()) 200 severity (randomString ()) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>
    
    [<Test>]
    let ``Calling make, with empty heading, raises expected exception`` () =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was an empty string"
        
        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make String.empty (randomString ()) 200 severity (randomString ()) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with null sub-heading, raises expected exception`` () =
        let expectedParamName = "subHeading"
        let expectedMessage = "Sub-heading parameter was null"
        
        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (randomString ()) null 200 severity (randomString ()) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with status code less than zero, raises expected exception`` () =
        let expectedParamName = "statusCode"
        let expectedMessage = "Status code parameter was less than zero"
        
        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (randomString ()) (randomString ()) -1 severity (randomString ()) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with null message, raises expected exception`` () =
        let expectedParamName = "message"
        let expectedMessage = "Message parameter was null"
        
        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (randomString ()) (randomString ()) 200 severity null @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>
    
    [<Test>]
    let ``Calling make, with empty message, raises expected exception`` () =
        let expectedParamName = "message"
        let expectedMessage = "Message parameter was an empty string"
        
        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (randomString ()) (randomString ()) 200 severity String.empty @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel =
            MessageDisplay.ReadModel.make (randomString ()) (randomString ()) 200 MessageDisplay.Severity.informational
                (randomString ())

        test <@ box readModel :? IMessageDisplayReadModel @>

    [<Test>]
    let ``Calling heading, returns expected heading`` () =
        let expectedHeading = randomString ()

        let readModel =
            MessageDisplay.ReadModel.make expectedHeading (randomString ()) 200 MessageDisplay.Severity.informational
                (randomString ())

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>

    [<Test>]
    let ``Calling sub-heading, returns expected sub-heading`` () =
        let expectedSubHeading = randomString ()

        let readModel =
            MessageDisplay.ReadModel.make (randomString ()) expectedSubHeading 200 MessageDisplay.Severity.informational
                (randomString ())

        let actualSubHeading = readModel.SubHeading
        test <@ actualSubHeading = expectedSubHeading @>

    [<Test>]
    let ``Calling status code, returns expected status code`` () =
        let expectedStatusCode = 200

        let readModel =
            MessageDisplay.ReadModel.make (randomString ()) (randomString ()) expectedStatusCode
                MessageDisplay.Severity.informational (randomString ())

        let actualStatusCode = readModel.StatusCode
        test <@ actualStatusCode = expectedStatusCode @>

    [<Test>]
    let ``Calling severity, returns expected severity`` () =
        let expectedSeverity = MessageDisplay.Severity.informational

        let readModel =
            MessageDisplay.ReadModel.make (randomString ()) (randomString ()) 200 expectedSeverity (randomString ())

        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>

    [<Test>]
    let ``Calling message, returns expected message`` () =
        let expectedMessage = randomString ()

        let readModel =
            MessageDisplay.ReadModel.make (randomString ()) (randomString ()) 200 MessageDisplay.Severity.informational
                expectedMessage

        let actualMessage = readModel.Message
        test <@ actualMessage = expectedMessage @>
