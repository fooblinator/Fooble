namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Fooble.UnitTest
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDetailQueryHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler`` () =
        let handler = MemberDetailQuery.makeHandler (mock ()) (mock ())

        test <@ box handler :? IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> @>

    [<Test>]
    let ``Calling handle, with no matching member in data store, returns expected result`` () =
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(None).Verifiable()

        let handler = MemberDetailQuery.makeHandler contextMock.Object (mock ())

        let query = MemberDetailQuery.make (Guid.random ())
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with matching member in data store, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let memberData =
            makeTestMemberData expectedId expectedUsername (Password.random 32) expectedEmail expectedNickname
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(Some memberData).Verifiable()

        let handler = MemberDetailQuery.makeHandler contextMock.Object (makeTestMemberDetailReadModelFactory ())

        let query = MemberDetailQuery.make expectedId
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualReadModel = queryResult.ReadModel
        testMemberDetailReadModel actualReadModel expectedId expectedUsername expectedEmail expectedNickname
