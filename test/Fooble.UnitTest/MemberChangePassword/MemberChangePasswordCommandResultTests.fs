namespace Fooble.UnitTest

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberChangePasswordCommandResultTests =

    [<Test>]
    let ``Calling is success, with success command result, returns true`` () =
        let commandResult = MemberChangePasswordCommand.successResult

        commandResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, with not found command result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.notFoundResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, with invalid command result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.incorrectPasswordResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is not found, with success command result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.successResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, with not found command result, returns true`` () =
        let commandResult = MemberChangePasswordCommand.notFoundResult

        commandResult.IsNotFound =! true

    [<Test>]
    let ``Calling is not found, with invalid command result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.incorrectPasswordResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is invalid, with success command result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.successResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is invalid, with not found command result, returns false`` () =
        let commandResult = MemberChangePasswordCommand.notFoundResult

        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling is invalid, with invalid command result, returns true`` () =
        let commandResult = MemberChangePasswordCommand.incorrectPasswordResult

        commandResult.IsIncorrectPassword =! true
