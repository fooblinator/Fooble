namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.IntegrationTest
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module SelfServiceChangePasswordCommandHandlerToDataStoreTests =

    [<Test>]
    let ``Calling handle, with id not found in data store, and returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let handler = SelfServiceChangePasswordCommand.makeHandler context

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        let command = SelfServiceChangePasswordCommand.make (Guid.random ()) (Password.random 32) (Password.random 32)
        let commandResult = handler.Handle(command)

        commandResult.IsNotFound =! true
        commandResult.IsSuccess =! false
        commandResult.IsInvalid =! false

    [<Test>]
    let ``Calling handle, with invalid password, and returns expected result`` () =
        let expectedId = Guid.random ()
         
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = SelfServiceChangePasswordCommand.makeHandler context

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let memberData =
            let passwordData = Crypto.hash (Password.random 32) 100
            memberDataFactory.Invoke(expectedId, String.random 32, passwordData, EmailAddress.random 32,
                String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let command = SelfServiceChangePasswordCommand.make expectedId (Password.random 32) (Password.random 32)
        let commandResult = handler.Handle(command)

        commandResult.IsInvalid =! true
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling handle, with existing id in the data store, and with valid password, and returns expected result`` () =
        let expectedId = Guid.random ()
        let currentPassword = Password.random 32
        let currentPasswordData = Crypto.hash currentPassword 100
        let currentPasswordChanged = DateTime.UtcNow
        
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler = SelfServiceChangePasswordCommand.makeHandler context

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let memberData =
            memberDataFactory.Invoke(expectedId, String.random 32, currentPasswordData, EmailAddress.random 32,
                String.random 64)
        memberData.Registered <- DateTime(2001, 1, 1)
        memberData.PasswordChanged <- DateTime(2001, 1, 1)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let command = SelfServiceChangePasswordCommand.make expectedId currentPassword (Password.random 32)
        let commandResult = handler.Handle(command)

        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsInvalid =! false

        memberData.PasswordData <>! currentPasswordData

        let actualPasswordChanged = memberData.PasswordChanged
        actualPasswordChanged.Date =! currentPasswordChanged.Date
