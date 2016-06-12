namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberChangeOtherCommandHandlerTests =

    [<Test>]
    let ``Calling handle, with id not found in data store, returns expected result`` () =
        let id = Guid.random ()
        let newNickname = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let handler = MemberChangeOtherCommand.makeHandler context

        let command = MemberChangeOtherCommand.make id newNickname
        let commandResult = handler.Handle(command)

        commandResult.IsNotFound =! true
        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling handle, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let newNickname = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = MemberChangeOtherCommand.makeHandler context

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(considerDeactivated = true))

        // add matching member to the data store
        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            memberDataFactory.Invoke(id, String.random 32, passwordData, EmailAddress.random 32, String.random 64)
        memberData.Registered <- DateTime(2001, 1, 1)
        memberData.PasswordChanged <- DateTime(2001, 1, 1)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let command = MemberChangeOtherCommand.make id newNickname
        let commandResult = handler.Handle(command)

        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false

