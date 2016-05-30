namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.IntegrationTest
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDetailQueryHandlerToDataStoreTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query handler`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        let container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let handler = MemberDetailQuery.makeHandler context (makeMemberDetailReadModelFactory ())

        test <@ box handler :? IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> @>

    [<Test>]
    let ``Calling handle, with no matching member in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        let container = builder.Build()

        let context = container.Resolve<IFoobleContext>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        let handler = MemberDetailQuery.makeHandler context (makeMemberDetailReadModelFactory ())

        let query = MemberDetailQuery.make (Guid.random ())
        let queryResult = handler.Handle(query)

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with matching member in data store, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedEmail = sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)
        let expectedNickname = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        let container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let memberData = memberDataFactory.Invoke(expectedId, expectedUsername, expectedEmail, expectedNickname)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let handler = MemberDetailQuery.makeHandler context (makeMemberDetailReadModelFactory ())

        let query = MemberDetailQuery.make expectedId
        let queryResult = handler.Handle(query)

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualReadModel = queryResult.ReadModel
        test <@ actualReadModel.Id = expectedId @>
        test <@ actualReadModel.Username = expectedUsername @>
        test <@ actualReadModel.Email = expectedEmail @>
        test <@ actualReadModel.Nickname = expectedNickname @>
