namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.UnitTest
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberRegisterCommandTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make Guid.empty (String.random 32) (Password.random 32)
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with null username, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) null (Password.random 32) (EmailAddress.random 32)
                (String.random 64) @>

    [<Test>]
    let ``Calling make, with empty username, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) String.empty (Password.random 32)
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with username shorter than 3 characters, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is shorter than 3 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 2) (Password.random 32)
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with username longer than 32 characters, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is longer than 32 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 33) (Password.random 32)
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with username in invalid format, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is not in the correct format (lowercase alphanumeric)"

        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) invalidFormatUsername (Password.random 32)
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with null password, raises expected exception`` () =
        let expectedParamName = "password"
        let expectedMessage = "Password is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) null (EmailAddress.random 32)
                (String.random 64) @>

    [<Test>]
    let ``Calling make, with empty password, raises expected exception`` () =
        let expectedParamName = "password"
        let expectedMessage = "Password is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) String.empty
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with password shorter than 8 characters, raises expected exception`` () =
        let expectedParamName = "password"
        let expectedMessage = "Password is shorter than 8 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 7)
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with password longer than 32 characters, raises expected exception`` () =
        let expectedParamName = "password"
        let expectedMessage = "Password is longer than 32 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 33)
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with password without digits, raises expected exception`` () =
        let expectedParamName = "password"
        let expectedMessage = "Password does not contain any numbers"

        let noDigitsPassword = makeBadPasswordWithoutDigits 32
        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) noDigitsPassword
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with password without lower alphas, raises expected exception`` () =
        let expectedParamName = "password"
        let expectedMessage = "Password does not contain any lower-case letters"

        let noLowerAlphasPassword = makeBadPasswordWithoutLowerAlphas 32
        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) noLowerAlphasPassword
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with password without upper alphas, raises expected exception`` () =
        let expectedParamName = "password"
        let expectedMessage = "Password does not contain any upper-case letters"

        let noUpperAlphasPassword = makeBadPasswordWithoutUpperAlphas 32
        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) noUpperAlphasPassword
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with password without special chars, raises expected exception`` () =
        let expectedParamName = "password"
        let expectedMessage = "Password does not contain any special characters"

        let noSpecialCharsPassword = makeBadPasswordWithoutSpecialChars 32
        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) noSpecialCharsPassword
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with password without invalid chars, raises expected exception`` () =
        let expectedParamName = "password"
        let expectedMessage = "Password contains invalid characters"

        let invalidCharsPassword = makeBadPasswordWithInvalidChars 32
        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) invalidCharsPassword
                (EmailAddress.random 32) (String.random 64) @>

    [<Test>]
    let ``Calling make, with null email, raises expected exception`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32) null
                (String.random 64) @>

    [<Test>]
    let ``Calling make, with empty email, raises expected exception`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32) String.empty
                (String.random 64) @>

    [<Test>]
    let ``Calling make, with email longer than 254 characters, raises expected exception`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is longer than 254 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (String.random 255) (String.random 64) @>

    [<Test>]
    let ``Calling make, with email in invalid format, raises expected exception`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is not in the correct format"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (String.random 64) (String.random 64) @>

    [<Test>]
    let ``Calling make, with null nickname, raises expected exception`` () =
        let expectedParamName = "nickname"
        let expectedMessage = "Nickname is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (EmailAddress.random 32) null @>

    [<Test>]
    let ``Calling make, with empty nickname, raises expected exception`` () =
        let expectedParamName = "nickname"
        let expectedMessage = "Nickname is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (EmailAddress.random 32) String.empty @>

    [<Test>]
    let ``Calling make, with nickname longer than 64 characters, raises expected exception`` () =
        let expectedParamName = "nickname"
        let expectedMessage = "Nickname is longer than 64 characters"

        testArgumentException expectedParamName expectedMessage
            <@ MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (EmailAddress.random 32) (String.random 65) @>

    [<Test>]
    let ``Calling make, with valid parameters, returns command`` () =
        let command =
            MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (EmailAddress.random 32) (String.random 64)

        box command :? IRequest<IMemberRegisterCommandResult> =! true

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let command =
            MemberRegisterCommand.make expectedId (String.random 32) (Password.random 32) (EmailAddress.random 32)
                (String.random 64)

        command.Id =! expectedId

    [<Test>]
    let ``Calling username, returns expected username`` () =
        let expectedUsername = String.random 32

        let command =
            MemberRegisterCommand.make (Guid.random ()) expectedUsername (Password.random 32)
                (EmailAddress.random 32) (String.random 64)

        command.Username =! expectedUsername

    [<Test>]
    let ``Calling email, returns expected email`` () =
        let expectedEmail = EmailAddress.random 32

        let command =
            MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32) expectedEmail
                (String.random 64)

        command.Email =! expectedEmail

    [<Test>]
    let ``Calling nickname, returns expected nickname`` () =
        let expectedNickname = String.random 64

        let command =
            MemberRegisterCommand.make (Guid.random ()) (String.random 32) (Password.random 32)
                (EmailAddress.random 32) expectedNickname

        command.Nickname =! expectedNickname
