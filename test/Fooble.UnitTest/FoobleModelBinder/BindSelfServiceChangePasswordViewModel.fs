namespace Fooble.UnitTest

open Fooble.Common
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module BindSelfServiceChangePasswordViewModel =

    [<Test>]
    let ``Binding to a self-service change password view model, with null current password, adds expected model state error`` () =
        let nullCurrentPassword:string = null
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel nullCurrentPassword expectedNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel nullCurrentPassword expectedNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Binding to a self-service change password view model, with empty current password, adds expected model state error`` () =
        let emptyCurrentPassword = String.empty
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel emptyCurrentPassword expectedNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel emptyCurrentPassword expectedNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Binding to a self-service change password view model, with current password shorter than 8 characters, adds expected model state error`` () =
        let shortCurrentPassword = Password.random 7
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel shortCurrentPassword expectedNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel shortCurrentPassword expectedNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password is shorter than 8 characters"

    [<Test>]
    let ``Binding to a self-service change password view model, with current password longer than 32 characters, adds expected model state error`` () =
        let longCurrentPassword = Password.random 33
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel longCurrentPassword expectedNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel longCurrentPassword expectedNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password is longer than 32 characters"

    [<Test>]
    let ``Binding to a self-service change password view model, with current password without digits, adds expected model state error`` () =
        let noDigitsCurrentPassword = makeBadPasswordWithoutDigits 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel noDigitsCurrentPassword expectedNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel noDigitsCurrentPassword expectedNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password does not contain any numbers"

    [<Test>]
    let ``Binding to a self-service change password view model, with current password without lower alphas, adds expected model state error`` () =
        let noLowerAlphasCurrentPassword = makeBadPasswordWithoutLowerAlphas 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel noLowerAlphasCurrentPassword expectedNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel noLowerAlphasCurrentPassword expectedNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password does not contain any lower-case letters"

    [<Test>]
    let ``Binding to a self-service change password view model, with current password without upper alphas, adds expected model state error`` () =
        let noUpperAlphasCurrentPassword = makeBadPasswordWithoutUpperAlphas 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel noUpperAlphasCurrentPassword expectedNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel noUpperAlphasCurrentPassword expectedNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password does not contain any upper-case letters"

    [<Test>]
    let ``Binding to a self-service change password view model, with current password without special chars, adds expected model state error`` () =
        let noSpecialCharsCurrentPassword = makeBadPasswordWithoutSpecialChars 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel noSpecialCharsCurrentPassword expectedNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel noSpecialCharsCurrentPassword expectedNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password does not contain any special characters"

    [<Test>]
    let ``Binding to a self-service change password view model, with current password without invalid chars, adds expected model state error`` () =
        let invalidCharsCurrentPassword = makeBadPasswordWithInvalidChars 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel invalidCharsCurrentPassword expectedNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel invalidCharsCurrentPassword expectedNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password contains invalid characters"

    [<Test>]
    let ``Binding to a self-service change password view model, with null new password, adds expected model state error`` () =
        let expectedCurrentPassword = Password.random 32
        let nullNewPassword:string = null
        let expectedConfirmPassword = nullNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel expectedCurrentPassword nullNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel expectedCurrentPassword nullNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Binding to a self-service change password view model, with empty new password, adds expected model state error`` () =
        let expectedCurrentPassword = Password.random 32
        let emptyNewPassword = String.empty
        let expectedConfirmPassword = emptyNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel expectedCurrentPassword emptyNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel expectedCurrentPassword emptyNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Binding to a self-service change password view model, with new password shorter than 8 characters, adds expected model state error`` () =
        let expectedCurrentPassword = Password.random 32
        let shortNewPassword = Password.random 7
        let expectedConfirmPassword = shortNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel expectedCurrentPassword shortNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel expectedCurrentPassword shortNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password is shorter than 8 characters"

    [<Test>]
    let ``Binding to a self-service change password view model, with new password longer than 32 characters, adds expected model state error`` () =
        let expectedCurrentPassword = Password.random 32
        let longNewPassword = Password.random 33
        let expectedConfirmPassword = longNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel expectedCurrentPassword longNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel expectedCurrentPassword longNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password is longer than 32 characters"

    [<Test>]
    let ``Binding to a self-service change password view model, with new password without digits, adds expected model state error`` () =
        let expectedCurrentPassword = Password.random 32
        let noDigitsNewPassword = makeBadPasswordWithoutDigits 32
        let expectedConfirmPassword = noDigitsNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel expectedCurrentPassword noDigitsNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel expectedCurrentPassword noDigitsNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password does not contain any numbers"

    [<Test>]
    let ``Binding to a self-service change password view model, with new password without lower alphas, adds expected model state error`` () =
        let expectedCurrentPassword = Password.random 32
        let noLowerAlphasNewPassword = makeBadPasswordWithoutLowerAlphas 32
        let expectedConfirmPassword = noLowerAlphasNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel expectedCurrentPassword noLowerAlphasNewPassword
                expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel expectedCurrentPassword noLowerAlphasNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password does not contain any lower-case letters"

    [<Test>]
    let ``Binding to a self-service change password view model, with new password without upper alphas, adds expected model state error`` () =
        let expectedCurrentPassword = Password.random 32
        let noUpperAlphasNewPassword = makeBadPasswordWithoutUpperAlphas 32
        let expectedConfirmPassword = noUpperAlphasNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel expectedCurrentPassword noUpperAlphasNewPassword
                expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel expectedCurrentPassword noUpperAlphasNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password does not contain any upper-case letters"

    [<Test>]
    let ``Binding to a self-service change password view model, with new password without special chars, adds expected model state error`` () =
        let expectedCurrentPassword = Password.random 32
        let noSpecialCharsNewPassword = makeBadPasswordWithoutSpecialChars 32
        let expectedConfirmPassword = noSpecialCharsNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel expectedCurrentPassword noSpecialCharsNewPassword
                expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel expectedCurrentPassword noSpecialCharsNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password does not contain any special characters"

    [<Test>]
    let ``Binding to a self-service change password view model, with new password without invalid chars, adds expected model state error`` () =
        let expectedCurrentPassword = Password.random 32
        let invalidCharsNewPassword = makeBadPasswordWithInvalidChars 32
        let expectedConfirmPassword = invalidCharsNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel expectedCurrentPassword invalidCharsNewPassword
                expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel expectedCurrentPassword invalidCharsNewPassword
            expectedConfirmPassword

        testModelState modelState "password" "Password contains invalid characters"

    [<Test>]
    let ``Binding to a self-service change password view model, with non-matching passwords, adds expected model state error`` () =
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let nonMatchingConfirmPassword = Password.random 32

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel expectedCurrentPassword expectedNewPassword
                nonMatchingConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel expectedCurrentPassword expectedNewPassword
            nonMatchingConfirmPassword

        testModelState modelState "confirmPassword" "Passwords do not match"

    [<Test>]
    let ``Binding to a self-service change password view model, with valid parameters, adds no model state errors`` () =
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let (actualViewModel, modelState) =
            bindSelfServiceChangePasswordViewModel expectedCurrentPassword expectedNewPassword expectedConfirmPassword

        testSelfServiceChangePasswordViewModel actualViewModel expectedCurrentPassword expectedNewPassword
            expectedConfirmPassword

        modelState.IsValid =! true
