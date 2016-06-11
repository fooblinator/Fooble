namespace Fooble.UnitTest

open Fooble.Common
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module BindMemberChangeOtherViewModelTests =

    [<Test>]
    let ``Binding to a member change other view model, with null new nickname, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let nullNewNickname:string = null

        let (actualViewModel, modelState) = bindMemberChangeOtherViewModel expectedId nullNewNickname

        testMemberChangeOtherViewModel actualViewModel expectedId nullNewNickname

        testModelState modelState "newNickname" "New nickname is required"

    [<Test>]
    let ``Binding to a member change other view model, with empty new nickname, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let emptyNewNickname = String.empty

        let (actualViewModel, modelState) = bindMemberChangeOtherViewModel expectedId emptyNewNickname

        testMemberChangeOtherViewModel actualViewModel expectedId emptyNewNickname

        testModelState modelState "newNickname" "New nickname is required"

    [<Test>]
    let ``Binding to a member change other view model, with new nickname longer than 64 characters, adds expected model state error`` () =
        let expectedId = Guid.random ()
        let longNewNickname = String.random 65

        let (actualViewModel, modelState) = bindMemberChangeOtherViewModel expectedId longNewNickname

        testMemberChangeOtherViewModel actualViewModel expectedId longNewNickname

        testModelState modelState "newNickname" "New nickname is longer than 64 characters"

    [<Test>]
    let ``Binding to a member change other view model, with valid parameters, adds no model state errors`` () =
        let expectedId = Guid.random ()
        let expectedNewNickname = String.random 64

        let (actualViewModel, modelState) = bindMemberChangeOtherViewModel expectedId expectedNewNickname

        testMemberChangeOtherViewModel actualViewModel expectedId expectedNewNickname

        modelState.IsValid =! true
