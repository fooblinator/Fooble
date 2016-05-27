namespace Fooble.UnitTest.SelfServiceRegister

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
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
        let handler = SelfServiceRegisterCommand.makeHandler (mock ())

        test <@ box handler :? IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult> @>

    [<Test>]
    let ``Calling handle, with existing username in data store, and returns expected result`` () =
        let existingUsername = String.random 32

        let memberData =
            Seq.singleton
                (MemberData(Id = Guid.random (), Username = existingUsername,
                    Email = sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3),
                    Nickname = String.random 64))
        let memberSetMock = makeObjectSet memberData
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let command =
            SelfServiceRegister.makeCommand (Guid.random ()) existingUsername
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64)
        let handler = SelfServiceRegisterCommand.makeHandler contextMock.Object

        let commandResult = handler.Handle(command)

        contextMock.Verify()

        test <@ commandResult.IsUsernameUnavailable @>
        test <@ not <| commandResult.IsSuccess @>
        test <@ not <| commandResult.IsEmailUnavailable @>

    [<Test>]
    let ``Calling handle, with existing email in data store, and returns expected result`` () =
        let existingEmail = sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)

        let memberData =
            Seq.singleton
                (MemberData(Id = Guid.random (), Username = String.random 32, Email = existingEmail,
                    Nickname = String.random 64))
        let memberSetMock = makeObjectSet memberData
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let command =
            SelfServiceRegister.makeCommand (Guid.random ()) (String.random 32) existingEmail (String.random 64)
        let handler = SelfServiceRegisterCommand.makeHandler contextMock.Object

        let commandResult = handler.Handle(command)

        contextMock.Verify()

        test <@ commandResult.IsEmailUnavailable @>
        test <@ not <| commandResult.IsSuccess @>
        test <@ not <| commandResult.IsUsernameUnavailable @>

    [<Test>]
    let ``Calling handle, with no existing username or email in data store, returns expected result`` () =
        let memberSetMock = makeObjectSet (Seq.empty<MemberData>)
        memberSetMock.SetupAction(fun x -> x.AddObject(any ())).Verifiable()
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.MemberData).Returns(memberSetMock.Object).Verifiable()

        let command =
            SelfServiceRegister.makeCommand (Guid.random ()) (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64)
        let handler = SelfServiceRegisterCommand.makeHandler contextMock.Object

        let commandResult = handler.Handle(command)

        memberSetMock.Verify()
        contextMock.Verify()

        test <@ commandResult.IsSuccess @>
        test <@ not <| commandResult.IsUsernameUnavailable @>
        test <@ not <| commandResult.IsEmailUnavailable @>
