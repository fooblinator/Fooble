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
    let ``Calling handle, with existing username in data store, and returns expected result`` () =
        let existingUsername = String.random 32

        let memberData =
            Seq.singleton (MemberData(Id = Guid.random (), Username = existingUsername, Name = String.random 64))
        let memberSetMock = makeObjectSet memberData
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let command = SelfServiceRegister.Command.make (Guid.random ()) existingUsername (String.random 64)
        let handler = SelfServiceRegister.CommandHandler.make contextMock.Object

        let commandResult = handler.Handle(command)

        contextMock.Verify()

        test <@ commandResult.IsUsernameUnavailable @>
        test <@ not <| commandResult.IsSuccess @>

    [<Test>]
    let ``Calling handle, with no existing username in data store, returns expected result`` () =
        let memberSetMock = makeObjectSet (Seq.empty<MemberData>)
        memberSetMock.SetupAction(fun x -> x.AddObject(any ())).Verifiable()
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let command = SelfServiceRegister.Command.make (Guid.random ()) (String.random 32) (String.random 64)
        let handler = SelfServiceRegister.CommandHandler.make contextMock.Object

        let commandResult = handler.Handle(command)

        memberSetMock.Verify()
        contextMock.Verify()

        test <@ commandResult.IsSuccess @>
        test <@ not <| commandResult.IsUsernameUnavailable @>
