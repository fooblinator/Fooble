namespace Fooble.IntegrationTest

open Fooble.Core
open Fooble.Persistence
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListQueryHandlerToDataStoreTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext (Some connectionString)
        let handler = MemberList.QueryHandler.make context

        test <@ box handler :? IRequestHandler<IMemberListQuery, IMemberListQueryResult> @>

    [<Test>]
    let ``Calling handle, with no members in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext (Some connectionString)

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let handler = MemberList.QueryHandler.make context

        let query = MemberList.Query.make ()
        let queryResult = handler.Handle(query)

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with members in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext (Some connectionString)

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // add members to the data store
        let memberData = List.init 5 (fun _ ->
            MemberData(Id = Guid.random (), Username = String.random 32,
                Email = (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)),
                Nickname = String.random 64))
        List.iter (fun x -> context.MemberData.AddObject(x)) memberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let handler = MemberList.QueryHandler.make context

        let query = MemberList.Query.make ()
        let queryResult = handler.Handle(query)

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualMembers = Seq.toList queryResult.ReadModel.Members

        test <@ List.length actualMembers = 5 @>
        for current in actualMembers do
            let findResult =
                List.tryFind (fun (x:MemberData) -> x.Id = current.Id && x.Nickname = current.Nickname) memberData
            test <@ findResult.IsSome @>

