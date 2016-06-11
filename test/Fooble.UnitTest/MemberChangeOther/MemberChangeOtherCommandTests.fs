namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberChangeOtherCommandTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeOtherCommand.make Guid.empty (String.random 32) @>

    [<Test>]
    let ``Calling make, with null new nickname, raises expected exception`` () =
        let expectedParamName = "newNickname"
        let expectedMessage = "New nickname is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeOtherCommand.make (Guid.random ()) null @>

    [<Test>]
    let ``Calling make, with empty new nickname, raises expected exception`` () =
        let expectedParamName = "newNickname"
        let expectedMessage = "New nickname is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeOtherCommand.make (Guid.random ()) String.empty @>

    [<Test>]
    let ``Calling make, with new nickname longer than 64 characters, raises expected exception`` () =
        let expectedParamName = "newNickname"
        let expectedMessage = "New nickname is longer than 64 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeOtherCommand.make (Guid.random ()) (String.random 65) @>

    [<Test>]
    let ``Calling make, with valid parameters, returns command`` () =
        let command = MemberChangeOtherCommand.make (Guid.random ()) (String.random 32)

        box command :? IRequest<IMemberChangeOtherCommandResult> =! true

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let command = MemberChangeOtherCommand.make expectedId (String.random 32)

        command.Id =! expectedId

    [<Test>]
    let ``Calling new nickname, returns expected new nickname`` () =
        let expectedNewNickname = String.random 32

        let command = MemberChangeOtherCommand.make (Guid.random ()) expectedNewNickname

        command.NewNickname =! expectedNewNickname
