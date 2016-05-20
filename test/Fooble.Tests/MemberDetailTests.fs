namespace Fooble.Tests

open Fooble.Core
open Fooble.Core.Persistence
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDetailTests =

    [<Test>]
    let ``Calling make query, with valid parameters, returns query`` () =
        let query = MemberDetail.makeQuery <| randomGuid ()

        test <@ box query :? IMemberDetailQuery @>
        test <@ box query :? IRequest<IMemberDetailQueryResult> @>

[<TestFixture>]
module MemberDetailQueryTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query`` () =
        let query = MemberDetail.makeQuery <| randomGuid ()

        test <@ box query :? IMemberDetailQuery @>
        test <@ box query :? IRequest<IMemberDetailQueryResult> @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = randomGuid ()

        let query = MemberDetail.makeQuery expectedId

        let actualId = query.Id
        test <@ actualId = expectedId @>

[<TestFixture>]
module MemberDetailQueryHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler`` () =
        let queryHandler = MemberDetail.makeQueryHandler <| mock ()

        test <@ box queryHandler :? IMemberDetailQueryHandler @>

    [<Test>]
    let ``Calling handler, with no matching member in data store, returns expected result`` () =
        let memberSet = makeObjectSet Seq.empty<MemberData>
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSet).Verifiable()

        let query = MemberDetail.makeQuery <| randomGuid ()
        let queryHandler = MemberDetail.makeQueryHandler contextMock.Object

        let queryResult = queryHandler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handler, with matching member in data store, returns expected result`` () =
        let expectedId = randomGuid ()
        let expectedName = randomString ()

        let memberData = Seq.singleton <| MemberData(Id = expectedId, Name = expectedName)
        let memberSet = makeObjectSet memberData
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun c -> c.MemberData).Returns(memberSet).Verifiable()

        let query = MemberDetail.makeQuery expectedId
        let queryHandler = MemberDetail.makeQueryHandler contextMock.Object

        let queryResult = queryHandler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualReadModel = queryResult.ReadModel

        test <@ actualReadModel.Id = expectedId @>
        test <@ actualReadModel.Name = expectedName @>
 
[<TestFixture>]
module MemberDetailReadModelTests =
 
    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel = MemberDetail.makeReadModel <|| (randomGuid (), randomString ())

        test <@ box readModel :? IMemberDetailReadModel @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = randomGuid ()

        let readModel = MemberDetail.makeReadModel <|| (expectedId, randomString ())

        let actualId = readModel.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling name, returns expected name`` () =
        let expectedName = randomString ()

        let readModel = MemberDetail.makeReadModel <|| (randomGuid (), expectedName)

        let actualName = readModel.Name
        test <@ actualName = expectedName @>
 
[<TestFixture>]
module MemberDetailQueryResultTests =
 
    [<Test>]
    let ``Calling make success, with valid parameters, returns query result`` () =
        let readModel = MemberDetail.makeReadModel <|| (randomGuid (), randomString ())
        let queryResult = MemberDetail.makeSuccessResult readModel

        test <@ box queryResult :? IMemberDetailQueryResult @>

    [<Test>]
    let ``Calling not found, returns query result`` () =
        let queryResult = MemberDetail.notFoundResult

        test <@ box queryResult :? IMemberDetailQueryResult @>

    [<Test>]
    let ``Calling read model, with success query result, returns expected read model`` () =
        let expectedReadModel = MemberDetail.makeReadModel <|| (randomGuid (), randomString ())

        let queryResult = MemberDetail.makeSuccessResult expectedReadModel

        test <@ queryResult.ReadModel = expectedReadModel @>

    [<Test>]
    let ``Calling is success, with success query result, returns true`` () =
        let readModel = MemberDetail.makeReadModel <|| (randomGuid (), randomString ())
        let queryResult = MemberDetail.makeSuccessResult readModel

        test <@ queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with not found query result, returns false`` () =
        let queryResult = MemberDetail.notFoundResult

        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is not found, with success query result, returns false`` () =
        let readModel = MemberDetail.makeReadModel <|| (randomGuid (), randomString ())
        let queryResult = MemberDetail.makeSuccessResult readModel

        test <@ not <| queryResult.IsNotFound @>

    [<Test>]
    let ``Calling is not found, with not found query result, returns true`` () =
        let queryResult = MemberDetail.notFoundResult

        test <@ queryResult.IsNotFound @>
