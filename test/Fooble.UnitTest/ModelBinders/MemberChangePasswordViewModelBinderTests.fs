namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open Moq
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberChangePasswordViewModelBinderTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore (MemberChangePasswordViewModelBinder(Mock.Of<MemberChangePasswordViewModelFactory>()))

    [<Test>]
    let ``Constructing, with null change password view model factory, raises expected exception`` () =
        testArgumentException "viewModelFactory" "Member change password view model factory is required"
            <@ MemberChangePasswordViewModelBinder(null) @>

    [<Test>]
    let ``Binding member change password view model, with null password, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let nullPassword:string = null
        let confirmPassword = nullPassword
        let (viewModel, modelState) =
            bindMemberChangePasswordViewModel id currentPassword nullPassword confirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword nullPassword confirmPassword
        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Binding member change password view model, with empty password, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let emptyPassword = String.Empty
        let confirmPassword = emptyPassword
        let (viewModel, modelState) =
            bindMemberChangePasswordViewModel id currentPassword emptyPassword confirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword emptyPassword confirmPassword
        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Binding member change password view model, with password shorter than 8 characters, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let shortPassword = randomPassword 7
        let confirmPassword = shortPassword
        let (viewModel, modelState) =
            bindMemberChangePasswordViewModel id currentPassword shortPassword confirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword shortPassword confirmPassword
        testModelState modelState "password" "Password is shorter than 8 characters"

    [<Test>]
    let ``Binding member change password view model, with password longer than 32 characters, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let longPassword = randomPassword 33
        let confirmPassword = longPassword
        let (viewModel, modelState) =
            bindMemberChangePasswordViewModel id currentPassword longPassword confirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword longPassword confirmPassword
        testModelState modelState "password" "Password is longer than 32 characters"

    [<Test>]
    let ``Binding member change password view model, with password without digits, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let noDigitsPassword = randomPasswordWithoutDigitChars 32
        let confirmPassword = noDigitsPassword
        let (viewModel, modelState) =
            bindMemberChangePasswordViewModel id currentPassword noDigitsPassword confirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword noDigitsPassword confirmPassword
        testModelState modelState "password" "Password does not contain any numbers"

    [<Test>]
    let ``Binding member change password view model, with password without lower alphas, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let noLowerAlphasPassword = randomPasswordWithoutLowercaseChars 32
        let confirmPassword = noLowerAlphasPassword
        let (viewModel, modelState) =
            bindMemberChangePasswordViewModel id currentPassword noLowerAlphasPassword confirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword noLowerAlphasPassword confirmPassword
        testModelState modelState "password" "Password does not contain any lower-case letters"

    [<Test>]
    let ``Binding member change password view model, with password without upper alphas, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let noUpperAlphasPassword = randomPasswordWithoutUppercaseChars 32
        let confirmPassword = noUpperAlphasPassword
        let (viewModel, modelState) =
            bindMemberChangePasswordViewModel id currentPassword noUpperAlphasPassword confirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword noUpperAlphasPassword confirmPassword
        testModelState modelState "password" "Password does not contain any upper-case letters"

    [<Test>]
    let ``Binding member change password view model, with password without special chars, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let noSpecialCharsPassword = randomPasswordWithoutSpecialChars 32
        let confirmPassword = noSpecialCharsPassword
        let (viewModel, modelState) =
            bindMemberChangePasswordViewModel id currentPassword noSpecialCharsPassword confirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword noSpecialCharsPassword confirmPassword
        testModelState modelState "password" "Password does not contain any special characters"

    [<Test>]
    let ``Binding member change password view model, with password without invalid chars, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let invalidCharsPassword = randomPasswordWithInvalidChars 32
        let confirmPassword = invalidCharsPassword
        let (viewModel, modelState) =
            bindMemberChangePasswordViewModel id currentPassword invalidCharsPassword confirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword invalidCharsPassword confirmPassword
        testModelState modelState "password" "Password contains invalid characters"

    [<Test>]
    let ``Binding member change password view model, with non-matching passwords, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let password = randomPassword 32
        let nonMatchingConfirmPassword = randomPassword 32
        let (viewModel, modelState) =
            bindMemberChangePasswordViewModel id currentPassword password nonMatchingConfirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword password nonMatchingConfirmPassword
        testModelState modelState "confirmPassword" "Passwords do not match"

    [<Test>]
    let ``Binding member change email view model, with valid parameters, does not modify model state`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let password = randomPassword 32
        let confirmPassword = password
        let (viewModel, modelState) = bindMemberChangePasswordViewModel id currentPassword password confirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword password confirmPassword
        modelState.IsValid =! true
