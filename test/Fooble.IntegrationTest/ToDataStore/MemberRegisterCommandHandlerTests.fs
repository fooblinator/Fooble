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
module MemberRegisterCommandHandlerTests =

    [<Test>]
    let ``Calling handle, with existing username in data store, returns expected result`` () =
        let id = Guid.random ()
        let username = String.random 32
        let password = Password.random 32
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = MemberRegisterCommand.makeHandler context memberDataFactory

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(considerDeactivated = true))

        // add matching member to the data store
        let passwordData = Crypto.hash password 100
        let memberData = memberDataFactory.Invoke(id, username, passwordData, EmailAddress.random 32, String.random 64)
        memberData.Registered <- DateTime(2001, 1, 1)
        memberData.PasswordChanged <- DateTime(2001, 1, 1)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let command = MemberRegisterCommand.make id username password email nickname
        let commandResult = handler.Handle(command)

        commandResult.IsUnavailableUsername =! true
        commandResult.IsSuccess =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handle, with existing email in data store, returns expected result`` () =
        let id = Guid.random ()
        let username = String.random 32
        let password = Password.random 32
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = MemberRegisterCommand.makeHandler context memberDataFactory

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(considerDeactivated = true))

        // add matching member to the data store
        let passwordData = Crypto.hash password 100
        let memberData = memberDataFactory.Invoke(id, String.random 32, passwordData, email, String.random 64)
        memberData.Registered <- DateTime(2001, 1, 1)
        memberData.PasswordChanged <- DateTime(2001, 1, 1)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let command = MemberRegisterCommand.make id username password email nickname
        let commandResult = handler.Handle(command)

        commandResult.IsUnavailableEmail =! true
        commandResult.IsSuccess =! false
        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling handle, with no existing username or email in data store, returns expected result`` () =
        let id = Guid.random ()
        let username = String.random 32
        let password = Password.random 32
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = MemberRegisterCommand.makeHandler context memberDataFactory

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(considerDeactivated = true))

        // persist changes to the data store
        context.SaveChanges()

        let command = MemberRegisterCommand.make id username password email nickname
        let commandResult = handler.Handle(command)

        commandResult.IsSuccess =! true
        commandResult.IsUnavailableUsername =! false
        commandResult.IsUnavailableEmail =! false
