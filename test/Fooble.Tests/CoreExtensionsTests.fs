namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module CoreExtensionsTests =

    [<Test>]
    let ``Calling to message display read model, as success result of member detail read model, returns expected result`` () =
        let expectedHeading = "Member Detail Query"
        let expectedSeverity = MessageDisplay.informationalSeverity
        let expectedMessages = [ "Member detail query was successful" ]

        let readModel =
            MemberDetail.makeReadModel <|| (randomGuid (), randomString ())
            |> MemberDetail.makeSuccessResult
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling to message display read model, as not found result of member detail read model, returns expected result`` () =
        let expectedHeading = "Member Detail Query"
        let expectedSeverity = MessageDisplay.errorSeverity
        let expectedMessages = [ "Member detail query was not successful and returned not found" ]

        let readModel =
            MemberDetail.notFoundResult
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling to message display read model, as success result with member list read model, returns expected result`` () =
        let expectedHeading = "Member List Query"
        let expectedSeverity = MessageDisplay.informationalSeverity
        let expectedMessages = [ "Member list query was successful" ]

        let readModel =
            MemberList.makeItemReadModel <|| (randomGuid (), randomString ())
            |> Seq.singleton
            |> MemberList.makeReadModel
            |> MemberList.makeSuccessResult
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling to message display read model, as not found result with member list read model, returns expected result`` () =
        let expectedHeading = "Member List Query"
        let expectedSeverity = MessageDisplay.errorSeverity
        let expectedMessages = [ "Member list query was not successful and returned not found" ]

        let readModel =
            MemberList.notFoundResult
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>
