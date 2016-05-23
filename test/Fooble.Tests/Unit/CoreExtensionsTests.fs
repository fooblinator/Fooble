namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module CoreExtensionsTests =

    [<Test>]
    let ``Calling to message display read model, as success result of member detail query result, returns expected read model`` () =
        let expectedHeading = "Member Detail"
        let expectedSeverity = MessageDisplay.Severity.informational
        let expectedMessages = [ "Member detail query was successful" ]

        let readModel =
            MemberDetail.ReadModel.make <|| (randomGuid (), randomString ())
            |> MemberDetail.QueryResult.makeSuccess
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling to message display read model, as not found result of member detail query result, returns expected read model`` () =
        let expectedHeading = "Member Detail"
        let expectedSeverity = MessageDisplay.Severity.error
        let expectedMessages = [ "Member detail query was not successful and returned \"not found\"" ]

        let readModel =
            MemberDetail.QueryResult.notFound
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling to message display read model, as success result with member list query result, returns expected read model`` () =
        let expectedHeading = "Member List"
        let expectedSeverity = MessageDisplay.Severity.informational
        let expectedMessages = [ "Member list query was successful" ]

        let readModel =
            MemberList.ItemReadModel.make <|| (randomGuid (), randomString ())
            |> Seq.singleton
            |> MemberList.ReadModel.make
            |> MemberList.QueryResult.makeSuccess
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling to message display read model, as not found result with member list query result, returns expected read model`` () =
        let expectedHeading = "Member List"
        let expectedSeverity = MessageDisplay.Severity.error
        let expectedMessages = [ "Member list query was not successful and returned \"not found\"" ]

        let readModel =
            MemberList.QueryResult.notFound
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling to message display read model, as valid result of validation result, returns expected read model`` () =
        let expectedHeading = "Validation"
        let expectedSeverity = MessageDisplay.Severity.informational
        let expectedMessages = [ "Validation was successful" ]

        let readModel =
            Validation.Result.valid
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling to message display read model, as invalid result of validation result, returns expected read model`` () =
        let expectedHeading = "Validation"
        let expectedSeverity = MessageDisplay.Severity.error
        let expectedMessage = randomString ()
        let expectedMessages = [ sprintf "Validation was not successful and returned: \"%s\"" expectedMessage ]

        let readModel =
            Validation.Result.makeInvalid <|| (randomString (), expectedMessage)
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>
