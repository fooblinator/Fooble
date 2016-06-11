namespace Fooble.UnitTest

open Fooble.Common
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module BindMemberChangeEmailViewModelTests =

    [<Test>]
    let ``Binding to a member change email view model, with null new email, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let nullNewEmail:string = null

        let (actualViewModel, modelState) =
            bindMemberChangeEmailViewModel expectedId expectedCurrentPassword nullNewEmail

        testMemberChangeEmailViewModel actualViewModel expectedId expectedCurrentPassword nullNewEmail

        testModelState modelState "newEmail" "New email is required"

    [<Test>]
    let ``Binding to a member change email view model, with empty new email, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let emptyNewEmail = String.empty

        let (actualViewModel, modelState) =
            bindMemberChangeEmailViewModel expectedId expectedCurrentPassword emptyNewEmail

        testMemberChangeEmailViewModel actualViewModel expectedId expectedCurrentPassword emptyNewEmail

        testModelState modelState "newEmail" "New email is required"

    [<Test>]
    let ``Binding to a member change email view model, with new email longer than 254 characters, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let longNewEmail = EmailAddress.random 255

        let (actualViewModel, modelState) =
            bindMemberChangeEmailViewModel expectedId expectedCurrentPassword longNewEmail

        testMemberChangeEmailViewModel actualViewModel expectedId expectedCurrentPassword longNewEmail

        testModelState modelState "newEmail" "New email is longer than 254 characters"

    [<Test>]
    let ``Binding to a member change email view model, with new email in invalid format, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let invalidFormatNewEmail = String.random 32

        let (actualViewModel, modelState) =
            bindMemberChangeEmailViewModel expectedId expectedCurrentPassword invalidFormatNewEmail

        testMemberChangeEmailViewModel actualViewModel expectedId expectedCurrentPassword invalidFormatNewEmail

        testModelState modelState "newEmail" "New email is not in the correct format"

    [<Test>]
    let ``Binding to a member change email view model, with valid parameters, adds no model state errors`` () =
        let expectedId = Guid.random ()
        let expectedCurrentPassword = Password.random 32
        let expectedNewEmail = EmailAddress.random 32

        let (actualViewModel, modelState) =
            bindMemberChangeEmailViewModel expectedId expectedCurrentPassword expectedNewEmail

        testMemberChangeEmailViewModel actualViewModel expectedId expectedCurrentPassword expectedNewEmail

        modelState.IsValid =! true
