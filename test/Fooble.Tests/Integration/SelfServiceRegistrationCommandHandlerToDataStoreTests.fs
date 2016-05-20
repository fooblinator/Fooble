namespace Fooble.Tests.Integration

open Fooble.Core
open Fooble.Core.Persistence
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegistrationCommandHandlerToDataStoreTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns command handler`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString
        let commandHandler = SelfServiceRegister.makeCommandHandler context

        test <@ box commandHandler :? ISelfServiceRegisterCommandHandler @>

    [<Test>]
    let ``Calling handler, with no duplicate member id in data store, registers new member, and returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let commandHandler = SelfServiceRegister.makeCommandHandler context

        let command = SelfServiceRegister.makeCommand <|| (randomGuid (), randomString ())
        let commandResult = commandHandler.Handle(command)

        test <@ commandResult.IsSuccess @>
        test <@ not <| commandResult.IsDuplicateId @>

    [<Test>]
    let ``Calling handler, with duplicate member id in data store, does not register new member, and returns expected result`` () =
        let existingId = randomGuid ()

        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // add matching member to the data store
        let memberData = MemberData(Id = existingId, Name = randomString ())
        context.MemberData.AddObject(memberData)

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let commandHandler = SelfServiceRegister.makeCommandHandler context

        let command = SelfServiceRegister.makeCommand <|| (existingId, randomString ())
        let commandResult = commandHandler.Handle(command)

        test <@ commandResult.IsDuplicateId @>
        test <@ not <| commandResult.IsSuccess @>

