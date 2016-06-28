namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open Moq
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberChangeUsernameViewModelBinderTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore (MemberChangeUsernameViewModelBinder(Mock.Of<MemberChangeUsernameViewModelFactory>()))

    [<Test>]
    let ``Constructing, with null change username view model factory, raises expected exception`` () =
        testArgumentException "viewModelFactory" "Member change username view model factory is required"
            <@ MemberChangeUsernameViewModelBinder(null) @>

    [<Test>]
    let ``Binding member change username view model, with null username, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let nullUsername:string = null
        let (viewModel, modelState) = bindMemberChangeUsernameViewModel id currentPassword nullUsername
        testMemberChangeUsernameViewModel viewModel id currentPassword nullUsername
        testModelState modelState "username" "Username is required"

    [<Test>]
    let ``Binding member change username view model, with empty username, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let emptyUsername = String.Empty
        let (viewModel, modelState) = bindMemberChangeUsernameViewModel id currentPassword emptyUsername
        testMemberChangeUsernameViewModel viewModel id currentPassword emptyUsername
        testModelState modelState "username" "Username is required"

    [<Test>]
    let ``Binding member change username view model, with username shorter than 3 characters, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let shortUsername = randomString 2
        let (viewModel, modelState) = bindMemberChangeUsernameViewModel id currentPassword shortUsername
        testMemberChangeUsernameViewModel viewModel id currentPassword shortUsername
        testModelState modelState "username" "Username is shorter than 3 characters"

    [<Test>]
    let ``Binding member change username view model, with username longer than 32 characters, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let longUsername = randomString 33
        let (viewModel, modelState) = bindMemberChangeUsernameViewModel id currentPassword longUsername
        testMemberChangeUsernameViewModel viewModel id currentPassword longUsername
        testModelState modelState "username" "Username is longer than 32 characters"

    [<Test>]
    let ``Binding member change username view model, with username in invalid format, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let invalidFormatUsername = sprintf "-%s-" (randomString 30)
        let (viewModel, modelState) = bindMemberChangeUsernameViewModel id currentPassword invalidFormatUsername
        testMemberChangeUsernameViewModel viewModel id currentPassword invalidFormatUsername
        testModelState modelState "username" "Username is not in the correct format (lowercase alphanumeric)"

    [<Test>]
    let ``Binding member change username view model, with valid parameters, does not modify model state`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let username = randomString 32
        let (viewModel, modelState) = bindMemberChangeUsernameViewModel id currentPassword username
        testMemberChangeUsernameViewModel viewModel id currentPassword username
        modelState.IsValid =! true
