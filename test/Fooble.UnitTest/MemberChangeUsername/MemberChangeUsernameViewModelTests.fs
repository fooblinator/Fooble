namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberChangeUsernameViewModelTests =

    [<Test>]
    let ``Calling current password, with initial view model, returns expected current password`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = String.empty

        let viewModel = MemberChangeUsernameViewModel.makeInitial expectedId

        viewModel.CurrentPassword =! expectedCurrentPassword

    [<Test>]
    let ``Calling current password, with not initial view model, returns expected current password`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32

        let viewModel = bindMemberChangeUsernameViewModel2 expectedId expectedCurrentPassword (String.random 32)

        viewModel.CurrentPassword =! expectedCurrentPassword

    [<Test>]
    let ``Calling new username, with initial view model, returns expected new username`` () =
        let expectedId = Guid.random ()
        let expectedNewUsername = String.empty

        let viewModel = MemberChangeUsernameViewModel.makeInitial expectedId

        viewModel.NewUsername =! expectedNewUsername

    [<Test>]
    let ``Calling new username, with not initial view model, returns expected new username`` () =
        let expectedId = Guid.random ()
        let expectedNewUsername = String.random 32

        let viewModel = bindMemberChangeUsernameViewModel2 expectedId (Password.random 32) expectedNewUsername

        viewModel.NewUsername =! expectedNewUsername

    [<Test>]
    let ``Calling clean, returns expected view model`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewUsername = String.random 32

        let viewModel = bindMemberChangeUsernameViewModel2 expectedId expectedCurrentPassword expectedNewUsername

        let actualViewModel = MemberChangeUsernameViewModel.clean viewModel

        testMemberChangeUsernameViewModel actualViewModel expectedId String.empty expectedNewUsername

    [<Test>]
    let ``Calling to command, returns expected command`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewUsername = String.random 32

        let viewModel = bindMemberChangeUsernameViewModel2 expectedId expectedCurrentPassword expectedNewUsername

        let actualCommand = MemberChangeUsernameViewModel.toCommand viewModel

        testMemberChangeUsernameCommand actualCommand expectedId expectedCurrentPassword expectedNewUsername
