﻿namespace Fooble.IntegrationTest

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
module MemberListQueryHandlerToDataStoreTests =

    [<Test>]
    let ``Calling handle, with no members in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let handler = container.Resolve<IRequestHandler<IMemberListQuery, IMemberListQueryResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        let query = MemberListQuery.make ()
        let queryResult = handler.Handle(query)

        test <@ queryResult.IsNotFound @>
        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with members in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = container.Resolve<IRequestHandler<IMemberListQuery, IMemberListQueryResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add members to the data store
        let members =
            List.init 5 (fun _ ->
                memberDataFactory.Invoke(Guid.random (), String.random 32, Password.random 32, EmailAddress.random (),
                    String.random 64))
        List.iter (fun x -> context.AddMember(x)) members

        // persist changes to the data store
        context.SaveChanges()

        let query = MemberListQuery.make ()
        let queryResult = handler.Handle(query)

        test <@ queryResult.IsSuccess @>
        test <@ not <| queryResult.IsNotFound @>

        let actualMembers = Seq.toList queryResult.ReadModel.Members
        test <@ List.length actualMembers = 5 @>
        for current in actualMembers do
            let findResult =
                List.tryFind (fun (x:IMemberData) -> x.Id = current.Id && x.Nickname = current.Nickname) members
            test <@ findResult.IsSome @>
