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
module MemberChangePasswordCommandHandlerTests =

    [<Test>]
    let ``Calling handle, with id not found in data store, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let newPassword = Password.random 32

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(None).Verifiable()

        let handler = MemberChangePasswordCommand.makeHandler contextMock.Object

        let command = MemberChangePasswordCommand.make id currentPassword newPassword
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsNotFound =! true
        commandResult.IsSuccess =! false
        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling handle, with incorrect password, returns expected result`` () =
        let id = Guid.random ()
        let incorrectPassword = Password.random 32
        let newPassword = Password.random 32

        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            makeTestMemberData2 id (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(Some(memberData)).Verifiable()

        let handler = MemberChangePasswordCommand.makeHandler contextMock.Object

        let command = MemberChangePasswordCommand.make id incorrectPassword newPassword
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsIncorrectPassword =! true
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling handle, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let currentPasswordData = Crypto.hash currentPassword 100
        let newPassword = Password.random 32
        let passwordChanged = DateTime.UtcNow

        let memberData =
            makeTestMemberData id (String.random 32) currentPasswordData (EmailAddress.random 32)
                (String.random 64) (DateTime(2001, 01, 01)) (DateTime(2001, 01, 01))
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(Some(memberData)).Verifiable()

        let handler = MemberChangePasswordCommand.makeHandler contextMock.Object

        let command = MemberChangePasswordCommand.make id currentPassword newPassword
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false

        memberData.PasswordData <>! currentPasswordData

        let actualPasswordChanged = memberData.PasswordChanged
        actualPasswordChanged.Date =! passwordChanged.Date
