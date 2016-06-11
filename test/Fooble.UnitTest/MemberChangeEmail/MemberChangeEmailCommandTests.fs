namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberChangeEmailCommandTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeEmailCommand.make Guid.empty (Password.random 32) (EmailAddress.random 32) @>

    [<Test>]
    let ``Calling make, with null new email, raises expected exception`` () =
        let expectedParamName = "newEmail"
        let expectedMessage = "New email is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeEmailCommand.make (Guid.random ()) (Password.random 32) null @>

    [<Test>]
    let ``Calling make, with empty new email, raises expected exception`` () =
        let expectedParamName = "newEmail"
        let expectedMessage = "New email is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeEmailCommand.make (Guid.random ()) (Password.random 32) String.empty @>

    [<Test>]
    let ``Calling make, with new email longer than 254 characters, raises expected exception`` () =
        let expectedParamName = "newEmail"
        let expectedMessage = "New email is longer than 254 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeEmailCommand.make (Guid.random ()) (Password.random 32) (EmailAddress.random 255) @>

    [<Test>]
    let ``Calling make, with new email in invalid format, raises expected exception`` () =
        let expectedParamName = "newEmail"
        let expectedMessage = "New email is not in the correct format"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangeEmailCommand.make (Guid.random ()) (Password.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with valid parameters, returns command`` () =
        let command = MemberChangeEmailCommand.make (Guid.random ()) (Password.random 32) (EmailAddress.random 32)

        box command :? IRequest<IMemberChangeEmailCommandResult> =! true

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let command = MemberChangeEmailCommand.make expectedId (Password.random 32) (EmailAddress.random 32)

        command.Id =! expectedId

    [<Test>]
    let ``Calling current password, returns expected current password`` () =
        let expectedCurrentPassword = EmailAddress.random 32

        let command =
            MemberChangeEmailCommand.make (Guid.random ()) expectedCurrentPassword (EmailAddress.random 32)

        command.CurrentPassword =! expectedCurrentPassword

    [<Test>]
    let ``Calling new email, returns expected new email`` () =
        let expectedNewEmail = EmailAddress.random 32

        let command = MemberChangeEmailCommand.make (Guid.random ()) (Password.random 32) expectedNewEmail

        command.NewEmail =! expectedNewEmail
