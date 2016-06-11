namespace Fooble.UnitTest

open Fooble.Common
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module BindMemberChangeUsernameViewModelTests =

    [<Test>]
    let ``Binding to a member change username view model, with null new username, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let nullNewUsername:string = null

        let (actualViewModel, modelState) =
            bindMemberChangeUsernameViewModel expectedId expectedCurrentPassword nullNewUsername

        testMemberChangeUsernameViewModel actualViewModel expectedId expectedCurrentPassword nullNewUsername

        testModelState modelState "newUsername" "New username is required"

    [<Test>]
    let ``Binding to a member change username view model, with empty new username, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let emptyNewUsername = String.empty

        let (actualViewModel, modelState) =
            bindMemberChangeUsernameViewModel expectedId expectedCurrentPassword emptyNewUsername

        testMemberChangeUsernameViewModel actualViewModel expectedId expectedCurrentPassword emptyNewUsername

        testModelState modelState "newUsername" "New username is required"

    [<Test>]
    let ``Binding to a member change username view model, with new username shorter than 3 characters, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let shortNewUsername = String.random 2

        let (actualViewModel, modelState) =
            bindMemberChangeUsernameViewModel expectedId expectedCurrentPassword shortNewUsername

        testMemberChangeUsernameViewModel actualViewModel expectedId expectedCurrentPassword shortNewUsername

        testModelState modelState "newUsername" "New username is shorter than 3 characters"

    [<Test>]
    let ``Binding to a member change username view model, with new username longer than 32 characters, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let longNewUsername = String.random 33

        let (actualViewModel, modelState) =
            bindMemberChangeUsernameViewModel expectedId expectedCurrentPassword longNewUsername

        testMemberChangeUsernameViewModel actualViewModel expectedId expectedCurrentPassword longNewUsername

        testModelState modelState "newUsername" "New username is longer than 32 characters"

    [<Test>]
    let ``Binding to a member change username view model, with new username in invalid format, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let invalidFormatNewUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)

        let (actualViewModel, modelState) =
            bindMemberChangeUsernameViewModel expectedId expectedCurrentPassword invalidFormatNewUsername

        testMemberChangeUsernameViewModel actualViewModel expectedId expectedCurrentPassword invalidFormatNewUsername

        testModelState modelState "newUsername" "New username is not in the correct format (lowercase alphanumeric)"

    [<Test>]
    let ``Binding to a member change username view model, with valid parameters, adds no model state errors`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewUsername = String.random 32

        let (actualViewModel, modelState) =
            bindMemberChangeUsernameViewModel expectedId expectedCurrentPassword expectedNewUsername

        testMemberChangeUsernameViewModel actualViewModel expectedId expectedCurrentPassword expectedNewUsername

        modelState.IsValid =! true
