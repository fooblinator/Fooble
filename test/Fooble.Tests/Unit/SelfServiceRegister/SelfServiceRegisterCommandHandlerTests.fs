namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Core.Persistence
open Fooble.Tests
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegisterCommandHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns command handler`` () =
        let commandHandler = SelfServiceRegister.CommandHandler.make <| mock ()

        test <@ box commandHandler :? ISelfServiceRegisterCommandHandler @>

    [<Test>]
    let ``Calling handle, with no duplicate member id in data store, registers new member, and returns expected result`` () =
        let memberSetMock = makeObjectSet Seq.empty<MemberData>
        memberSetMock.SetupAction(fun x -> x.AddObject(any ())).Verifiable()
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let command = SelfServiceRegister.Command.make <|| (randomGuid (), randomString ())
        let commandHandler = SelfServiceRegister.CommandHandler.make contextMock.Object

        let commandResult = commandHandler.Handle(command)

        memberSetMock.Verify()
        contextMock.Verify()

        test <@ commandResult.IsSuccess @>
        test <@ not <| commandResult.IsDuplicateId @>

    [<Test>]
    let ``Calling handle, with duplicate member id in data store, does not register new member, and returns expected result`` () =
        let existingId = randomGuid ()

        let memberData = MemberData(Id = existingId, Name = randomString ())
        let memberSetMock = makeObjectSet <| Seq.singleton memberData
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let command = SelfServiceRegister.Command.make <|| (existingId, randomString ())
        let commandHandler = SelfServiceRegister.CommandHandler.make contextMock.Object

        let commandResult = commandHandler.Handle(command)

        contextMock.Verify()

        test <@ commandResult.IsDuplicateId @>
        test <@ not <| commandResult.IsSuccess @>
