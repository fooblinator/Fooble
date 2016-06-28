namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open System

[<TestFixture>]
module MemberDeactivateViewModelTests =

    [<Test>]
    let ``Calling make initial, with successful parameters, returns expected view model`` () =
        let id = Guid.NewGuid()
        let viewModel = MemberDeactivateViewModel.makeInitial id
        testMemberDeactivateViewModel viewModel id String.Empty

    [<Test>]
    let ``Calling make, with successful parameters, returns expected view model`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let viewModel = MemberDeactivateViewModel.make id currentPassword
        testMemberDeactivateViewModel viewModel id currentPassword

    [<Test>]
    let ``Calling extension clean, returns expected view model`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let viewModel =
            MemberDeactivateViewModel.make id currentPassword
            |> fun x -> x.Clean()
        testMemberDeactivateViewModel viewModel id String.Empty

    [<Test>]
    let ``Calling extension to command, returns expected command`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let command =
            MemberDeactivateViewModel.make id currentPassword
            |> fun x -> x.MapCommand()
        testMemberDeactivateCommand command id currentPassword
