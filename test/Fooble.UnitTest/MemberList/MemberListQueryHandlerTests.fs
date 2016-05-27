namespace Fooble.UnitTest.MemberList

open Fooble.Core
open Fooble.UnitTest
open Fooble.Core.Persistence
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListQueryHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler`` () =
        let handler = MemberList.QueryHandler.make <| mock ()

        test <@ box handler :? IRequestHandler<IMemberListQuery, IMemberListQueryResult> @>

    [<Test>]
    let ``Calling handle, with no members in data store, returns expected result`` () =
        let memberSetMock = makeObjectSet Seq.empty<MemberData>
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let query = MemberList.Query.make ()
        let handler = MemberList.QueryHandler.make contextMock.Object

        let queryResult = handler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with members in data store, returns expected result`` () =
        let memberData = List.init 5 <| fun _ ->
            MemberData(Id = Guid.random (), Username = String.random 32,
                Email = sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3),
                Nickname = String.random 64)
        let memberSetMock = makeObjectSet (Seq.ofList memberData)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let query = MemberList.Query.make ()
        let handler = MemberList.QueryHandler.make contextMock.Object

        let queryResult = handler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualMembers = Seq.toList queryResult.ReadModel.Members
        test <@ List.length actualMembers = 5 @>
        for current in actualMembers do
            let findResult =
                List.tryFind (fun (x:MemberData) -> x.Id = current.Id && x.Nickname = current.Nickname) memberData
            test <@ findResult.IsSome @>
