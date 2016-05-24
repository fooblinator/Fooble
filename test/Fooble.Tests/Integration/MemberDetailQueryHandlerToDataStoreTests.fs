namespace Fooble.Tests.Integration

open Fooble.Core
open Fooble.Core.Persistence
open Fooble.Tests
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDetailQueryHandlerToDataStoreTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext (Some connectionString)
        let handler = MemberDetail.QueryHandler.make context

        test <@ box handler :? IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> @>

    [<Test>]
    let ``Calling handle, with no matching member in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext (Some connectionString)

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let handler = MemberDetail.QueryHandler.make context

        let query = MemberDetail.Query.make (Guid.random ())
        let queryResult = handler.Handle(query)

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with matching member in data store, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedName = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext (Some connectionString)

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // add matching member to the data store
        let memberData = MemberData(Id = expectedId, Username = expectedUsername, Name = expectedName)
        context.MemberData.AddObject(memberData)

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let handler = MemberDetail.QueryHandler.make context

        let query = MemberDetail.Query.make expectedId
        let queryResult = handler.Handle(query)

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualReadModel = queryResult.ReadModel

        test <@ actualReadModel.Id = expectedId @>
        test <@ actualReadModel.Username = expectedUsername @>
        test <@ actualReadModel.Name = expectedName @>
