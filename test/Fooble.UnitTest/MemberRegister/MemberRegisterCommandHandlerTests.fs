namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberRegisterCommandHandlerTests =

    [<Test>]
    let ``Calling handle, with existing username in data store, returns expected result`` () =
        let id = Guid.random ()
        let username = String.random 32
        let password = Password.random 32
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(any ())).Returns(true).Verifiable()

        let handler = MemberRegisterCommand.makeHandler contextMock.Object (mock ())

        let command = MemberRegisterCommand.make id username password email nickname
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsUnavailableUsername =! true
        commandResult.IsSuccess =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handle, with existing email in data store, returns expected result`` () =
        let id = Guid.random ()
        let username = String.random 32
        let password = Password.random 32
        let email = EmailAddress.random 32
        let nickname = String.random 64

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(any ())).Returns(true).Verifiable()

        let handler = MemberRegisterCommand.makeHandler contextMock.Object (mock ())

        let command = MemberRegisterCommand.make id username password email nickname
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsUnavailableEmail =! true
        commandResult.IsSuccess =! false
        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling handle, with no existing username or email in data store, returns expected result`` () =
        let id = Guid.random ()
        let username = String.random 32
        let password = Password.random 32
        let email = EmailAddress.random 32
        let nickname = String.random 64
        let registered = DateTime.UtcNow
        let passwordChanged = DateTime.UtcNow

        let capturedMemberData = ref null

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(any ())).Returns(false).Verifiable()
        contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(any ())).Returns(false).Verifiable()
        contextMock.SetupAction(fun x -> x.AddMember(any ()))
            .Callback<IMemberData>(fun x -> capturedMemberData := box x).Verifiable()

        let handler = MemberRegisterCommand.makeHandler contextMock.Object (makeTestMemberDataFactory ())

        let command = MemberRegisterCommand.make id username password email nickname
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        isNull !capturedMemberData =! false

        let actualMemberData = !capturedMemberData :?> IMemberData
        testMemberData actualMemberData id username password email nickname registered passwordChanged

        commandResult.IsSuccess =! true
        commandResult.IsUnavailableUsername =! false
        commandResult.IsUnavailableEmail =! false
