namespace Fooble.UnitTest.SelfServiceRegister

open Fooble.Core
open Fooble.Core.Persistence
open Fooble.UnitTest
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegisterCommandHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns command handler`` () =
        let handler = SelfServiceRegister.CommandHandler.make (mock ())

        test <@ box handler :? IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult> @>

    [<Test>]
    let ``Calling handle, registers new member, and completes without exception`` () =
        let memberSetMock = makeObjectSet Seq.empty<MemberData>
        memberSetMock.SetupAction(fun x -> x.AddObject(any ())).Verifiable()
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let command = SelfServiceRegister.Command.make (Guid.random ()) (String.random 32) (String.random 64)
        let handler = SelfServiceRegister.CommandHandler.make contextMock.Object

        ignore <| handler.Handle(command)

        memberSetMock.Verify()
        contextMock.Verify()
