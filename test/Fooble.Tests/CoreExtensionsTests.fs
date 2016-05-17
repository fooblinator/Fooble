namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module CoreExtensionsTests =

    [<Test>]
    let ``Calling to message display read model, with null result of member detail read model, raises expected exception``() =
        let expectedParamName = "result"
        let expectedMessage = "Result was null"

        let nullResult = null :> IResult<IMemberDetailReadModel,IMemberDetailQueryFailureStatus>
        raisesWith<ArgumentException> <@ CoreExtensions.ToMessageDisplayReadModel(nullResult) @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling to message display read model, as success result of member detail read model, returns expected result``() =
        let expectedHeading = "Member Detail Query"
        let expectedSeverity = MessageDisplay.Severity.informational
        let expectedMessages = [ "Member detail query was successful" ]

        let readModel =
            MemberDetail.ReadModel.make (randomGuidString()) (randomGuidString())
            |> Result.success
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling to message display read model, with null result of member list read model, raises expected exception``() =
        let expectedParamName = "result"
        let expectedMessage = "Result was null"

        let nullResult = null :> IResult<IMemberListReadModel,IMemberListQueryFailureStatus>
        raisesWith<ArgumentException> <@ CoreExtensions.ToMessageDisplayReadModel(nullResult) @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling to message display read model, as success result with member list read model, returns expected result``() =
        let expectedHeading = "Member List Query"
        let expectedSeverity = MessageDisplay.Severity.informational
        let expectedMessages = [ "Member list query was successful" ]

        let readModel =
            [ MemberList.ItemReadModel.make (randomGuidString()) (randomGuidString()) ]
            |> MemberList.ReadModel.make
            |> Result.success
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>
