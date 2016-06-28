namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open Moq
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberChangeOtherViewModelBinderTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore (MemberChangeOtherViewModelBinder(Mock.Of<MemberChangeOtherViewModelFactory>()))

    [<Test>]
    let ``Constructing, with null change other view model factory, raises expected exception`` () =
        testArgumentException "viewModelFactory" "Member change other view model factory is required"
            <@ MemberChangeOtherViewModelBinder(null) @>

    [<Test>]
    let ``Binding member change other view model, with null nickname, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let nullNickname:string = null
        let avatarData = randomString 32
        let (viewModel, modelState) = bindMemberChangeOtherViewModel id nullNickname avatarData
        testMemberChangeOtherViewModel viewModel id nullNickname avatarData
        testModelState modelState "nickname" "Nickname is required"

    [<Test>]
    let ``Binding member change other view model, with empty nickname, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let emptyNickname = String.Empty
        let avatarData = randomString 32
        let (viewModel, modelState) = bindMemberChangeOtherViewModel id emptyNickname avatarData
        testMemberChangeOtherViewModel viewModel id emptyNickname avatarData
        testModelState modelState "nickname" "Nickname is required"

    [<Test>]
    let ``Binding member change other view model, with nickname longer than 64 characters, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let longNickname = randomString 65
        let avatarData = randomString 32
        let (viewModel, modelState) = bindMemberChangeOtherViewModel id longNickname avatarData
        testMemberChangeOtherViewModel viewModel id longNickname avatarData
        testModelState modelState "nickname" "Nickname is longer than 64 characters"

    [<Test>]
    let ``Binding member change other view model, with null avatar data, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 64
        let nullAvatarData:string = null
        let (viewModel, modelState) = bindMemberChangeOtherViewModel id nickname nullAvatarData
        testMemberChangeOtherViewModel viewModel id nickname nullAvatarData
        testModelState modelState "avatarData" "Avatar data is required"

    [<Test>]
    let ``Binding member change other view model, with empty avatar data, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 64
        let emptyAvatarData = String.Empty
        let (viewModel, modelState) = bindMemberChangeOtherViewModel id nickname emptyAvatarData
        testMemberChangeOtherViewModel viewModel id nickname emptyAvatarData
        testModelState modelState "avatarData" "Avatar data is required"

    [<Test>]
    let ``Binding member change other view model, with avatar data longer than 32 characters, modifies model state appropriately`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 64
        let longAvatarData = randomString 33
        let (viewModel, modelState) = bindMemberChangeOtherViewModel id nickname longAvatarData
        testMemberChangeOtherViewModel viewModel id nickname longAvatarData
        testModelState modelState "avatarData" "Avatar data is longer than 32 characters"

    [<Test>]
    let ``Binding member change other view model, with valid parameters, does not modify model state`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 64
        let avatarData = randomString 32
        let (viewModel, modelState) = bindMemberChangeOtherViewModel id nickname avatarData
        testMemberChangeOtherViewModel viewModel id nickname avatarData
        modelState.IsValid =! true
