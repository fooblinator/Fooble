namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.IntegrationTest
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegisterCommandHandlerToDataStoreTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns command handler`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        let container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let handler = SelfServiceRegisterCommand.makeHandler context (makeMemberDataFactory ())

        test <@ box handler :? IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult> @>

    [<Test>]
    let ``Calling handle, with existing username in data store, and returns expected result`` () =
        let existingUsername = String.random 32

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        let container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let memberData =
            memberDataFactory.Invoke(Guid.random (), existingUsername,
                sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3), String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let handler = SelfServiceRegisterCommand.makeHandler context memberDataFactory

        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) existingUsername
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64)
        let commandResult = handler.Handle(command)

        test <@ commandResult.IsUsernameUnavailable @>
        test <@ not <| commandResult.IsSuccess @>
        test <@ not <| commandResult.IsEmailUnavailable @>

    [<Test>]
    let ``Calling handle, with existing email in data store, and returns expected result`` () =
        let existingEmail = sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)

        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        let container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // add matching member to the data store
        let memberData = memberDataFactory.Invoke(Guid.random (), String.random 32, existingEmail, String.random 64)
        context.AddMember(memberData)

        // persist changes to the data store
        context.SaveChanges()

        let handler = SelfServiceRegisterCommand.makeHandler context memberDataFactory

        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32) existingEmail (String.random 64)
        let commandResult = handler.Handle(command)

        test <@ commandResult.IsEmailUnavailable @>
        test <@ not <| commandResult.IsSuccess @>
        test <@ not <| commandResult.IsUsernameUnavailable@>

    [<Test>]
    let ``Calling handle, with no existing username or email in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        let container = builder.Build()

        let context = container.Resolve<IFoobleContext>()
        let memberDataFactory = container.Resolve<MemberDataFactory>()

        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers())

        // persist changes to the data store
        context.SaveChanges()

        let handler = SelfServiceRegisterCommand.makeHandler context memberDataFactory

        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64)
        let commandResult = handler.Handle(command)

        test <@ commandResult.IsSuccess @>
        test <@ not <| commandResult.IsUsernameUnavailable @>
        test <@ not <| commandResult.IsEmailUnavailable @>
