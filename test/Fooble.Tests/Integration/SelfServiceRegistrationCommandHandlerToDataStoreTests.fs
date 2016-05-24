namespace Fooble.Tests.Integration

open Fooble.Core
open Fooble.Core.Persistence
open Fooble.Tests
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegistrationCommandHandlerToDataStoreTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns command handler`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString
        let commandHandler = SelfServiceRegister.CommandHandler.make context

        test <@ box commandHandler :? IRequestHandler<ISelfServiceRegisterCommand, Unit> @>

    [<Test>]
    let ``Calling handle, registers new member, and completes without exception`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let commandHandler = SelfServiceRegister.CommandHandler.make context

        let command = SelfServiceRegister.Command.make <|| (randomGuid (), randomString ())
        ignore <| commandHandler.Handle(command)
