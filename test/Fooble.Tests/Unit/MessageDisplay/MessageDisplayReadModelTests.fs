namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
 
[<TestFixture>]
module MessageDisplayReadModelTests =
 
    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let severity = MessageDisplay.informationalSeverity
        let messages = Seq.init 5 <| fun _ -> randomString ()
        let readModel = MessageDisplay.makeReadModel <||| (randomString (), severity, messages)

        test <@ box readModel :? IMessageDisplayReadModel @>

    [<Test>]
    let ``Calling heading, returns expected heading`` () =
        let expectedHeading = randomString ()

        let severity = MessageDisplay.informationalSeverity
        let messages = Seq.init 5 <| fun _ -> randomString ()
        let readModel = MessageDisplay.makeReadModel expectedHeading severity messages

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>

    [<Test>]
    let ``Calling severity, returns expected severity`` () =
        let expectedSeverity = MessageDisplay.informationalSeverity

        let messages = Seq.init 5 <| fun _ -> randomString ()
        let readModel = MessageDisplay.makeReadModel <||| (randomString (), expectedSeverity, messages)

        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>

    [<Test>]
    let ``Calling messages, returns expected messages`` () =
        let expectedMessages = List.init 5 <| fun _ -> randomString ()

        let severity = MessageDisplay.informationalSeverity
        let readModel = MessageDisplay.makeReadModel <||| (randomString (), severity, Seq.ofList expectedMessages)

        let actualMessages = Seq.toList readModel.Messages
        test <@ List.length actualMessages = 5 @>
        for current in actualMessages do
            let findResult = List.tryFind (fun x -> x = current) expectedMessages
            test <@ findResult.IsSome @>
