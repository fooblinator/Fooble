namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module FoobleModelBinderTests =

    [<Test>]
    let ``Binding to a self-service register view model, with null username, adds expected model state error`` () =
        let nullUsername:string = null
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", nullUsername)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = nullUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is required" @>

    [<Test>]
    let ``Binding to a self-service register view model, with empty username, adds expected model state error`` () =
        let emptyUsername = String.empty
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", emptyUsername)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = emptyUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is required" @>

    [<Test>]
    let ``Binding to a self-service register view model, with username shorter than 3 characters, adds expected model state error`` () =
        let shortUsername = String.random 2
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", shortUsername)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = shortUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is shorter than 3 characters" @>

    [<Test>]
    let ``Binding to a self-service register view model, with username longer than 32 characters, adds expected model state error`` () =
        let longUsername = String.random 33
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", longUsername)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = longUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage = "Username is longer than 32 characters" @>

    [<Test>]
    let ``Binding to a self-service register view model, with username in invalid format, adds expected model state error`` () =
        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", invalidFormatUsername)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = invalidFormatUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("username") @>
        test <@ modelState.["username"].Errors.Count = 1 @>
        test <@ modelState.["username"].Errors.[0].ErrorMessage =
            "Username is not in the correct format (lowercase alphanumeric)" @>

    [<Test>]
    let ``Binding to a self-service register view model, with null email, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let nullEmail:string = null
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Email", nullEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = nullEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("email") @>
        test <@ modelState.["email"].Errors.Count = 1 @>
        test <@ modelState.["email"].Errors.[0].ErrorMessage = "Email is required" @>

    [<Test>]
    let ``Binding to a self-service register view model, with empty email, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let emptyEmail = String.empty
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Email", emptyEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = emptyEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("email") @>
        test <@ modelState.["email"].Errors.Count = 1 @>
        test <@ modelState.["email"].Errors.[0].ErrorMessage = "Email is required" @>

    [<Test>]
    let ``Binding to a self-service register view model, with email longer than 254 characters, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let longEmail = String.random 255
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Email", longEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = longEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("email") @>
        test <@ modelState.["email"].Errors.Count = 1 @>
        test <@ modelState.["email"].Errors.[0].ErrorMessage = "Email is longer than 254 characters" @>

    [<Test>]
    let ``Binding to a self-service register view model, with email in invalid format, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let invalidFormatEmail = String.random 64
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Email", invalidFormatEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = invalidFormatEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("email") @>
        test <@ modelState.["email"].Errors.Count = 1 @>
        test <@ modelState.["email"].Errors.[0].ErrorMessage = "Email is not in the correct format" @>

    [<Test>]
    let ``Binding to a self-service register view model, with null nickname, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let nullNickname:string = null

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Email", expectedEmail)
                .Add("Nickname", nullNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = nullNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("nickname") @>
        test <@ modelState.["nickname"].Errors.Count = 1 @>
        test <@ modelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is required" @>

    [<Test>]
    let ``Binding to a self-service register view model, with empty nickname, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let emptyNickname = String.empty

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Email", expectedEmail)
                .Add("Nickname", emptyNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = emptyNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("nickname") @>
        test <@ modelState.["nickname"].Errors.Count = 1 @>
        test <@ modelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is required" @>

    [<Test>]
    let ``Binding to a self-service register view model, with nickname longer than 64 characters, adds expected model state error`` () =
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let longNickname = String.random 65

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Email", expectedEmail)
                .Add("Nickname", longNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = longNickname @>

        test <@ not (modelState.IsValid) @>
        test <@ modelState.ContainsKey("nickname") @>
        test <@ modelState.["nickname"].Errors.Count = 1 @>
        test <@ modelState.["nickname"].Errors.[0].ErrorMessage = "Nickname is longer than 64 characters" @>

    [<Test>]
    let ``Binding to a self-service register view model, with valid parameters, adds no model state errors`` () =
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random ()
        let expectedNickname = String.random 64

        let (actualViewModel, modelState) =
            Map.empty
                .Add("Username", expectedUsername)
                .Add("Email", expectedEmail)
                .Add("Nickname", expectedNickname)
            |> bindModel<ISelfServiceRegisterViewModel>

        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

        test <@ modelState.IsValid @>
