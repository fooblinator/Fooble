namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberChangeEmailCommandHandlerTests =

    [<Test>]
    let ``Calling handle, with id not found in data store, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let newEmail = EmailAddress.random 32

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any (), considerDeactivated = false)).Returns(None).Verifiable()

        let handler = MemberChangeEmailCommand.makeHandler contextMock.Object

        let command = MemberChangeEmailCommand.make id currentPassword newEmail
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsNotFound =! true
        commandResult.IsSuccess =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handle, with incorrect password, returns expected result`` () =
        let id = Guid.random ()
        let incorrectPassword = Password.random 32
        let newEmail = EmailAddress.random 32

        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            makeTestMemberData2 id (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(Some(memberData)).Verifiable()

        let handler = MemberChangeEmailCommand.makeHandler contextMock.Object

        let command = MemberChangeEmailCommand.make id incorrectPassword newEmail
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsIncorrectPassword =! true
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handle, with unavailable email, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let unavailableEmail = EmailAddress.random 32

        let passwordData = Crypto.hash currentPassword 100
        let memberData =
            makeTestMemberData2 id (String.random 32) passwordData unavailableEmail (String.random 64)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(Some(memberData)).Verifiable()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberEmail(any (), considerDeactivated = true)).Returns(true).Verifiable()

        let handler = MemberChangeEmailCommand.makeHandler contextMock.Object

        let command = MemberChangeEmailCommand.make id currentPassword unavailableEmail
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsUnavailableEmail =! true
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling handle, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32
        let newEmail = EmailAddress.random 32

        let passwordData = Crypto.hash currentPassword 100
        let memberData =
            makeTestMemberData2 id (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(Some(memberData)).Verifiable()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberEmail(any (), considerDeactivated = true)).Returns(false).Verifiable()

        let handler = MemberChangeEmailCommand.makeHandler contextMock.Object

        let command = MemberChangeEmailCommand.make id currentPassword newEmail
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableEmail =! false
