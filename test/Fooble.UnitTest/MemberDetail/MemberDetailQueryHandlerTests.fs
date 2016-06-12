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
    let ``Calling handle, with id not found in data store, returns expected result`` () =
        let id = Guid.random ()

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any (), considerDeactivated = false)).Returns(None).Verifiable()

        let handler = MemberDetailQuery.makeHandler contextMock.Object (mock ())

        let query = MemberDetailQuery.make id
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        queryResult.IsNotFound =! true
        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling handle, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let username = String.random 32
        let email = EmailAddress.random 32
        let nickname = String.random 64
        let registered = DateTime.UtcNow
        let passwordChanged = DateTime.UtcNow

        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            makeTestMemberData2 id username passwordData email nickname
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(Some(memberData)).Verifiable()

        let handler = MemberDetailQuery.makeHandler contextMock.Object (makeTestMemberDetailReadModelFactory ())

        let query = MemberDetailQuery.make id
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        queryResult.IsSuccess =! true
        queryResult.IsNotFound =! false

        let actualReadModel = queryResult.ReadModel
        testMemberDetailReadModel actualReadModel id username email nickname registered passwordChanged
