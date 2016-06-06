namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.UnitTest
open MediatR
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberChangePasswordCommandTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make Guid.empty (Password.random 32) (Password.random 32) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with null current password, raises expected exception`` () =
        let expectedParamName = "currentPassword"
        let expectedMessage = "Current password is required"

        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) null (Password.random 32) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with empty current password, raises expected exception`` () =
        let expectedParamName = "currentPassword"
        let expectedMessage = "Current password is required"

        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) String.empty (Password.random 32) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with current password shorter than 8 characters, raises expected exception`` () =
        let expectedParamName = "currentPassword"
        let expectedMessage = "Current password is shorter than 8 characters"

        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 7) (Password.random 32) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with current password longer than 32 characters, raises expected exception`` () =
        let expectedParamName = "currentPassword"
        let expectedMessage = "Current password is longer than 32 characters"

        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 33) (Password.random 32) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with current password without digits, raises expected exception`` () =
        let expectedParamName = "currentPassword"
        let expectedMessage = "Current password does not contain any numbers"

        let noDigitsCurrentPassword = makeBadPasswordWithoutDigits 32
        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) noDigitsCurrentPassword (Password.random 32) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with current password without lower alphas, raises expected exception`` () =
        let expectedParamName = "currentPassword"
        let expectedMessage = "Current password does not contain any lower-case letters"

        let noLowerAlphasCurrentPassword = makeBadPasswordWithoutLowerAlphas 32
        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) noLowerAlphasCurrentPassword
                (Password.random 32) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with current password without upper alphas, raises expected exception`` () =
        let expectedParamName = "currentPassword"
        let expectedMessage = "Current password does not contain any upper-case letters"

        let noUpperAlphasCurrentPassword = makeBadPasswordWithoutUpperAlphas 32
        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) noUpperAlphasCurrentPassword
                (Password.random 32) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with current password without special chars, raises expected exception`` () =
        let expectedParamName = "currentPassword"
        let expectedMessage = "Current password does not contain any special characters"

        let noSpecialCharsCurrentPassword = makeBadPasswordWithoutSpecialChars 32
        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) noSpecialCharsCurrentPassword
                (Password.random 32) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with current password without invalid chars, raises expected exception`` () =
        let expectedParamName = "currentPassword"
        let expectedMessage = "Current password contains invalid characters"

        let invalidCharsCurrentPassword = makeBadPasswordWithInvalidChars 32
        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) invalidCharsCurrentPassword
                (Password.random 32) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with null new password, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password is required"

        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) null @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with empty new password, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password is required"

        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) String.empty @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with new password shorter than 8 characters, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password is shorter than 8 characters"

        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) (Password.random 7) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with new password longer than 32 characters, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password is longer than 32 characters"

        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) (Password.random 33) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with new password without digits, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password does not contain any numbers"

        let noDigitsNewPassword = makeBadPasswordWithoutDigits 32
        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) noDigitsNewPassword @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with new password without lower alphas, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password does not contain any lower-case letters"

        let noLowerAlphasNewPassword = makeBadPasswordWithoutLowerAlphas 32
        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) noLowerAlphasNewPassword @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with new password without upper alphas, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password does not contain any upper-case letters"

        let noUpperAlphasNewPassword = makeBadPasswordWithoutUpperAlphas 32
        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) noUpperAlphasNewPassword @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with new password without special chars, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password does not contain any special characters"

        let noSpecialCharsNewPassword = makeBadPasswordWithoutSpecialChars 32
        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) noSpecialCharsNewPassword @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with new password without invalid chars, raises expected exception`` () =
        let expectedParamName = "newPassword"
        let expectedMessage = "New password contains invalid characters"

        let invalidCharsNewPassword = makeBadPasswordWithInvalidChars 32
        raisesWith<ArgumentException>
            <@ MemberChangePasswordCommand.make (Guid.random ()) (Password.random 32) invalidCharsNewPassword @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

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
