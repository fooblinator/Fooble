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
module SelfServiceRegisterCommandHandlerToDataStoreTests =

    [<Test>]
    let ``Calling handle, with existing username in data store, and returns expected result`` () =
        let existingUsername = String.random 32

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler =
            container.Resolve<IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let memberData =
            let passwordData = Crypto.hash (Password.random 32) 100
            memberDataFactory.Invoke(Guid.random (), existingUsername, passwordData, EmailAddress.random (),
                String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) existingUsername (Password.random 32)
                (EmailAddress.random ()) (String.random 64)
        let commandResult = handler.Handle(command)

        test <@ commandResult.IsUsernameUnavailable @>
        test <@ not <| commandResult.IsSuccess @>
        test <@ not <| commandResult.IsEmailUnavailable @>

    [<Test>]
    let ``Calling handle, with existing email in data store, and returns expected result`` () =
        let existingEmail = EmailAddress.random ()

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        let handler =
            container.Resolve<IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult>>()

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
            SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32) existingEmail
                (String.random 64)
        let commandResult = handler.Handle(command)

        test <@ commandResult.IsEmailUnavailable @>
        test <@ not <| commandResult.IsSuccess @>
        test <@ not <| commandResult.IsUsernameUnavailable@>

    [<Test>]
    let ``Calling handle, with no existing username or email in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        use container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let handler =
            container.Resolve<IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult>>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (EmailAddress.random ()) (String.random 64)
        let commandResult = handler.Handle(command)

        test <@ commandResult.IsSuccess @>
        test <@ not <| commandResult.IsUsernameUnavailable @>
        test <@ not <| commandResult.IsEmailUnavailable @>
