namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.IntegrationTest
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberRegisterCommandHandlerToDataStoreTests =

    [<Test>]
    let ``Calling handle, with existing username in data store, and returns expected result`` () =
        let existingUsername = String.random 32

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler =
            container.Resolve<IRequestHandler<IMemberRegisterCommand, IMemberRegisterCommandResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let memberData =
            let passwordData = Crypto.hash (Password.random 32) 100
            memberDataFactory.Invoke(Guid.random (), existingUsername, passwordData, EmailAddress.random 32,
                String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let command =
            MemberRegisterCommand.make (Guid.random ()) existingUsername (Password.random 32)
                (EmailAddress.random 32) (String.random 64)
        let commandResult = handler.Handle(command)

        commandResult.IsUsernameUnavailable =! true
        commandResult.IsSuccess =! false
        commandResult.IsEmailUnavailable =! false

    [<Test>]
    let ``Calling handle, with existing email in data store, and returns expected result`` () =
        let existingEmail = EmailAddress.random 32

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler =
            container.Resolve<IRequestHandler<IMemberRegisterCommand, IMemberRegisterCommandResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let memberData =
            let passwordData = Crypto.hash (Password.random 32) 100
            memberDataFactory.Invoke(Guid.random (), String.random 32, passwordData, existingEmail,
                String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let command =
            MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32) existingEmail
                (String.random 64)
        let commandResult = handler.Handle(command)

        commandResult.IsEmailUnavailable =! true
        commandResult.IsSuccess =! false
        commandResult.IsUsernameUnavailable =! false

    [<Test>]
    let ``Calling handle, with no existing username or email in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let handler =
            container.Resolve<IRequestHandler<IMemberRegisterCommand, IMemberRegisterCommandResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        let command =
            MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (EmailAddress.random 32) (String.random 64)
        let commandResult = handler.Handle(command)

        commandResult.IsSuccess =! true
        commandResult.IsUsernameUnavailable =! false
        commandResult.IsEmailUnavailable =! false
