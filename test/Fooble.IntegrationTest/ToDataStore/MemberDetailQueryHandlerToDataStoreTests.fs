namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.IntegrationTest
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open Fooble.Presentation.Infrastructure
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDetailQueryHandlerToDataStoreTests =

    [<Test>]
    let ``Calling handle, with no matching member in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let handler = container.Resolve<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        let query = MemberDetailQuery.make (Guid.random ())
        let queryResult = handler.Handle(query)

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with matching member in data store, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = container.Resolve<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let memberData =
            memberDataFactory.Invoke(expectedId, expectedUsername, Password.random 32, expectedEmail, expectedNickname)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let query = MemberDetailQuery.make expectedId
        let queryResult = handler.Handle(query)

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualReadModel = queryResult.ReadModel
        testMemberDetailReadModel actualReadModel expectedId expectedUsername expectedEmail expectedNickname
