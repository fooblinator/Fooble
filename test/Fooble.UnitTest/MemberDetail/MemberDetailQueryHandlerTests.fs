namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailQueryHandlerTests =

    [<Test>]
    let ``Calling handle, with no matching member in data store, returns expected result`` () =
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(None).Verifiable()

        let handler = MemberDetailQuery.makeHandler contextMock.Object (mock ())

        let query = MemberDetailQuery.make (Guid.random ())
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        queryResult.IsNotFound =! true
        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling handle, with matching member in data store, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64
        let expectedRegistered = DateTime.UtcNow
        let expectedPasswordChanged = DateTime.UtcNow

        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            makeTestMemberData2 expectedId expectedUsername passwordData expectedEmail expectedNickname
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(Some(memberData)).Verifiable()

        let handler = MemberDetailQuery.makeHandler contextMock.Object (makeTestMemberDetailReadModelFactory ())

        let query = MemberDetailQuery.make expectedId
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        queryResult.IsSuccess =! true
        queryResult.IsNotFound =! false

        let actualReadModel = queryResult.ReadModel
        testMemberDetailReadModel actualReadModel expectedId expectedUsername expectedEmail expectedNickname
            expectedRegistered expectedPasswordChanged
