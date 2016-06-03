namespace Fooble.UnitTest

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
        let handler = SelfServiceRegisterCommand.makeHandler (mock ()) (mock ())

        test <@ box handler :? IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult> @>

    [<Test>]
    let ``Calling handle, with existing username in data store, and returns expected result`` () =
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(any ())).Returns(true).Verifiable()

        let handler = SelfServiceRegisterCommand.makeHandler contextMock.Object (mock ())

        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (EmailAddress.random ()) (String.random 64)
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        test <@ commandResult.IsUsernameUnavailable @>
        test <@ not <| commandResult.IsSuccess @>
        test <@ not <| commandResult.IsEmailUnavailable @>

    [<Test>]
    let ``Calling handle, with existing email in data store, and returns expected result`` () =
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(any ())).Returns(true).Verifiable()

        let handler = SelfServiceRegisterCommand.makeHandler contextMock.Object (mock ())

        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (EmailAddress.random ()) (String.random 64)
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        test <@ commandResult.IsEmailUnavailable @>
        test <@ not <| commandResult.IsSuccess @>
        test <@ not <| commandResult.IsUsernameUnavailable @>

    [<Test>]
    let ``Calling handle, with no existing username or email in data store, returns expected result`` () =
        let capturedMemberData = ref null

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(any ())).Returns(false).Verifiable()
        contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(any ())).Returns(false).Verifiable()
        contextMock.SetupAction(fun x -> x.AddMember(any ()))
            .Callback<IMemberData>(fun x -> capturedMemberData := box x).Verifiable()

        let handler = SelfServiceRegisterCommand.makeHandler contextMock.Object (makeTestMemberDataFactory ())

        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (EmailAddress.random ()) (String.random 64)
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        test <@ isNotNull !capturedMemberData @>

        test <@ commandResult.IsSuccess @>
        test <@ not <| commandResult.IsUsernameUnavailable @>
        test <@ not <| commandResult.IsEmailUnavailable @>
