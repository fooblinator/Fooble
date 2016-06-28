namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open System

[<TestFixture>]
module MemberChangeUsernameViewModelTests =

    [<Test>]
    let ``Calling make, with successful parameters, returns expected view model`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let username = randomString 32
        let viewModel = MemberChangeUsernameViewModel.make id currentPassword username
        testMemberChangeUsernameViewModel viewModel id currentPassword username

    [<Test>]
    let ``Calling extension clean, returns expected view model`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let username = randomString 32
        let viewModel =
            MemberChangeUsernameViewModel.make id currentPassword username
            |> fun x -> x.Clean()
        testMemberChangeUsernameViewModel viewModel id String.Empty username

    [<Test>]
    let ``Calling extension to command, returns expected command`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let username = randomString 32
        let command =
            MemberChangeUsernameViewModel.make id currentPassword username
            |> fun x -> x.MapCommand()
        testMemberChangeUsernameCommand command id currentPassword username
