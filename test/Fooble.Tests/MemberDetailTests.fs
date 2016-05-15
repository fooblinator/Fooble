namespace Fooble.Tests

open Fooble.Core
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailTests =

    [<Test>]
    let ``Calling validate id, with null id, returns expected message``() =
        let expectedParamName = "id"
        let expectedMessage = "Id was null value"

        let result = MemberDetail.validateId null

        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate id, with empty id, returns expected message``() =
        let expectedParamName = "id"
        let expectedMessage = "Id was empty string"

        let result = MemberDetail.validateId ""

        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate id, with invalid guid formatted id, returns expected message``() =
        let expectedParamName = "id"
        let expectedMessage = "Id was string with invalid GUID format"

        let result = MemberDetail.validateId (randomNonGuidString())

        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling make query, with null id, raises expected exception``() =
        let expectedParamName = "id"
        let expectedMessage = "Id was null value"

        raisesWith<ArgumentException> <@ MemberDetail.makeQuery null @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make query, with empty id, raises expected exception``() =
        let expectedParamName = "id"
        let expectedMessage = "Id was empty string"

        raisesWith<ArgumentException> <@ MemberDetail.makeQuery "" @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make query, with invalid guid formatted id, raises expected exception``() =
        let expectedParamName = "id"
        let expectedMessage = "Id was string with invalid GUID format"

        raisesWith<ArgumentException> <@ MemberDetail.makeQuery (randomNonGuidString()) @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make query, with valid parameters, returns expected result``() =
        let expectedId = randomGuidString()

        let query = MemberDetail.makeQuery expectedId

        let actualId = query.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling query id, returns expected id``() =
        let expectedId = randomGuidString()

        let query = MemberDetail.makeQuery expectedId

        let actualId = query.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling make read model, with valid parameters, returns expected result``() =
        let expectedId = randomGuidString()
        let expectedName = randomGuidString()

        let readModel = MemberDetail.makeReadModel expectedId expectedName

        let actualId = readModel.Id
        test <@ actualId = expectedId @>
        let actualName = readModel.Name
        test <@ actualName = expectedName @>

    [<Test>]
    let ``Calling read model id, returns expected id``() =
        let expectedId = randomGuidString()

        let readModel = MemberDetail.makeReadModel expectedId (randomGuidString())

        let actualId = readModel.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling read model name, returns expected name``() =
        let expectedName = randomGuidString()

        let readModel = MemberDetail.makeReadModel (randomGuidString()) expectedName

        let actualName = readModel.Name
        test <@ actualName = expectedName @>

    [<Test>]
    let ``Calling not found query failure status, returns expected status``() =
        let status = MemberDetail.notFoundQueryFailureStatus

        test <@ status.IsNotFound @>

    [<Test>]
    let ``Calling query failure status is not found, as not found, returns true``() =
        let status = MemberDetail.notFoundQueryFailureStatus

        test <@ status.IsNotFound @>

    [<Test>]
    let ``Calling make query handler, with valid parameters, returns expected result``() =
        ignore <| MemberDetail.makeQueryHandler (mock())

    [<Test>]
    let ``Calling to message display read model, with null result, raises expected exception``() =
        let expectedParamName = "result"
        let expectedMessage = "Result was null value"

        let nullResult = null :> IResult<IMemberDetailReadModel,IMemberDetailQueryFailureStatus>
        raisesWith<ArgumentException> <@ CoreExtensions.ToMessageDisplayReadModel(nullResult) @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling to message display read model, as success result with member detail read model, returns expected result``() =
        let expectedHeading = "Member Detail Query"
        let expectedSeverity = MessageDisplay.informationalSeverity
        let expectedMessages = [ "Member detail query was successful" ]

        let readModel =
            MemberDetail.makeReadModel (randomGuidString()) (randomGuidString())
            |> successResult
            |> CoreExtensions.ToMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>
