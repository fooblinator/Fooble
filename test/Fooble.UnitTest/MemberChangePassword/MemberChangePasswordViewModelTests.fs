namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberChangePasswordViewModelTests =

    [<Test>]
    let ``Calling current password, with initial view model, returns expected current password`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = String.empty

        let viewModel = MemberChangePasswordViewModel.makeInitial expectedId

        viewModel.CurrentPassword =! expectedCurrentPassword

    [<Test>]
    let ``Calling current password, with not initial view model, returns expected current password`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32

        let newPassword = Password.random 32
        let confirmPassword = newPassword
        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId expectedCurrentPassword newPassword confirmPassword

        viewModel.CurrentPassword =! expectedCurrentPassword

    [<Test>]
    let ``Calling new password, with initial view model, returns expected new password`` () =
        let expectedId = Guid.random ()
        let expectedNewPassword = String.empty

        let viewModel = MemberChangePasswordViewModel.makeInitial expectedId

        viewModel.NewPassword =! expectedNewPassword

    [<Test>]
    let ``Calling new password, with not initial view model, returns expected new password`` () =
        let expectedId = Guid.random ()
        let expectedNewPassword = Password.random 32

        let confirmPassword = expectedNewPassword
        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId (Password.random 32) expectedNewPassword confirmPassword

        viewModel.NewPassword =! expectedNewPassword

    [<Test>]
    let ``Calling confirm password, with initial view model, returns expected confirm password`` () =
        let expectedId = Guid.random ()
        let expectedConfirmPassword = String.empty

        let viewModel = MemberChangePasswordViewModel.makeInitial expectedId

        viewModel.ConfirmPassword =! expectedConfirmPassword

    [<Test>]
    let ``Calling confirm password, with not initial view model, returns expected confirm password`` () =
        let expectedId = Guid.random ()
        let expectedConfirmPassword = Password.random 32

        let newPassword = expectedConfirmPassword
        let viewModel =
            bindMemberChangePasswordViewModel2 expectedId (Password.random 32) newPassword expectedConfirmPassword

        viewModel.ConfirmPassword =! expectedConfirmPassword
