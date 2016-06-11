namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberChangeUsernameCommandTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeUsernameCommand.make Guid.empty (Password.random 32) (String.random 32) @>

    [<Test>]
    let ``Calling make, with null new username, raises expected exception`` () =
        let expectedParamName = "newUsername"
        let expectedMessage = "New username is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeUsernameCommand.make (Guid.random ()) (Password.random 32) null @>

    [<Test>]
    let ``Calling make, with empty new username, raises expected exception`` () =
        let expectedParamName = "newUsername"
        let expectedMessage = "New username is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeUsernameCommand.make (Guid.random ()) (Password.random 32) String.empty @>

    [<Test>]
    let ``Calling make, with new username shorter than 3 characters, raises expected exception`` () =
        let expectedParamName = "newUsername"
        let expectedMessage = "New username is shorter than 3 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeUsernameCommand.make (Guid.random ()) (Password.random 32) (String.random 2) @>

    [<Test>]
    let ``Calling make, with new username longer than 32 characters, raises expected exception`` () =
        let expectedParamName = "newUsername"
        let expectedMessage = "New username is longer than 32 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeUsernameCommand.make (Guid.random ()) (Password.random 32) (String.random 33) @>

    [<Test>]
    let ``Calling make, with new username in invalid format, raises expected exception`` () =
        let expectedParamName = "newUsername"
        let expectedMessage = "New username is not in the correct format (lowercase alphanumeric)"

        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeUsernameCommand.make (Guid.random ()) (Password.random 32) invalidFormatUsername @>

    [<Test>]
    let ``Calling make, with valid parameters, returns command`` () =
        let command = MemberChangeUsernameCommand.make (Guid.random ()) (Password.random 32) (String.random 32)

        box command :? IRequest<IMemberChangeUsernameCommandResult> =! true

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let command = MemberChangeUsernameCommand.make expectedId (Password.random 32) (String.random 32)

        command.Id =! expectedId

    [<Test>]
    let ``Calling current password, returns expected current password`` () =
        let expectedCurrentPassword = String.random 32

        let command =
            MemberChangeUsernameCommand.make (Guid.random ()) expectedCurrentPassword (String.random 32)

        command.CurrentPassword =! expectedCurrentPassword

    [<Test>]
    let ``Calling new username, returns expected new username`` () =
        let expectedNewUsername = String.random 32

        let command = MemberChangeUsernameCommand.make (Guid.random ()) (Password.random 32) expectedNewUsername

        command.NewUsername =! expectedNewUsername
