namespace Fooble.Tests

open Fooble.Core
open Fooble.Core.Persistence
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System
open System.Data.Entity

[<TestFixture>]
module MemberDetailTests =

    [<Test>]
    let ``Calling validate id, with null id, returns expected validation result``() =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was null"

        let result = MemberDetail.validateId null

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate id, with empty id, returns expected  validation result``() =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was empty string"

        let result = MemberDetail.validateId ""

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate id, with invalid guid formatted id, returns expected validation result``() =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was not GUID string"

        let result = MemberDetail.validateId (randomNonGuidString())

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling make query, with null id, raises expected exception``() =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was null"

        raisesWith<ArgumentException> <@ MemberDetail.makeQuery null @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make query, with empty id, raises expected exception``() =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was empty string"

        raisesWith<ArgumentException> <@ MemberDetail.makeQuery"" @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make query, with invalid guid formatted id, raises expected exception``() =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was not GUID string"

        raisesWith<ArgumentException> <@ MemberDetail.makeQuery (randomNonGuidString()) @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make query, with valid parameters, returns query``() =
        let query = MemberDetail.makeQuery (randomGuidString())

        test <@ box query :? IMemberDetailQuery @>
        test <@ box query :? IRequest<IMemberDetailQueryResult> @>

[<TestFixture>]
module MemberDetailQueryTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query``() =
        let query = MemberDetail.makeQuery (randomGuidString())

        test <@ box query :? IMemberDetailQuery @>
        test <@ box query :? IRequest<IMemberDetailQueryResult> @>

    [<Test>]
    let ``Calling id, returns expected id``() =
        let expectedId = randomGuidString()

        let query = MemberDetail.makeQuery expectedId

        let actualId = query.Id
        test <@ actualId = expectedId @>

[<TestFixture>]
module MemberDetailQueryHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler``() =
        let queryHandler = MemberDetail.makeQueryHandler (mock())

        test <@ box queryHandler :? IMemberDetailQueryHandler @>

    [<Test>]
    let ``Calling handler, with no matching member in data store, returns expected result``() =
        let memberSetMock = Mock<IDbSet<IMemberData>>()
        memberSetMock.SetupFunc(fun xs -> xs.Find(It.IsAny<string>())).Returns(null :> IMemberData).End
        let dataContextMock = Mock<IDataContext>()
        dataContextMock.SetupFunc(fun x -> x.Members).Returns(memberSetMock.Object).End

        let query = MemberDetail.makeQuery (randomGuidString())
        let queryHandler = MemberDetail.makeQueryHandler dataContextMock.Object

        let queryResult = queryHandler.Handle(query)

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handler, with matching member in data store, returns expected result``() =
        let expectedId = randomGuidString()
        let expectedName = randomGuidString()

        let memberData = MemberData(Id = expectedId, Name = expectedName) :> IMemberData
        let memberSetMock = Mock<IDbSet<IMemberData>>()
        memberSetMock.SetupFunc(fun xs -> xs.Find(It.IsAny<string>())).Returns(memberData).End
        let dataContextMock = Mock<IDataContext>()
        dataContextMock.SetupFunc(fun x -> x.Members).Returns(memberSetMock.Object).End

        let query = MemberDetail.makeQuery (randomGuidString())
        let queryHandler = MemberDetail.makeQueryHandler dataContextMock.Object

        let queryResult = queryHandler.Handle(query)

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualReadModel = queryResult.ReadModel

        test <@ actualReadModel.Id = expectedId @>
        test <@ actualReadModel.Name = expectedName @>
 
[<TestFixture>]
module MemberDetailReadModelTests =
 
    [<Test>]
    let ``Calling make, with valid parameters, returns read model``() =
        let readModel = MemberDetail.makeReadModel (randomGuidString()) (randomGuidString())

        test <@ box readModel :? IMemberDetailReadModel @>

    [<Test>]
    let ``Calling id, returns expected id``() =
        let expectedId = randomGuidString()

        let readModel = MemberDetail.makeReadModel expectedId (randomGuidString())

        let actualId = readModel.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling name, returns expected name``() =
        let expectedName = randomGuidString()

        let readModel = MemberDetail.makeReadModel (randomGuidString()) expectedName

        let actualName = readModel.Name
        test <@ actualName = expectedName @>
 
[<TestFixture>]
module MemberDetailQueryResultTests =
 
    [<Test>]
    let ``Calling make success, with valid parameters, returns query result``() =
        let readModel = MemberDetail.makeReadModel (randomGuidString()) (randomGuidString())
        let queryResult = MemberDetail.makeSuccessResult readModel

        test <@ box queryResult :? IMemberDetailQueryResult @>

    [<Test>]
    let ``Calling not found, returns query result``() =
        let queryResult = MemberDetail.notFoundResult

        test <@ box queryResult :? IMemberDetailQueryResult @>

    [<Test>]
    let ``Calling read model, with success query result, returns expected read model``() =
        let expectedReadModel = MemberDetail.makeReadModel (randomGuidString()) (randomGuidString())

        let queryResult = MemberDetail.makeSuccessResult expectedReadModel

        test <@ queryResult.ReadModel = expectedReadModel @>

    [<Test>]
    let ``Calling is success, with success query result, returns true``() =
        let readModel = MemberDetail.makeReadModel (randomGuidString()) (randomGuidString())
        let queryResult = MemberDetail.makeSuccessResult readModel

        test <@ queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with not found query result, returns false``() =
        let queryResult = MemberDetail.notFoundResult

        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is not found, with success query result, returns false``() =
        let readModel = MemberDetail.makeReadModel (randomGuidString()) (randomGuidString())
        let queryResult = MemberDetail.makeSuccessResult readModel

        test <@ not <| queryResult.IsNotFound @>

    [<Test>]
    let ``Calling is not found, with not found query result, returns true``() =
        let queryResult = MemberDetail.notFoundResult

        test <@ queryResult.IsNotFound @>
