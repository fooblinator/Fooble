namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open System

[<TestFixture>]
module MemberChangeEmailViewModelTests =

    [<Test>]
    let ``Calling make, with successful parameters, returns expected view model`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let email = randomEmail 32
        let viewModel = MemberChangeEmailViewModel.make id currentPassword email
        testMemberChangeEmailViewModel viewModel id currentPassword email

    [<Test>]
    let ``Calling extension clean, returns expected view model`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let email = randomEmail 32
        let viewModel =
            MemberChangeEmailViewModel.make id currentPassword email
            |> fun x -> x.Clean()
        testMemberChangeEmailViewModel viewModel id String.Empty email

    [<Test>]
    let ``Calling extension to command, returns expected command`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let email = randomEmail 32
        let command =
            MemberChangeEmailViewModel.make id currentPassword email
            |> fun x -> x.MapCommand()
        testMemberChangeEmailCommand command id currentPassword email
