namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote

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
