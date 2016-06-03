﻿namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.UnitTest
open Fooble.Persistence
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListQueryHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler`` () =
        let handler = MemberListQuery.makeHandler (mock ()) (mock ()) (mock ())

        test <@ box handler :? IRequestHandler<IMemberListQuery, IMemberListQueryResult> @>

    [<Test>]
    let ``Calling handle, with no members in data store, returns expected result`` () =
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMembers()).Returns([]).Verifiable()

        let handler = MemberListQuery.makeHandler contextMock.Object (mock ()) (mock ())

        let query = MemberListQuery.make ()
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with members in data store, returns expected result`` () =
        let members =
            List.init 5 <| fun _ ->
                let passwordData = Crypto.hash (Password.random 32) 100
                makeTestMemberData (Guid.random ()) (String.random 32) passwordData (EmailAddress.random ())
                    (String.random 64)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMembers()).Returns(members).Verifiable()

        let handler =
            MemberListQuery.makeHandler contextMock.Object (makeTestMemberListItemReadModelFactory ())
                (makeTestMemberListReadModelFactory ())

        let query = MemberListQuery.make ()
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        testMemberListReadModel queryResult.ReadModel members
