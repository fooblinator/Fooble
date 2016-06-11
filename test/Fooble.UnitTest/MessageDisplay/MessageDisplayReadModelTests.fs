namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MessageDisplayReadModelTests =

    [<Test>]
    let ``Calling make, with null heading, raises expected exception`` () =
        let expectedParamName = "heading"
        let expectedMessage = "Heading is required"

        let severity = MessageDisplayReadModel.informationalSeverity
        testArgumentException expectedParamName expectedMessage
            <@ MessageDisplayReadModel.make null (String.random 64) 200 severity (String.random 64) @>

    [<Test>]
    let ``Calling make, with empty heading, raises expected exception`` () =
        let expectedParamName = "heading"
        let expectedMessage = "Heading is required"

        let severity = MessageDisplayReadModel.informationalSeverity
        testArgumentException expectedParamName expectedMessage
            <@ MessageDisplayReadModel.make String.empty (String.random 64) 200 severity (String.random 64) @>

    [<Test>]
    let ``Calling make, with null sub-heading, raises expected exception`` () =
        let expectedParamName = "subHeading"
        let expectedMessage = "Sub-heading is required"

        let severity = MessageDisplayReadModel.informationalSeverity
        testArgumentException expectedParamName expectedMessage
            <@ MessageDisplayReadModel.make (String.random 64) null 200 severity (String.random 64) @>

    [<Test>]
    let ``Calling make, with status code less than zero, raises expected exception`` () =
        let expectedParamName = "statusCode"
        let expectedMessage = "Status code parameter is less than zero"

        let severity = MessageDisplayReadModel.informationalSeverity
        testArgumentException expectedParamName expectedMessage
            <@ MessageDisplayReadModel.make (String.random 64) (String.random 64) -1 severity (String.random 64) @>

    [<Test>]
    let ``Calling make, with null message, raises expected exception`` () =
        let expectedParamName = "message"
        let expectedMessage = "Message is required"

        let severity = MessageDisplayReadModel.informationalSeverity
        testArgumentException expectedParamName expectedMessage
            <@ MessageDisplayReadModel.make (String.random 64) (String.random 64) 200 severity null @>

    [<Test>]
    let ``Calling make, with empty message, raises expected exception`` () =
        let expectedParamName = "message"
        let expectedMessage = "Message is required"

        let severity = MessageDisplayReadModel.informationalSeverity
        testArgumentException expectedParamName expectedMessage
            <@ MessageDisplayReadModel.make (String.random 64) (String.random 64) 200 severity String.empty @>

    [<Test>]
    let ``Calling heading, returns expected heading`` () =
        let expectedHeading = String.random 64

        let readModel =
            MessageDisplayReadModel.make expectedHeading (String.random 64) 200
                MessageDisplayReadModel.informationalSeverity (String.random 64)

        readModel.Heading =! expectedHeading

    [<Test>]
    let ``Calling sub-heading, returns expected sub-heading`` () =
        let expectedSubHeading = String.random 64

        let readModel =
            MessageDisplayReadModel.make (String.random 64) expectedSubHeading 200
                MessageDisplayReadModel.informationalSeverity (String.random 64)

        readModel.SubHeading =! expectedSubHeading

    [<Test>]
    let ``Calling status code, returns expected status code`` () =
        let expectedStatusCode = 200

        let readModel =
            MessageDisplayReadModel.make (String.random 64) (String.random 64) expectedStatusCode
                MessageDisplayReadModel.informationalSeverity (String.random 64)

        readModel.StatusCode =! expectedStatusCode

    [<Test>]
    let ``Calling severity, returns expected severity`` () =
        let expectedSeverity = MessageDisplayReadModel.informationalSeverity

        let readModel =
            MessageDisplayReadModel.make (String.random 64) (String.random 64) 200 expectedSeverity (String.random 64)

        readModel.Severity =! expectedSeverity

    [<Test>]
    let ``Calling message, returns expected message`` () =
        let expectedMessage = String.random 64

        let readModel =
            MessageDisplayReadModel.make (String.random 64) (String.random 64) 200
                MessageDisplayReadModel.informationalSeverity expectedMessage

        readModel.Message =! expectedMessage
