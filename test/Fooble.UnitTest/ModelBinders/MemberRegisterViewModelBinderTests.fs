namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open Moq
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberRegisterViewModelBinderTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore (MemberRegisterViewModelBinder(Mock.Of<MemberRegisterViewModelFactory>()))

    [<Test>]
    let ``Constructing, with null register view model factory, raises expected exception`` () =
        testArgumentException "viewModelFactory" "Member register view model factory is required"
            <@ MemberRegisterViewModelBinder(null) @>

    [<Test>]
    let ``Binding member register view model, with null username, modifies model state appropriately`` () =
        let nullUsername:string = null
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel nullUsername password confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel nullUsername password confirmPassword email nickname (Some(avatarData))
        testModelState modelState "username" "Username is required"

    [<Test>]
    let ``Binding member register view model, with empty username, modifies model state appropriately`` () =
        let emptyUsername = String.Empty
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel emptyUsername password confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel emptyUsername password confirmPassword email nickname (Some(avatarData))
        testModelState modelState "username" "Username is required"

    [<Test>]
    let ``Binding member register view model, with username shorter than 3 characters, modifies model state appropriately`` () =
        let shortUsername = randomString 2
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel shortUsername password confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel shortUsername password confirmPassword email nickname (Some(avatarData))
        testModelState modelState "username" "Username is shorter than 3 characters"

    [<Test>]
    let ``Binding member register view model, with username longer than 32 characters, modifies model state appropriately`` () =
        let longUsername = randomString 33
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel longUsername password confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel longUsername password confirmPassword email nickname (Some(avatarData))
        testModelState modelState "username" "Username is longer than 32 characters"

    [<Test>]
    let ``Binding member register view model, with username in invalid format, modifies model state appropriately`` () =
        let invalidFormatUsername = sprintf "-%s-" (randomString 30)
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel invalidFormatUsername password confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel invalidFormatUsername password confirmPassword email nickname
            (Some(avatarData))
        testModelState modelState "username" "Username is not in the correct format (lowercase alphanumeric)"

    [<Test>]
    let ``Binding member register view model, with null password, modifies model state appropriately`` () =
        let username = randomString 32
        let nullPassword:string = null
        let confirmPassword = nullPassword
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username nullPassword confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username nullPassword confirmPassword email nickname (Some(avatarData))
        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Binding member register view model, with empty password, modifies model state appropriately`` () =
        let username = randomString 32
        let emptyPassword = String.Empty
        let confirmPassword = emptyPassword
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username emptyPassword confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username emptyPassword confirmPassword email nickname (Some(avatarData))
        testModelState modelState "password" "Password is required"

    [<Test>]
    let ``Binding member register view model, with password shorter than 8 characters, modifies model state appropriately`` () =
        let username = randomString 32
        let shortPassword = randomPassword 7
        let confirmPassword = shortPassword
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username shortPassword confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username shortPassword confirmPassword email nickname (Some(avatarData))
        testModelState modelState "password" "Password is shorter than 8 characters"

    [<Test>]
    let ``Binding member register view model, with password longer than 32 characters, modifies model state appropriately`` () =
        let username = randomString 32
        let longPassword = randomPassword 33
        let confirmPassword = longPassword
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username longPassword confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username longPassword confirmPassword email nickname (Some(avatarData))
        testModelState modelState "password" "Password is longer than 32 characters"

    [<Test>]
    let ``Binding member register view model, with password without digits, modifies model state appropriately`` () =
        let username = randomString 32
        let noDigitsPassword = randomPasswordWithoutDigitChars 32
        let confirmPassword = noDigitsPassword
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username noDigitsPassword confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username noDigitsPassword confirmPassword email nickname
            (Some(avatarData))
        testModelState modelState "password" "Password does not contain any numbers"

    [<Test>]
    let ``Binding member register view model, with password without lower alphas, modifies model state appropriately`` () =
        let username = randomString 32
        let noLowerAlphasPassword = randomPasswordWithoutLowercaseChars 32
        let confirmPassword = noLowerAlphasPassword
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username noLowerAlphasPassword confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username noLowerAlphasPassword confirmPassword email nickname
            (Some(avatarData))
        testModelState modelState "password" "Password does not contain any lower-case letters"

    [<Test>]
    let ``Binding member register view model, with password without upper alphas, modifies model state appropriately`` () =
        let username = randomString 32
        let noUpperAlphasPassword = randomPasswordWithoutUppercaseChars 32
        let confirmPassword = noUpperAlphasPassword
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username noUpperAlphasPassword confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username noUpperAlphasPassword confirmPassword email nickname
            (Some(avatarData))
        testModelState modelState "password" "Password does not contain any upper-case letters"

    [<Test>]
    let ``Binding member register view model, with password without special chars, modifies model state appropriately`` () =
        let username = randomString 32
        let noSpecialCharsPassword = randomPasswordWithoutSpecialChars 32
        let confirmPassword = noSpecialCharsPassword
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username noSpecialCharsPassword confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username noSpecialCharsPassword confirmPassword email nickname
            (Some(avatarData))
        testModelState modelState "password" "Password does not contain any special characters"

    [<Test>]
    let ``Binding member register view model, with password without invalid chars, modifies model state appropriately`` () =
        let username = randomString 32
        let invalidCharsPassword = randomPasswordWithInvalidChars 32
        let confirmPassword = invalidCharsPassword
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username invalidCharsPassword confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username invalidCharsPassword confirmPassword email nickname
            (Some(avatarData))
        testModelState modelState "password" "Password contains invalid characters"

    [<Test>]
    let ``Binding member register view model, with non-matching passwords, modifies model state appropriately`` () =
        let username = randomString 32
        let password = randomPassword 32
        let nonMatchingConfirmPassword = randomPassword 32
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password nonMatchingConfirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username password nonMatchingConfirmPassword email nickname
            (Some(avatarData))
        testModelState modelState "confirmPassword" "Passwords do not match"

    [<Test>]
    let ``Binding member register view model, with null email, modifies model state appropriately`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let nullEmail:string = null
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword nullEmail nickname avatarData
        testMemberRegisterViewModel viewModel username password confirmPassword nullEmail nickname (Some(avatarData))
        testModelState modelState "email" "Email is required"

    [<Test>]
    let ``Binding member register view model, with empty email, modifies model state appropriately`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let emptyEmail = String.Empty
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword emptyEmail nickname avatarData
        testMemberRegisterViewModel viewModel username password confirmPassword emptyEmail nickname (Some(avatarData))
        testModelState modelState "email" "Email is required"

    [<Test>]
    let ``Binding member register view model, with email longer than 256 characters, modifies model state appropriately`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let longEmail = randomEmail 257
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword longEmail nickname avatarData
        testMemberRegisterViewModel viewModel username password confirmPassword longEmail nickname (Some(avatarData))
        testModelState modelState "email" "Email is longer than 256 characters"

    [<Test>]
    let ``Binding member register view model, with email in invalid format, modifies model state appropriately`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let invalidFormatEmail = randomString 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword invalidFormatEmail nickname avatarData
        testMemberRegisterViewModel viewModel username password confirmPassword invalidFormatEmail nickname
            (Some(avatarData))
        testModelState modelState "email" "Email is not in the correct format"

    [<Test>]
    let ``Binding member register view model, with null nickname, modifies model state appropriately`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nullNickname:string = null
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword email nullNickname avatarData
        testMemberRegisterViewModel viewModel username password confirmPassword email nullNickname (Some(avatarData))
        testModelState modelState "nickname" "Nickname is required"

    [<Test>]
    let ``Binding member register view model, with empty nickname, modifies model state appropriately`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let emptyNickname = String.Empty
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword email emptyNickname avatarData
        testMemberRegisterViewModel viewModel username password confirmPassword email emptyNickname (Some(avatarData))
        testModelState modelState "nickname" "Nickname is required"

    [<Test>]
    let ``Binding member register view model, with nickname longer than 64 characters, modifies model state appropriately`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let longNickname = randomString 65
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword email longNickname avatarData
        testMemberRegisterViewModel viewModel username password confirmPassword email longNickname (Some(avatarData))
        testModelState modelState "nickname" "Nickname is longer than 64 characters"

    [<Test>]
    let ``Binding member register view model, with null avatar data, modifies model state appropriately`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let nullAvatarData:string = null
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword email nickname nullAvatarData
        testMemberRegisterViewModel viewModel username password confirmPassword email nickname (Some(nullAvatarData))
        testModelState modelState "avatarData" "Avatar data is required"

    [<Test>]
    let ``Binding member register view model, with empty avatar data, modifies model state appropriately`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let emptyAvatarData = String.Empty
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword email nickname emptyAvatarData
        testMemberRegisterViewModel viewModel username password confirmPassword email nickname (Some(emptyAvatarData))
        testModelState modelState "avatarData" "Avatar data is required"

    [<Test>]
    let ``Binding member register view model, with avatar data longer than 32 characters, modifies model state appropriately`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let longAvatarData = randomString 33
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword email nickname longAvatarData
        testMemberRegisterViewModel viewModel username password confirmPassword email nickname (Some(longAvatarData))
        testModelState modelState "avatarData" "Avatar data is longer than 32 characters"

    [<Test>]
    let ``Binding member register view model, with valid parameters, does not modify model state`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username password confirmPassword email nickname (Some(avatarData))
        modelState.IsValid =! true
