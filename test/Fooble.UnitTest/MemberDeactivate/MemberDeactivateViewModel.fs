namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDeactivateViewModelTests =

    [<Test>]
    let ``Calling current password, with initial view model, returns expected current password`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = String.empty

        let viewModel = MemberDeactivateViewModel.makeInitial expectedId

        viewModel.CurrentPassword =! expectedCurrentPassword

    [<Test>]
    let ``Calling current password, with not initial view model, returns expected current password`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32

        let viewModel = bindMemberDeactivateViewModel2 expectedId expectedCurrentPassword

        viewModel.CurrentPassword =! expectedCurrentPassword

    [<Test>]
    let ``Calling clean, returns expected view model`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32

        let viewModel = bindMemberDeactivateViewModel2 expectedId expectedCurrentPassword

        let actualViewModel = MemberDeactivateViewModel.clean viewModel

        testMemberDeactivateViewModel actualViewModel expectedId String.empty

    [<Test>]
    let ``Calling to command, returns expected command`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32

        let viewModel = bindMemberDeactivateViewModel2 expectedId expectedCurrentPassword

        let actualCommand = MemberDeactivateViewModel.toCommand viewModel

        testMemberDeactivateCommand actualCommand expectedId expectedCurrentPassword
