namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open Moq
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberChangeEmailViewModelBinderTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore (MemberChangeEmailViewModelBinder(Mock.Of<MemberChangeEmailViewModelFactory>()))

    [<Test>]
    let ``Constructing, with null change email view model factory, raises expected exception`` () =
        testArgumentException "viewModelFactory" "Member change email view model factory is required"
            <@ MemberChangeEmailViewModelBinder(null) @>

    [<Test>]
    let ``Binding member change email view model, with null email, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let nullEmail:string = null
        let (viewModel, modelState) = bindMemberChangeEmailViewModel id currentPassword nullEmail
        testMemberChangeEmailViewModel viewModel id currentPassword nullEmail
        testModelState modelState "email" "Email is required"

    [<Test>]
    let ``Binding member change email view model, with empty email, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let emptyEmail = String.Empty
        let (viewModel, modelState) = bindMemberChangeEmailViewModel id currentPassword emptyEmail
        testMemberChangeEmailViewModel viewModel id currentPassword emptyEmail
        testModelState modelState "email" "Email is required"

    [<Test>]
    let ``Binding member change email view model, with email longer than 256 characters, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let longEmail = randomEmail 257
        let (viewModel, modelState) = bindMemberChangeEmailViewModel id currentPassword longEmail
        testMemberChangeEmailViewModel viewModel id currentPassword longEmail
        testModelState modelState "email" "Email is longer than 256 characters"

    [<Test>]
    let ``Binding member change email view model, with email in invalid format, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let invalidFormatEmail = randomString 32
        let (viewModel, modelState) = bindMemberChangeEmailViewModel id currentPassword invalidFormatEmail
        testMemberChangeEmailViewModel viewModel id currentPassword invalidFormatEmail
        testModelState modelState "email" "Email is not in the correct format"

    [<Test>]
    let ``Binding member change email view model, with valid parameters, does not modify model state`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let email = randomEmail 32
        let (viewModel, modelState) = bindMemberChangeEmailViewModel id currentPassword email
        testMemberChangeEmailViewModel viewModel id currentPassword email
        modelState.IsValid =! true
