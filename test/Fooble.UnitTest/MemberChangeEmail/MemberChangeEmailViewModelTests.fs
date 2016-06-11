namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberChangeEmailViewModelTests =

    [<Test>]
    let ``Calling current password, with initial view model, returns expected current password`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = String.empty

        let viewModel = MemberChangeEmailViewModel.makeInitial expectedId

        viewModel.CurrentPassword =! expectedCurrentPassword

    [<Test>]
    let ``Calling current password, with not initial view model, returns expected current password`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32

        let viewModel = bindMemberChangeEmailViewModel2 expectedId expectedCurrentPassword (EmailAddress.random 32)

        viewModel.CurrentPassword =! expectedCurrentPassword

    [<Test>]
    let ``Calling new email, with initial view model, returns expected new email`` () =
        let expectedId = Guid.random ()
        let expectedNewEmail = String.empty

        let viewModel = MemberChangeEmailViewModel.makeInitial expectedId

        viewModel.NewEmail =! expectedNewEmail

    [<Test>]
    let ``Calling new email, with not initial view model, returns expected new email`` () =
        let expectedId = Guid.random ()
        let expectedNewEmail = EmailAddress.random 32

        let viewModel = bindMemberChangeEmailViewModel2 expectedId (Password.random 32) expectedNewEmail

        viewModel.NewEmail =! expectedNewEmail

    [<Test>]
    let ``Calling clean, returns expected view model`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewEmail = EmailAddress.random 32

        let viewModel = bindMemberChangeEmailViewModel2 expectedId expectedCurrentPassword expectedNewEmail

        let actualViewModel = MemberChangeEmailViewModel.clean viewModel

        testMemberChangeEmailViewModel actualViewModel expectedId String.empty expectedNewEmail

    [<Test>]
    let ``Calling to command, returns expected command`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewEmail = EmailAddress.random 32

        let viewModel = bindMemberChangeEmailViewModel2 expectedId expectedCurrentPassword expectedNewEmail

        let actualCommand = MemberChangeEmailViewModel.toCommand viewModel

        testMemberChangeEmailCommand actualCommand expectedId expectedCurrentPassword expectedNewEmail
