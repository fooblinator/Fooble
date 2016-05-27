namespace Fooble.IntegrationTest

open Fooble.Core
open Fooble.Core.Persistence
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegisterCommandHandlerToDataStoreTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns command handler`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext (Some connectionString)
        let handler = SelfServiceRegister.CommandHandler.make context

        test <@ box handler :? IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult> @>

    [<Test>]
    let ``Calling handle, with existing username in data store, and returns expected result`` () =
        let existingUsername = String.random 32

        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext (Some connectionString)

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // add matching member to the data store
        let memberData = MemberData(Id = Guid.random (), Username = existingUsername, Nickname = String.random 64)
        context.MemberData.AddObject(memberData)

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let handler = SelfServiceRegister.CommandHandler.make context

        let command = SelfServiceRegister.Command.make (Guid.random ()) existingUsername (String.random 64)
        let commandResult = handler.Handle(command)

        test <@ commandResult.IsUsernameUnavailable @>
        test <@ not <| commandResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with no existing username in data store, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext (Some connectionString)

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let handler = SelfServiceRegister.CommandHandler.make context

        let command = SelfServiceRegister.Command.make (Guid.random ()) (String.random 32) (String.random 64)
        let commandResult = handler.Handle(command)

        test <@ commandResult.IsSuccess @>
        test <@ not <| commandResult.IsUsernameUnavailable @>
