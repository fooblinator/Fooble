namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberRegisterViewModelTests =

    [<Test>]
    let ``Calling username, with initial view model, returns expected username`` () =
        let expectedUsername = String.empty

        let viewModel = MemberRegisterViewModel.empty

        viewModel.Username =! expectedUsername

    [<Test>]
    let ``Calling username, with not initial view model, returns expected username`` () =
        let expectedUsername = String.random 32

        let password = Password.random 32
        let viewModel =
            bindMemberRegisterViewModel2 expectedUsername password password (EmailAddress.random 32)
                (String.random 64)

        viewModel.Username =! expectedUsername

    [<Test>]
    let ``Calling password, with initial view model, returns expected passwords`` () =
        let expectedPassword = String.empty

        let viewModel = MemberRegisterViewModel.empty

        viewModel.Password =! expectedPassword

    [<Test>]
    let ``Calling password, with not initial view model, returns expected passwords`` () =
        let expectedPassword = Password.random 32

        let viewModel =
            bindMemberRegisterViewModel2 (String.random 32) expectedPassword expectedPassword
                (EmailAddress.random 32) (String.random 64)

        viewModel.Password =! expectedPassword

    [<Test>]
    let ``Calling email, with initial view model, returns expected email`` () =
        let expectedEmail = String.empty

        let viewModel = MemberRegisterViewModel.empty

        viewModel.Email =! expectedEmail

    [<Test>]
    let ``Calling email, with not initial view model, returns expected email`` () =
        let expectedEmail = EmailAddress.random 32

        let password = Password.random 32
        let viewModel =
            bindMemberRegisterViewModel2 (String.random 32) password password expectedEmail (String.random 64)

        viewModel.Email =! expectedEmail

    [<Test>]
    let ``Calling nickname, with initial view model, returns expected nickname`` () =
        let expectedNickname = String.empty

        let viewModel = MemberRegisterViewModel.empty

        viewModel.Nickname =! expectedNickname

    [<Test>]
    let ``Calling nickname, with not initial view model, returns expected name`` () =
        let expectedNickname = String.random 64

        let password = Password.random 32
        let viewModel =
            bindMemberRegisterViewModel2 (String.random 32) password password (EmailAddress.random 32)
                expectedNickname

        viewModel.Nickname =! expectedNickname

    [<Test>]
    let ``Calling to command, returns expected command`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let viewModel =
            bindMemberRegisterViewModel2 expectedUsername expectedPassword expectedPassword expectedEmail
                expectedNickname

        let actualCommand = MemberRegisterViewModel.toCommand viewModel expectedId

        testMemberRegisterCommand actualCommand expectedId expectedUsername expectedPassword expectedEmail
            expectedNickname

    [<Test>]
    let ``Calling clean, returns expected view model`` () =
        let expectedUsername = String.random 32
        let expectedPassword = Password.random 32
        let expectedConfirmPassword = expectedPassword
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64

        let viewModel =
            bindMemberRegisterViewModel2 expectedUsername expectedPassword expectedConfirmPassword expectedEmail
                expectedNickname

        let actualViewModel = MemberRegisterViewModel.clean viewModel

        testMemberRegisterViewModel actualViewModel expectedUsername String.empty String.empty expectedEmail
            expectedNickname