namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open Fooble.Core.Persistence
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListQueryHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler`` () =
        let queryHandler = MemberList.makeQueryHandler <| mock ()

        test <@ box queryHandler :? IMemberListQueryHandler @>

    [<Test>]
    let ``Calling handler, with no members in data store, returns expected result`` () =
        let memberSetMock = makeObjectSet Seq.empty<MemberData>
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let query = MemberList.makeQuery ()
        let queryHandler = MemberList.makeQueryHandler contextMock.Object

        let queryResult = queryHandler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handler, with members in data store, returns expected result`` () =
        let expectedId = randomGuid ()
        let expectedName = randomString ()

        let memberData = List.init 5 <| fun _ -> MemberData(Id = randomGuid (), Name = randomString ())
        let memberSetMock = makeObjectSet <| Seq.ofList memberData
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun c -> c.MemberData).Returns(memberSetMock.Object).Verifiable()

        let query = MemberList.makeQuery ()
        let queryHandler = MemberList.makeQueryHandler contextMock.Object

        let queryResult = queryHandler.Handle(query)

        contextMock.Verify()

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualMembers = Seq.toList queryResult.ReadModel.Members

        test <@ List.length actualMembers = 5 @>
        for current in actualMembers do
            let findResult = List.tryFind (fun (x:MemberData) -> x.Id = current.Id && x.Name = current.Name) memberData
            test <@ findResult.IsSome @>
