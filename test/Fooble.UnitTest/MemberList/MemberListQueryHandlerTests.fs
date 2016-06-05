namespace Fooble.UnitTest

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
    let ``Calling handle, with no members in data store, returns expected result`` () =
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMembers()).Returns([]).Verifiable()

        let handler = MemberListQuery.makeHandler contextMock.Object (mock ()) (mock ())

        let query = MemberListQuery.make ()
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        queryResult.IsNotFound =! true
        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling handle, with members in data store, returns expected result`` () =
        let members =
            List.init 5 <| fun _ ->
                let passwordData = Crypto.hash (Password.random 32) 100
                makeTestMemberData (Guid.random ()) (String.random 32) passwordData (EmailAddress.random 32)
                    (String.random 64)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMembers()).Returns(members).Verifiable()

        let handler =
            MemberListQuery.makeHandler contextMock.Object (makeTestMemberListItemReadModelFactory ())
                (makeTestMemberListReadModelFactory ())

        let query = MemberListQuery.make ()
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        queryResult.IsSuccess =! true
        queryResult.IsNotFound =! false

        testMemberListReadModel queryResult.ReadModel members
