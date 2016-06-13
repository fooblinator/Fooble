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
module MemberChangeEmailCommandHandlerTests =

    [<Test>]
    let ``Calling handle, with id not found in data store, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let newEmail = EmailAddress.random 32

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let handler = MemberChangeEmailCommand.makeHandler context

        let command = MemberChangeEmailCommand.make id currentPassword newEmail
        let commandResult = handler.Handle(command)

        commandResult.IsNotFound =! true
        commandResult.IsSuccess =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handle, with incorrect password, returns expected result`` () =
        let id = Guid.random ()
        let incorrectPassword = Password.random 32
        let newEmail = EmailAddress.random 32

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = MemberChangeEmailCommand.makeHandler context

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(includeDeactivated = true))

        // add matching member to the data store
        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            memberDataFactory.Invoke(id, String.random 32, passwordData, EmailAddress.random 32, String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let command = MemberChangeEmailCommand.make id incorrectPassword newEmail
        let commandResult = handler.Handle(command)

        commandResult.IsIncorrectPassword =! true
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handle, with unavailable email, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let unavailableEmail = EmailAddress.random 32

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = MemberChangeEmailCommand.makeHandler context

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(includeDeactivated = true))

        // add matching member to the data store
        let passwordData = Crypto.hash currentPassword 100
        let memberData =
            memberDataFactory.Invoke(id, String.random 32, passwordData, unavailableEmail, String.random 64)
        memberData.RegisteredOn <- DateTime(2001, 1, 1)
        memberData.PasswordChangedOn <- DateTime(2001, 1, 1)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let command = MemberChangeEmailCommand.make id currentPassword unavailableEmail
        let commandResult = handler.Handle(command)

        commandResult.IsUnavailableEmail =! true
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling handle, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let newEmail = EmailAddress.random 32

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = MemberChangeEmailCommand.makeHandler context

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(includeDeactivated = true))

        // add matching member to the data store
        let passwordData = Crypto.hash currentPassword 100
        let memberData =
            memberDataFactory.Invoke(id, String.random 32, passwordData, EmailAddress.random 32, String.random 64)
        memberData.RegisteredOn <- DateTime(2001, 1, 1)
        memberData.PasswordChangedOn <- DateTime(2001, 1, 1)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let command = MemberChangeEmailCommand.make id currentPassword newEmail
        let commandResult = handler.Handle(command)

        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableEmail =! false
