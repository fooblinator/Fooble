namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberChangeOtherViewModelTests =

    [<Test>]
    let ``Calling new nickname, with initial view model, returns expected new nickname`` () =
        let expectedId = Guid.random ()
        let expectedNewNickname = String.empty

        let viewModel = MemberChangeOtherViewModel.makeInitial expectedId

        viewModel.NewNickname =! expectedNewNickname

    [<Test>]
    let ``Calling new nickname, with not initial view model, returns expected new nickname`` () =
        let expectedId = Guid.random ()
        let expectedNewNickname = String.random 32

        let viewModel = bindMemberChangeOtherViewModel2 expectedId expectedNewNickname

        viewModel.NewNickname =! expectedNewNickname
