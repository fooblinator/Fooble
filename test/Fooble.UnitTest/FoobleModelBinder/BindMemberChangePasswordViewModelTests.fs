namespace Fooble.UnitTest

open Fooble.Common
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module BindMemberChangePasswordViewModelTests =

    [<Test>]
    let ``Binding to a member change password view model, with null new password, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let nullNewPassword:string = null
        let expectedConfirmPassword = nullNewPassword

        let (actualViewModel, modelState) =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword nullNewPassword
                expectedConfirmPassword

        testMemberChangePasswordViewModel actualViewModel expectedId expectedCurrentPassword nullNewPassword
            expectedConfirmPassword

        testModelState modelState "newPassword" "New password is required"

    [<Test>]
    let ``Binding to a member change password view model, with empty new password, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let emptyNewPassword = String.empty
        let expectedConfirmPassword = emptyNewPassword

        let (actualViewModel, modelState) =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword emptyNewPassword
                expectedConfirmPassword

        testMemberChangePasswordViewModel actualViewModel expectedId expectedCurrentPassword emptyNewPassword
            expectedConfirmPassword

        testModelState modelState "newPassword" "New password is required"

    [<Test>]
    let ``Binding to a member change password view model, with new password shorter than 8 characters, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let shortNewPassword = Password.random 7
        let expectedConfirmPassword = shortNewPassword

        let (actualViewModel, modelState) =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword shortNewPassword
                expectedConfirmPassword

        testMemberChangePasswordViewModel actualViewModel expectedId expectedCurrentPassword shortNewPassword
            expectedConfirmPassword

        testModelState modelState "newPassword" "New password is shorter than 8 characters"

    [<Test>]
    let ``Binding to a member change password view model, with new password longer than 32 characters, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let longNewPassword = Password.random 33
        let expectedConfirmPassword = longNewPassword

        let (actualViewModel, modelState) =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword longNewPassword
                expectedConfirmPassword

        testMemberChangePasswordViewModel actualViewModel expectedId expectedCurrentPassword longNewPassword
            expectedConfirmPassword

        testModelState modelState "newPassword" "New password is longer than 32 characters"

    [<Test>]
    let ``Binding to a member change password view model, with new password without digits, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let noDigitsNewPassword = makeBadPasswordWithoutDigits 32
        let expectedConfirmPassword = noDigitsNewPassword

        let (actualViewModel, modelState) =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword noDigitsNewPassword
                expectedConfirmPassword

        testMemberChangePasswordViewModel actualViewModel expectedId expectedCurrentPassword noDigitsNewPassword
            expectedConfirmPassword

        testModelState modelState "newPassword" "New password does not contain any numbers"

    [<Test>]
    let ``Binding to a member change password view model, with new password without lower alphas, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let noLowerAlphasNewPassword = makeBadPasswordWithoutLowerAlphas 32
        let expectedConfirmPassword = noLowerAlphasNewPassword

        let (actualViewModel, modelState) =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword noLowerAlphasNewPassword
                expectedConfirmPassword

        testMemberChangePasswordViewModel actualViewModel expectedId expectedCurrentPassword
            noLowerAlphasNewPassword expectedConfirmPassword

        testModelState modelState "newPassword" "New password does not contain any lower-case letters"

    [<Test>]
    let ``Binding to a member change password view model, with new password without upper alphas, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let noUpperAlphasNewPassword = makeBadPasswordWithoutUpperAlphas 32
        let expectedConfirmPassword = noUpperAlphasNewPassword

        let (actualViewModel, modelState) =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword noUpperAlphasNewPassword
                expectedConfirmPassword

        testMemberChangePasswordViewModel actualViewModel expectedId expectedCurrentPassword
            noUpperAlphasNewPassword expectedConfirmPassword

        testModelState modelState "newPassword" "New password does not contain any upper-case letters"

    [<Test>]
    let ``Binding to a member change password view model, with new password without special chars, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let noSpecialCharsNewPassword = makeBadPasswordWithoutSpecialChars 32
        let expectedConfirmPassword = noSpecialCharsNewPassword

        let (actualViewModel, modelState) =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword noSpecialCharsNewPassword
                expectedConfirmPassword

        testMemberChangePasswordViewModel actualViewModel expectedId expectedCurrentPassword
            noSpecialCharsNewPassword expectedConfirmPassword

        testModelState modelState "newPassword" "New password does not contain any special characters"

    [<Test>]
    let ``Binding to a member change password view model, with new password without invalid chars, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let invalidCharsNewPassword = makeBadPasswordWithInvalidChars 32
        let expectedConfirmPassword = invalidCharsNewPassword

        let (actualViewModel, modelState) =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword invalidCharsNewPassword
                expectedConfirmPassword

        testMemberChangePasswordViewModel actualViewModel expectedId expectedCurrentPassword
            invalidCharsNewPassword expectedConfirmPassword

        testModelState modelState "newPassword" "New password contains invalid characters"

    [<Test>]
    let ``Binding to a member change password view model, with non-matching passwords, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let nonMatchingConfirmPassword = Password.random 32

        let (actualViewModel, modelState) =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword expectedNewPassword
                nonMatchingConfirmPassword

        testMemberChangePasswordViewModel actualViewModel expectedId expectedCurrentPassword expectedNewPassword
            nonMatchingConfirmPassword

        testModelState modelState "confirmPassword" "Passwords do not match"

    [<Test>]
    let ``Binding to a member change password view model, with valid parameters, adds no model state errors`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewPassword = Password.random 32
        let expectedConfirmPassword = expectedNewPassword

        let (actualViewModel, modelState) =
            bindMemberChangePasswordViewModel expectedId expectedCurrentPassword expectedNewPassword
                expectedConfirmPassword

        testMemberChangePasswordViewModel actualViewModel expectedId expectedCurrentPassword expectedNewPassword
            expectedConfirmPassword

        modelState.IsValid =! true
