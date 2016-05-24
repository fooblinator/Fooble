namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Core.Persistence
open Fooble.Tests
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegisterCommandHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns command handler`` () =
        let commandHandler = SelfServiceRegister.CommandHandler.make <| mock ()

        test <@ box commandHandler :? IRequestHandler<ISelfServiceRegisterCommand, Unit> @>

    [<Test>]
    let ``Calling handle, registers new member, and completes without exception`` () =
        let memberSetMock = makeObjectSet Seq.empty<MemberData>
        memberSetMock.SetupAction(fun x -> x.AddObject(any ())).Verifiable()
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let command = SelfServiceRegister.Command.make <|| (randomGuid (), randomString ())
        let commandHandler = SelfServiceRegister.CommandHandler.make contextMock.Object

        ignore <| commandHandler.Handle(command)

        memberSetMock.Verify()
        contextMock.Verify()
