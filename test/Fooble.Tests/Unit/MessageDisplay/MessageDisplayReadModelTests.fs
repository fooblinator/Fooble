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
            <@ MessageDisplay.ReadModel.make null (String.random 64) 200 severity (String.random 64) @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with empty heading, raises expected exception`` () =
        let expectedParamName = "heading"
        let expectedMessage = "Heading parameter was an empty string"

        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make String.empty (String.random 64) 200 severity (String.random 64) @>
                (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with null sub-heading, raises expected exception`` () =
        let expectedParamName = "subHeading"
        let expectedMessage = "Sub-heading parameter was null"

        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (String.random 64) null 200 severity (String.random 64) @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with status code less than zero, raises expected exception`` () =
        let expectedParamName = "statusCode"
        let expectedMessage = "Status code parameter was less than zero"

        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (String.random 64) (String.random 64) -1 severity (String.random 64) @>
                (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with null message, raises expected exception`` () =
        let expectedParamName = "message"
        let expectedMessage = "Message parameter was null"

        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (String.random 64) (String.random 64) 200 severity null @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with empty message, raises expected exception`` () =
        let expectedParamName = "message"
        let expectedMessage = "Message parameter was an empty string"

        let severity = MessageDisplay.Severity.informational
        raisesWith<ArgumentException>
            <@ MessageDisplay.ReadModel.make (String.random 64) (String.random 64) 200 severity String.empty @>
                (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel =
            MessageDisplay.ReadModel.make (String.random 64) (String.random 64) 200
                MessageDisplay.Severity.informational (String.random 64)

        test <@ box readModel :? IMessageDisplayReadModel @>

    [<Test>]
    let ``Calling heading, returns expected heading`` () =
        let expectedHeading = String.random 64

        let readModel =
            MessageDisplay.ReadModel.make expectedHeading (String.random 64) 200
                MessageDisplay.Severity.informational (String.random 64)

        test <@ readModel.Heading = expectedHeading @>

    [<Test>]
    let ``Calling sub-heading, returns expected sub-heading`` () =
        let expectedSubHeading = String.random 64

        let readModel =
            MessageDisplay.ReadModel.make (String.random 64) expectedSubHeading 200
                MessageDisplay.Severity.informational (String.random 64)

        test <@ readModel.SubHeading = expectedSubHeading @>

    [<Test>]
    let ``Calling status code, returns expected status code`` () =
        let expectedStatusCode = 200

        let readModel =
            MessageDisplay.ReadModel.make (String.random 64) (String.random 64) expectedStatusCode
                MessageDisplay.Severity.informational (String.random 64)

        test <@ readModel.StatusCode = expectedStatusCode @>

    [<Test>]
    let ``Calling severity, returns expected severity`` () =
        let expectedSeverity = MessageDisplay.Severity.informational

        let readModel =
            MessageDisplay.ReadModel.make (String.random 64) (String.random 64) 200 expectedSeverity (String.random 64)

        test <@ readModel.Severity = expectedSeverity @>

    [<Test>]
    let ``Calling message, returns expected message`` () =
        let expectedMessage = String.random 64

        let readModel =
            MessageDisplay.ReadModel.make (String.random 64) (String.random 64) 200
                MessageDisplay.Severity.informational expectedMessage

        test <@ readModel.Message = expectedMessage @>
