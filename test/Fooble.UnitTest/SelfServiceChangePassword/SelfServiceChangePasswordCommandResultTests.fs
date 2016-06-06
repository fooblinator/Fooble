namespace Fooble.UnitTest

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceChangePasswordCommandResultTests =

    [<Test>]
    let ``Calling is success, with success command result, returns true`` () =
        let commandResult = SelfServiceChangePasswordCommand.successResult

        commandResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, with not found command result, returns false`` () =
        let commandResult = SelfServiceChangePasswordCommand.notFoundResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, with invalid command result, returns false`` () =
        let commandResult = SelfServiceChangePasswordCommand.invalidResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is not found, with success command result, returns false`` () =
        let commandResult = SelfServiceChangePasswordCommand.successResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, with not found command result, returns true`` () =
        let commandResult = SelfServiceChangePasswordCommand.notFoundResult

        commandResult.IsNotFound =! true

    [<Test>]
    let ``Calling is not found, with invalid command result, returns false`` () =
        let commandResult = SelfServiceChangePasswordCommand.invalidResult

        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling is invalid, with success command result, returns false`` () =
        let commandResult = SelfServiceChangePasswordCommand.successResult

        commandResult.IsInvalid =! false

    [<Test>]
    let ``Calling is invalid, with not found command result, returns false`` () =
        let commandResult = SelfServiceChangePasswordCommand.notFoundResult

        commandResult.IsInvalid =! false

    [<Test>]
    let ``Calling is invalid, with invalid command result, returns true`` () =
        let commandResult = SelfServiceChangePasswordCommand.invalidResult

        commandResult.IsInvalid =! true
