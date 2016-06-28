namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open Moq
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDeactivateViewModelBinderTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore (MemberDeactivateViewModelBinder(Mock.Of<MemberDeactivateViewModelFactory>()))

    [<Test>]
    let ``Constructing, with null deactivate view model factory, raises expected exception`` () =
        testArgumentException "viewModelFactory" "Member deactivate view model factory is required"
            <@ MemberDeactivateViewModelBinder(null) @>

    [<Test>]
    let ``Binding member deactivate view model, with valid parameters, does not modify model state`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let (viewModel, modelState) = bindMemberDeactivateViewModel id currentPassword
        testMemberDeactivateViewModel viewModel id currentPassword
        modelState.IsValid =! true
