namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Fooble.UnitTest
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberChangePasswordCommandHandlerTests =

    [<Test>]
    let ``Calling handle, with id not found in data store, and returns expected result`` () =
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(None).Verifiable()

        let handler = MemberChangePasswordCommand.makeHandler contextMock.Object

        let command = MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) (Password.random 32)
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsNotFound =! true
        commandResult.IsSuccess =! false
        commandResult.IsInvalid =! false

    [<Test>]
    let ``Calling handle, with invalid password, and returns expected result`` () =
        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            makeTestMemberData2 (Guid.random ()) (String.random 32) passwordData (EmailAddress.random 32)
                (String.random 64)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(Some(memberData)).Verifiable()

        let handler = MemberChangePasswordCommand.makeHandler contextMock.Object

        let command = MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) (Password.random 32)
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsInvalid =! true
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling handle, with existing id in the data store, and with valid password, and returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedPasswordData = Crypto.hash expectedCurrentPassword 100
        let expectedPasswordChanged = DateTime.UtcNow

        let memberData =
            makeTestMemberData expectedId (String.random 32) expectedPasswordData (EmailAddress.random 32)
                (String.random 64) (DateTime(2001, 01, 01)) (DateTime(2001, 01, 01))
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(Some(memberData)).Verifiable()

        let handler = MemberChangePasswordCommand.makeHandler contextMock.Object

        let command = MemberChangePasswordCommand.make expectedId expectedCurrentPassword (Password.random 32)
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsInvalid =! false

        memberData.PasswordData <>! expectedPasswordData

        let actualPasswordChanged = memberData.PasswordChanged
        actualPasswordChanged.Date =! expectedPasswordChanged.Date
