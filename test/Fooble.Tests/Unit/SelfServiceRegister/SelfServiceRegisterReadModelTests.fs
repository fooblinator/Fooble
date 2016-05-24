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
        let viewModel = SelfServiceRegister.ViewModel.make <| randomString ()

        test <@ box viewModel :? ISelfServiceRegisterViewModel @>

    [<Test>]
    let ``Calling name, with initial view model, returns expected name`` () =
        let expectedName = String.empty

        let viewModel = SelfServiceRegister.ViewModel.empty

        let actualName = viewModel.Name
        test <@ actualName = expectedName @>

    [<Test>]
    let ``Calling name, with not initial view model, returns expected name`` () =
        let expectedName = randomString ()

        let viewModel = SelfServiceRegister.ViewModel.make expectedName

        let actualName = viewModel.Name
        test <@ actualName = expectedName @>
