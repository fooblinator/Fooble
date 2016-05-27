namespace Fooble.UnitTest.SelfServiceRegister

open Fooble.Core
open Fooble.UnitTest
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegisterViewModelTests =

    [<Test>]
    let ``Calling make initial, returns view model`` () =
        let viewModel = SelfServiceRegister.ViewModel.empty

        test <@ box viewModel :? ISelfServiceRegisterViewModel @>

    [<Test>]
    let ``Calling make, with valid parameters, returns view model`` () =
        let viewModel =
            SelfServiceRegister.ViewModel.make (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64)

        test <@ box viewModel :? ISelfServiceRegisterViewModel @>

    [<Test>]
    let ``Calling username, with initial view model, returns expected username`` () =
        let expectedUsername = String.empty

        let viewModel = SelfServiceRegister.ViewModel.empty

        test <@ viewModel.Username = expectedUsername @>

    [<Test>]
    let ``Calling username, with not initial view model, returns expected username`` () =
        let expectedUsername = String.random 32

        let viewModel =
            SelfServiceRegister.ViewModel.make expectedUsername
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64)

        test <@ viewModel.Username = expectedUsername @>

    [<Test>]
    let ``Calling email, with initial view model, returns expected email`` () =
        let expectedEmail = String.empty

        let viewModel = SelfServiceRegister.ViewModel.empty

        test <@ viewModel.Email = expectedEmail @>

    [<Test>]
    let ``Calling email, with not initial view model, returns expected email`` () =
        let expectedEmail = sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)

        let viewModel = SelfServiceRegister.ViewModel.make (String.random 32) expectedEmail (String.random 64)

        test <@ viewModel.Email = expectedEmail @>

    [<Test>]
    let ``Calling nickname, with initial view model, returns expected nickname`` () =
        let expectedNickname = String.empty

        let viewModel = SelfServiceRegister.ViewModel.empty

        test <@ viewModel.Nickname = expectedNickname @>

    [<Test>]
    let ``Calling nickname, with not initial view model, returns expected name`` () =
        let expectedNickname = String.random 64

        let viewModel =
            SelfServiceRegister.ViewModel.make (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) expectedNickname

        test <@ viewModel.Nickname = expectedNickname @>
