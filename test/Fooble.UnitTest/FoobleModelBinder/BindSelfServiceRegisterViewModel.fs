namespace Fooble.UnitTest

open Fooble.Common
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module BindSelfServiceRegisterViewModel =

    [<Test>]
    let ``Binding to a self-service register view model, with null username, adds expected model state error`` () =
        let nullUsername:string = null
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel nullUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel nullUsername expectedPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "username" "Username is required"

    [<Test>]
    let ``Binding to a self-service register view model, with empty username, adds expected model state error`` () =
        let emptyUsername = String.empty
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel emptyUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel emptyUsername expectedPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "username" "Username is required"

    [<Test>]
    let ``Binding to a self-service register view model, with username shorter than 3 characters, adds expected model state error`` () =
        let shortUsername = String.random 2
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel shortUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel shortUsername expectedPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "username" "Username is shorter than 3 characters"

    [<Test>]
    let ``Binding to a self-service register view model, with username longer than 32 characters, adds expected model state error`` () =
        let longUsername = String.random 33
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel longUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel longUsername expectedPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "username" "Username is longer than 32 characters"

    [<Test>]
    let ``Binding to a self-service register view model, with username in invalid format, adds expected model state error`` () =
        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel invalidFormatUsername expectedPassword expectedConfirmPassword
                expectedEmail expectedNickname

        testSelfServiceRegisterViewModel actualViewModel invalidFormatUsername expectedPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "username" "Username is not in the correct format (lowercase alphanumeric)"

    [<Test>]
    let ``Binding to a self-service register view model, with null password, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let nullPassword:string = null
        let expectedConfirmPassword = nullPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername nullPassword expectedConfirmPassword expectedEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername nullPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Binding to a self-service register view model, with empty password, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let emptyPassword = String.empty
        let expectedConfirmPassword = emptyPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername emptyPassword expectedConfirmPassword expectedEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername emptyPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Binding to a self-service register view model, with password shorter than 8 characters, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let shortPassword = Password.random 7
        let expectedConfirmPassword = shortPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername shortPassword expectedConfirmPassword expectedEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername shortPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "password" "Password is shorter than 8 characters"

    [<Test>]
    let ``Binding to a self-service register view model, with password longer than 32 characters, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let longPassword = Password.random 33
        let expectedConfirmPassword = longPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername longPassword expectedConfirmPassword expectedEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername longPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "password" "Password is longer than 32 characters"

    [<Test>]
    let ``Binding to a self-service register view model, with password without digits, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let noDigitsPassword = makeBadPasswordWithoutDigits 32
        let expectedConfirmPassword = noDigitsPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername noDigitsPassword expectedConfirmPassword expectedEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername noDigitsPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "password" "Password does not contain any numbers"

    [<Test>]
    let ``Binding to a self-service register view model, with password without lower alphas, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let noLowerAlphasPassword = makeBadPasswordWithoutLowerAlphas 32
        let expectedConfirmPassword = noLowerAlphasPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername noLowerAlphasPassword expectedConfirmPassword
                expectedEmail expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername noLowerAlphasPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "password" "Password does not contain any lower-case letters"

    [<Test>]
    let ``Binding to a self-service register view model, with password without upper alphas, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let noUpperAlphasPassword = makeBadPasswordWithoutUpperAlphas 32
        let expectedConfirmPassword = noUpperAlphasPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername noUpperAlphasPassword expectedConfirmPassword
                expectedEmail expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername noUpperAlphasPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "password" "Password does not contain any upper-case letters"

    [<Test>]
    let ``Binding to a self-service register view model, with password without special chars, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let noSpecialCharsPassword = makeBadPasswordWithoutSpecialChars 32
        let expectedConfirmPassword = noSpecialCharsPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername noSpecialCharsPassword expectedConfirmPassword
                expectedEmail expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername noSpecialCharsPassword
            expectedConfirmPassword expectedEmail expectedNickname

        testModelState modelState "password" "Password does not contain any special characters"

    [<Test>]
    let ``Binding to a self-service register view model, with password without invalid chars, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let invalidCharsPassword = makeBadPasswordWithInvalidChars 32
        let expectedConfirmPassword = invalidCharsPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername invalidCharsPassword expectedConfirmPassword
                expectedEmail expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername invalidCharsPassword expectedConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "password" "Password contains invalid characters"

    [<Test>]
    let ``Binding to a self-service register view model, with non-matching passwords, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let nonMatchingConfirmPassword = Password.random 32
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername expectedPassword nonMatchingConfirmPassword expectedEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername expectedPassword nonMatchingConfirmPassword
            expectedEmail expectedNickname

        testModelState modelState "confirmPassword" "Passwords do not match"

    [<Test>]
    let ``Binding to a self-service register view model, with null email, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let nullEmail:string = null
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername expectedPassword expectedConfirmPassword nullEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername expectedPassword expectedConfirmPassword
            nullEmail expectedNickname

        testModelState modelState "email" "Email is required"

    [<Test>]
    let ``Binding to a self-service register view model, with empty email, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let emptyEmail = String.empty
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername expectedPassword expectedConfirmPassword emptyEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername expectedPassword expectedConfirmPassword
            emptyEmail expectedNickname

        testModelState modelState "email" "Email is required"

    [<Test>]
    let ``Binding to a self-service register view model, with email longer than 254 characters, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let longEmail = String.random 255
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername expectedPassword expectedConfirmPassword longEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername expectedPassword expectedConfirmPassword
            longEmail expectedNickname

        testModelState modelState "email" "Email is longer than 254 characters"

    [<Test>]
    let ``Binding to a self-service register view model, with email in invalid format, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let invalidFormatEmail = String.random 64
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername expectedPassword expectedConfirmPassword invalidFormatEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername expectedPassword expectedConfirmPassword
            invalidFormatEmail expectedNickname

        testModelState modelState "email" "Email is not in the correct format"

    [<Test>]
    let ``Binding to a self-service register view model, with null nickname, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let nullNickname:string = null

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername expectedPassword expectedConfirmPassword expectedEmail
                nullNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername expectedPassword expectedConfirmPassword
            expectedEmail nullNickname

        testModelState modelState "nickname" "Nickname is required"

    [<Test>]
    let ``Binding to a self-service register view model, with empty nickname, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let emptyNickname = String.empty

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername expectedPassword expectedConfirmPassword expectedEmail
                emptyNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername expectedPassword expectedConfirmPassword
            expectedEmail emptyNickname

        testModelState modelState "nickname" "Nickname is required"

    [<Test>]
    let ``Binding to a self-service register view model, with nickname longer than 64 characters, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let longNickname = String.random 65

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername expectedPassword expectedConfirmPassword expectedEmail
                longNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername expectedPassword expectedConfirmPassword
            expectedEmail longNickname

        testModelState modelState "nickname" "Nickname is longer than 64 characters"

    [<Test>]
    let ``Binding to a self-service register view model, with valid parameters, adds no model state errors`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            bindSelfServiceRegisterViewModel expectedUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname

        testSelfServiceRegisterViewModel actualViewModel expectedUsername expectedPassword expectedConfirmPassword
            expectedEmail expectedNickname

        modelState.IsValid =! true
