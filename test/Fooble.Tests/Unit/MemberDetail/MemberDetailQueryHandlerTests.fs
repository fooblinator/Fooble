﻿namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Core.Persistence
open Fooble.Tests
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDetailQueryHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler`` () =
        let handler = MemberDetail.QueryHandler.make (mock ())

        test <@ box handler :? IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> @>

    [<Test>]
    let ``Calling handle, with no matching member in data store, returns expected result`` () =
        let memberSetMock = makeObjectSet Seq.empty<MemberData>
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let query = MemberDetail.Query.make (Guid.random ())
        let handler = MemberDetail.QueryHandler.make contextMock.Object

        let queryResult = handler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with matching member in data store, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedName = String.random 64

        let memberData = Seq.singleton (MemberData(Id = expectedId, Username = expectedUsername, Name = expectedName))
        let memberSetMock = makeObjectSet memberData
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let query = MemberDetail.Query.make expectedId
        let handler = MemberDetail.QueryHandler.make contextMock.Object

        let queryResult = handler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualReadModel = queryResult.ReadModel
        test <@ actualReadModel.Id = expectedId @>
        test <@ actualReadModel.Username = expectedUsername @>
        test <@ actualReadModel.Name = expectedName @>
