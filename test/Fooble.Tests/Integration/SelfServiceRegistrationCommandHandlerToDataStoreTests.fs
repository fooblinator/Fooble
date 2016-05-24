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
        use context = makeFoobleContext (Some connectionString)
        let handler = SelfServiceRegister.CommandHandler.make context

        test <@ box handler :? IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult> @>

    [<Test>]
    let ``Calling handle, with no existing username in data store, registers new member, and completes without exception`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext (Some connectionString)

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let handler = SelfServiceRegister.CommandHandler.make context

        let command = SelfServiceRegister.Command.make (Guid.random ()) (String.random 32) (String.random 64)
        ignore <| handler.Handle(command)
