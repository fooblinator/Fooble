namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open System

[<TestFixture>]
module MemberChangePasswordViewModelTests =

    [<Test>]
    let ``Calling make initial, with successful parameters, returns expected view model`` () =
        let id = Guid.NewGuid()
        let viewModel = MemberChangePasswordViewModel.makeInitial id
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty

    [<Test>]
    let ``Calling make, with successful parameters, returns expected view model`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let password = randomPassword 32
        let confirmPassword = password
        let viewModel = MemberChangePasswordViewModel.make id currentPassword password confirmPassword
        testMemberChangePasswordViewModel viewModel id currentPassword password confirmPassword

    [<Test>]
    let ``Calling extension clean, returns expected view model`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let password = randomPassword 32
        let confirmPassword = password
        let viewModel =
            MemberChangePasswordViewModel.make id currentPassword password confirmPassword
            |> fun x -> x.Clean()
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty

    [<Test>]
    let ``Calling extension to command, returns expected command`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let password = randomPassword 32
        let confirmPassword = password
        let command =
            MemberChangePasswordViewModel.make id currentPassword password confirmPassword
            |> fun x -> x.MapCommand()
        testMemberChangePasswordCommand command id currentPassword password
