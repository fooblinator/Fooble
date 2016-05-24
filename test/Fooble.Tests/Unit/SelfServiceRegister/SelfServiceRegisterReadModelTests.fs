namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
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
        let viewModel = SelfServiceRegister.ViewModel.make (String.random 32) (String.random 64)

        test <@ box viewModel :? ISelfServiceRegisterViewModel @>

    [<Test>]
    let ``Calling username, with initial view model, returns expected username`` () =
        let expectedUsername = String.empty

        let viewModel = SelfServiceRegister.ViewModel.empty

        let actualUsername = viewModel.Username
        test <@ actualUsername = expectedUsername @>

    [<Test>]
    let ``Calling username, with not initial view model, returns expected username`` () =
        let expectedUsername = String.random 32

        let viewModel = SelfServiceRegister.ViewModel.make expectedUsername (String.random 64)

        let actualUsername = viewModel.Username
        test <@ actualUsername = expectedUsername @>

    [<Test>]
    let ``Calling name, with initial view model, returns expected name`` () =
        let expectedName = String.empty

        let viewModel = SelfServiceRegister.ViewModel.empty

        let actualName = viewModel.Name
        test <@ actualName = expectedName @>

    [<Test>]
    let ``Calling name, with not initial view model, returns expected name`` () =
        let expectedName = String.random 64

        let viewModel = SelfServiceRegister.ViewModel.make (String.random 32) expectedName

        let actualName = viewModel.Name
        test <@ actualName = expectedName @>
