namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Core.Persistence
open Fooble.Tests
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

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
