namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.UnitTest
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberChangePasswordCommandTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangePasswordCommand.make Guid.empty (Password.random 32) (Password.random 32) @>

    [<Test>]
    let ``Calling make, with null new password, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) null @>

    [<Test>]
    let ``Calling make, with empty new password, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) String.empty @>

    [<Test>]
    let ``Calling make, with new password shorter than 8 characters, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password is shorter than 8 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) (Password.random 7) @>

    [<Test>]
    let ``Calling make, with new password longer than 32 characters, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password is longer than 32 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) (Password.random 33) @>

    [<Test>]
    let ``Calling make, with new password without digits, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password does not contain any numbers"

        let noDigitsNewPassword = makeBadPasswordWithoutDigits 32

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) noDigitsNewPassword @>

    [<Test>]
    let ``Calling make, with new password without lower alphas, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password does not contain any lower-case letters"

        let noLowerAlphasNewPassword = makeBadPasswordWithoutLowerAlphas 32

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) noLowerAlphasNewPassword @>

    [<Test>]
    let ``Calling make, with new password without upper alphas, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password does not contain any upper-case letters"

        let noUpperAlphasNewPassword = makeBadPasswordWithoutUpperAlphas 32

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) noUpperAlphasNewPassword @>

    [<Test>]
    let ``Calling make, with new password without special chars, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password does not contain any special characters"

        let noSpecialCharsNewPassword = makeBadPasswordWithoutSpecialChars 32

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) noSpecialCharsNewPassword @>

    [<Test>]
    let ``Calling make, with new password without invalid chars, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password contains invalid characters"

        let invalidCharsNewPassword = makeBadPasswordWithInvalidChars 32

        testArgumentException expectedParamName expectedMessage
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) invalidCharsNewPassword @>

    [<Test>]
    let ``Calling make, with valid parameters, returns command`` () =
        let command = MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) (Password.random 32)

        box command :? IRequest<IMemberChangePasswordCommandResult> =! true

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let command = MemberChangePasswordCommand.make expectedId (Password.random 32) (Password.random 32)

        command.Id =! expectedId

    [<Test>]
    let ``Calling current password, returns expected current password`` () =
        let expectedCurrentPassword = Password.random 32

        let command =
            MemberChangePasswordCommand.make (Guid.random ()) expectedCurrentPassword (Password.random 32)

        command.CurrentPassword =! expectedCurrentPassword

    [<Test>]
    let ``Calling new password, returns expected new password`` () =
        let expectedNewPassword = Password.random 32

        let command = MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) expectedNewPassword

        command.NewPassword =! expectedNewPassword
