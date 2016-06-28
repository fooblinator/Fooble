namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberRegisterViewModelTests =

    [<Test>]
    let ``Calling make initial, with successful parameters, returns expected view model`` () =
        let viewModel = MemberRegisterViewModel.makeInitial ()
        viewModel.AvatarData <>! String.Empty
        testMemberRegisterViewModel viewModel String.Empty String.Empty String.Empty String.Empty String.Empty None

    [<Test>]
    let ``Calling make, with successful parameters, returns expected view model`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let viewModel = MemberRegisterViewModel.make username password confirmPassword email nickname avatarData
        testMemberRegisterViewModel viewModel username password confirmPassword email nickname (Some(avatarData))

    [<Test>]
    let ``Calling extension clean, returns expected view model`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let viewModel =
            MemberRegisterViewModel.make username password confirmPassword email nickname avatarData
            |> fun x -> x.Clean()
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname (Some(avatarData))

    [<Test>]
    let ``Calling extension randomize avatar data, returns expected view model`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let viewModel =
            MemberRegisterViewModel.make username password confirmPassword email nickname avatarData
            |> fun x -> x.RandomizeAvatarData()
        viewModel.AvatarData <>! avatarData
        testMemberRegisterViewModel viewModel username String.Empty String.Empty email nickname None

    [<Test>]
    let ``Calling extension to command, returns expected command`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let command =
            MemberRegisterViewModel.make username password confirmPassword email nickname avatarData
            |> fun x -> x.MapCommand(id)
        testMemberRegisterCommand command id username password email nickname avatarData
