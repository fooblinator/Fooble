namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDeactivateCommandHandlerTests =

    [<Test>]
    let ``Calling handle, with id not found in data store, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any (), considerDeactivated = false)).Returns(None).Verifiable()

        let handler = MemberDeactivateCommand.makeHandler contextMock.Object

        let command = MemberDeactivateCommand.make id currentPassword
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsNotFound =! true
        commandResult.IsSuccess =! false
        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling handle, with incorrect password, returns expected result`` () =
        let id = Guid.random ()
        let incorrectPassword = Password.random 32

        let passwordData = Crypto.hash (Password.random 32) 100
        let memberData =
            makeTestMemberData2 id (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(Some(memberData)).Verifiable()

        let handler = MemberDeactivateCommand.makeHandler contextMock.Object

        let command = MemberDeactivateCommand.make id incorrectPassword
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsIncorrectPassword =! true
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling handle, with successful parameters, returns expected result`` () =
        let id = Guid.random ()
        let currentPassword = Password.random 32

        let passwordData = Crypto.hash currentPassword 100
        let memberData =
            makeTestMemberData2 id (String.random 32) passwordData (EmailAddress.random 32) (String.random 64)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.GetMember(any (), considerDeactivated = false)).Returns(Some(memberData)).Verifiable()

        let handler = MemberDeactivateCommand.makeHandler contextMock.Object

        let command = MemberDeactivateCommand.make id currentPassword
        let commandResult = handler.Handle(command)

        contextMock.Verify()

        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
