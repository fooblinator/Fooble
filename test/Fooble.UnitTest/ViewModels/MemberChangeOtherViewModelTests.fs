namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberChangeOtherViewModelTests =

    [<Test>]
    let ``Calling make, with successful parameters, returns expected view model`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 64
        let avatarData = randomString 32
        let viewModel = MemberChangeOtherViewModel.make id nickname avatarData
        testMemberChangeOtherViewModel viewModel id nickname avatarData

    [<Test>]
    let ``Calling extension randomize avatar data, returns expected view model`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 64
        let avatarData = randomString 32
        let viewModel =
            MemberChangeOtherViewModel.make id nickname avatarData
            |> fun x -> x.RandomizeAvatarData()
        viewModel.AvatarData <>! avatarData
        testMemberChangeOtherViewModel viewModel id nickname viewModel.AvatarData

    [<Test>]
    let ``Calling extension to command, returns expected command`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 64
        let avatarData = randomString 32
        let command =
            MemberChangeOtherViewModel.make id nickname avatarData
            |> fun x -> x.MapCommand()
        testMemberChangeOtherCommand command id nickname avatarData
