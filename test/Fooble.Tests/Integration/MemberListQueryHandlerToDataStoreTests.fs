namespace Fooble.Tests.Integration

open Fooble.Core
open Fooble.Tests
open Fooble.Core.Persistence
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListQueryHandlerToDataStoreTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString
        let queryHandler = MemberList.QueryHandler.make context

        test <@ box queryHandler :? IMemberListQueryHandler @>

    [<Test>]
    let ``Calling handle, with no members in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let queryHandler = MemberList.QueryHandler.make context

        let query = MemberList.Query.make ()
        let queryResult = queryHandler.Handle(query)

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with members in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // add members to the data store
        let memberData = List.init 5 <| fun _ -> MemberData(Id = randomGuid (), Name = randomString ())
        List.iter (fun x -> context.MemberData.AddObject(x)) memberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let queryHandler = MemberList.QueryHandler.make context

        let query = MemberList.Query.make ()
        let queryResult = queryHandler.Handle(query)

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualMembers = Seq.toList queryResult.ReadModel.Members

        test <@ List.length actualMembers = 5 @>
        for current in actualMembers do
            let findResult = List.tryFind (fun (x:MemberData) -> x.Id = current.Id && x.Name = current.Name) memberData
            test <@ findResult.IsSome @>

