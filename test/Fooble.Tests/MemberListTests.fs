namespace Fooble.Tests

open Fooble.Core
open Fooble.Core.Persistence
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System
open System.Data.Entity

[<TestFixture>]
module MemberListTests =

    [<Test>]
    let ``Calling make query, returns query``() =
        let query = MemberList.makeQuery ()

        test <@ box query :? IMemberListQuery @>
        test <@ box query :? IRequest<IMemberListQueryResult> @>

[<TestFixture>]
module MemberListQueryTests =

    [<Test>]
    let ``Calling make, returns query``() =
        let query = MemberList.makeQuery ()

        test <@ box query :? IMemberListQuery @>
        test <@ box query :? IRequest<IMemberListQueryResult> @>

[<TestFixture>]
module MemberListQueryHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler``() =
        let queryHandler = MemberList.makeQueryHandler (mock())

        test <@ box queryHandler :? IMemberListQueryHandler @>

    [<Test>]
    let ``Calling handler, with no members in data store, returns expected result``() =
        let memberSet = makeDbSet Seq.empty<IMemberData>
        let dataContextMock = Mock<IDataContext>()
        dataContextMock.SetupFunc(fun x -> x.Members).Returns(memberSet).End

        let query = MemberList.makeQuery ()
        let queryHandler = MemberList.makeQueryHandler dataContextMock.Object

        let queryResult = queryHandler.Handle(query)

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handler, with members in data store, returns expected result``() =
        let expectedId = randomGuidString()
        let expectedName = randomGuidString()

        let memberData = 
            [ MemberData(Id = randomGuidString(), Name = randomGuidString()) :> IMemberData
              MemberData(Id = randomGuidString(), Name = randomGuidString()) :> IMemberData
              MemberData(Id = randomGuidString(), Name = randomGuidString()) :> IMemberData
              MemberData(Id = randomGuidString(), Name = randomGuidString()) :> IMemberData
              MemberData(Id = randomGuidString(), Name = randomGuidString()) :> IMemberData ]
        let memberSet = makeDbSet (Seq.ofList memberData)
        let dataContextMock = Mock<IDataContext>()
        dataContextMock.SetupFunc(fun c -> c.Members).Returns(memberSet).End

        let query = MemberList.makeQuery ()
        let queryHandler = MemberList.makeQueryHandler dataContextMock.Object

        let queryResult = queryHandler.Handle(query)

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualMembers = Seq.toList queryResult.ReadModel.Members

        test <@ List.length actualMembers = 5 @>
        for current in actualMembers do
            let findResult = List.tryFind (fun (x:IMemberData) -> x.Id = current.Id && x.Name = current.Name) memberData
            test <@ findResult.IsSome @>
 
[<TestFixture>]
module MemberListItemReadModelTests =
 
    [<Test>]
    let ``Calling make, with valid parameters, returns read model``() =
        let readModel = MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())

        test <@ box readModel :? IMemberListItemReadModel @>

    [<Test>]
    let ``Calling id, returns expected id``() =
        let expectedId = randomGuidString()

        let itemReadModel = MemberList.makeItemReadModel expectedId (randomGuidString())

        let actualId = itemReadModel.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling name, returns expected name``() =
        let expectedName = randomGuidString()

        let itemReadModel = MemberList.makeItemReadModel (randomGuidString()) expectedName

        let actualName = itemReadModel.Name
        test <@ actualName = expectedName @>
 
[<TestFixture>]
module MemberListReadModelTests =
 
    [<Test>]
    let ``Calling make, with valid parameters, returns read model``() =
        let members = 
            [ MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString()) ]

        let readModel = MemberList.makeReadModel(Seq.ofList members)

        test <@ box readModel :? IMemberListReadModel @>

    [<Test>]
    let ``Calling members, returns expected members``() =
        let expectedMembers = 
            [ MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString()) ]

        let readModel = MemberList.makeReadModel(Seq.ofList expectedMembers)

        let actualMembers = Seq.toList readModel.Members
        test <@ List.length actualMembers = 5 @>
        for current in actualMembers do
            let findResult =
                List.tryFind (fun (x:IMemberListItemReadModel) -> x.Id = current.Id && x.Name = current.Name)
                    expectedMembers
            test <@ findResult.IsSome @>
 
[<TestFixture>]
module MemberListQueryResultTests =
 
    [<Test>]
    let ``Calling make success, with valid parameters, returns query result``() =
        let members = 
            [ MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString()) ]
        let readModel = MemberList.makeReadModel(Seq.ofList members)
        let queryResult = MemberList.makeSuccessResult(readModel)

        test <@ box queryResult :? IMemberListQueryResult @>

    [<Test>]
    let ``Calling not found, returns query result``() =
        let queryResult = MemberList.notFoundResult

        test <@ box queryResult :? IMemberListQueryResult @>

    [<Test>]
    let ``Calling read model, with success query result, returns expected read model``() =
        let members = 
            [ MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString()) ]
        let expectedReadModel = MemberList.makeReadModel(Seq.ofList members)

        let queryResult = MemberList.makeSuccessResult(expectedReadModel)

        test <@ queryResult.ReadModel = expectedReadModel @>

    [<Test>]
    let ``Calling is success, with success query result, returns true``() =
        let members = 
            [ MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString()) ]
        let readModel = MemberList.makeReadModel(Seq.ofList members)

        let queryResult = MemberList.makeSuccessResult(readModel)

        test <@ queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with not found query result, returns false``() =
        let queryResult = MemberList.notFoundResult

        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is not found, with success query result, returns false``() =
        let members = 
            [ MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString())
              MemberList.makeItemReadModel (randomGuidString()) (randomGuidString()) ]
        let readModel = MemberList.makeReadModel(Seq.ofList members)

        let queryResult = MemberList.makeSuccessResult(readModel)

        test <@ not <| queryResult.IsNotFound @>

    [<Test>]
    let ``Calling is not found, with not found query result, returns true``() =
        let queryResult = MemberList.notFoundResult

        test <@ queryResult.IsNotFound @>
