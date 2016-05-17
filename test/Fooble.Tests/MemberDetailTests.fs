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
        let expectedMessage = "Id was null"

        let result = MemberDetail.validateId null

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate id, with empty id, returns expected message``() =
        let expectedParamName = "id"
        let expectedMessage = "Id was empty string"

        let result = MemberDetail.validateId ""

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate id, with invalid guid formatted id, returns expected message``() =
        let expectedParamName = "id"
        let expectedMessage = "Id was not GUID string"

        let result = MemberDetail.validateId (randomNonGuidString())

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

[<TestFixture>]
module MemberDetailQueryTests =

    [<Test>]
    let ``Calling make, with null id, raises expected exception``() =
        let expectedParamName = "id"
        let expectedMessage = "Id was null"

        raisesWith<ArgumentException> <@ MemberDetail.Query.make null @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with empty id, raises expected exception``() =
        let expectedParamName = "id"
        let expectedMessage = "Id was empty string"

        raisesWith<ArgumentException> <@ MemberDetail.Query.make "" @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with invalid guid formatted id, raises expected exception``() =
        let expectedParamName = "id"
        let expectedMessage = "Id was not GUID string"

        raisesWith<ArgumentException> <@ MemberDetail.Query.make (randomNonGuidString()) @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() =
        let expectedId = randomGuidString()

        let query = MemberDetail.Query.make expectedId

        let actualId = query.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling id, returns expected id``() =
        let expectedId = randomGuidString()

        let query = MemberDetail.Query.make expectedId

        let actualId = query.Id
        test <@ actualId = expectedId @>

[<TestFixture>]
module MemberDetailReadModelTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() =
        let expectedId = randomGuidString()
        let expectedName = randomGuidString()

        let readModel = MemberDetail.ReadModel.make expectedId expectedName

        let actualId = readModel.Id
        test <@ actualId = expectedId @>
        let actualName = readModel.Name
        test <@ actualName = expectedName @>

    [<Test>]
    let ``Calling id, returns expected id``() =
        let expectedId = randomGuidString()

        let readModel = MemberDetail.ReadModel.make expectedId (randomGuidString())

        let actualId = readModel.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling name, returns expected name``() =
        let expectedName = randomGuidString()

        let readModel = MemberDetail.ReadModel.make (randomGuidString()) expectedName

        let actualName = readModel.Name
        test <@ actualName = expectedName @>

[<TestFixture>]
module MemberDetailQueryFailureStatusTests =

    [<Test>]
    let ``Calling not found, returns expected status``() =
        let status = MemberDetail.QueryFailureStatus.notFound

        test <@ status.IsNotFound @>

    [<Test>]
    let ``Calling is not found, as not found query failure status, returns true``() =
        let status = MemberDetail.QueryFailureStatus.notFound

        test <@ status.IsNotFound @>

[<TestFixture>]
module MemberDetailQueryHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() =
        ignore <| MemberDetail.QueryHandler.make (mock())
