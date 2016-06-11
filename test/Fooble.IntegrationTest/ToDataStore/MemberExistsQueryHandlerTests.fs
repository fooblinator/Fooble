namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberExistsQueryHandlerTests =

    [<Test>]
    let ``Calling handle, with no matching member in data store, returns expected result`` () =
        let id = Guid.random ()

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let handler = container.Resolve<IRequestHandler<IMemberExistsQuery, IMemberExistsQueryResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        let query = MemberExistsQuery.make id
        let queryResult = handler.Handle(query)

        queryResult.IsNotFound =! true
        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling handle, with matching member in data store, returns expected result`` () =
        let id = Guid.random ()

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = container.Resolve<IRequestHandler<IMemberExistsQuery, IMemberExistsQueryResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            memberDataFactory.Invoke(id, String.random 32, passwordData, EmailAddress.random 32, String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let query = MemberExistsQuery.make id
        let queryResult = handler.Handle(query)

        queryResult.IsSuccess =! true
        queryResult.IsNotFound =! false
