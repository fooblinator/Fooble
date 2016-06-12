namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open Fooble.Presentation.Infrastructure
open MediatR
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailQueryHandlerTests =

    [<Test>]
    let ``Calling handle, with no matching member in data store, returns expected result`` () =
        let id = Guid.random ()

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let handler = container.Resolve<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(considerDeactivated = true))

        // persist changes to the data store
        context.SaveChanges()

        let query = MemberDetailQuery.make id
        let queryResult = handler.Handle(query)

        queryResult.IsNotFound =! true
        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling handle, with matching member in data store, returns expected result`` () =
        let id = Guid.random ()
        let username = String.random 32
        let email = EmailAddress.random 32
        let nickname = String.random 64
        let registered = DateTime.UtcNow
        let passwordChanged = DateTime.UtcNow

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = container.Resolve<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(considerDeactivated = true))

        // add matching member to the data store
        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData = memberDataFactory.Invoke(id, username, passwordData, email, nickname)
        memberData.Registered <- registered
        memberData.PasswordChanged <- passwordChanged
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let query = MemberDetailQuery.make id
        let queryResult = handler.Handle(query)

        queryResult.IsSuccess =! true
        queryResult.IsNotFound =! false

        let actualReadModel = queryResult.ReadModel
        testMemberDetailReadModel actualReadModel id username email nickname registered passwordChanged
