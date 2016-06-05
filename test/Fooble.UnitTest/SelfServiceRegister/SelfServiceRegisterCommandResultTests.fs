namespace Fooble.UnitTest

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegisterCommandResultTests =

    [<Test>]
    let ``Calling success, returns command result`` () =
        let commandResult = SelfServiceRegisterCommand.successResult

        box commandResult :? ISelfServiceRegisterCommandResult =! true

    [<Test>]
    let ``Calling username unavailable, returns command result`` () =
        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult

        box commandResult :? ISelfServiceRegisterCommandResult =! true

    [<Test>]
    let ``Calling email unavailable, returns command result`` () =
        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult

        box commandResult :? ISelfServiceRegisterCommandResult =! true

    [<Test>]
    let ``Calling is success, with success command result, returns true`` () =
        let commandResult = SelfServiceRegisterCommand.successResult

        commandResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, with username unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, with email unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is username unavailable, with success command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.successResult

        commandResult.IsUsernameUnavailable =! false

    [<Test>]
    let ``Calling is username unavailable, with username unavailable command result, returns true`` () =
        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult

        commandResult.IsUsernameUnavailable =! true

    [<Test>]
    let ``Calling is username unavailable, with email unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult

        commandResult.IsUsernameUnavailable =! false

    [<Test>]
    let ``Calling is email unavailable, with success command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.successResult

        commandResult.IsEmailUnavailable =! false

    [<Test>]
    let ``Calling is email unavailable, with username unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult

        commandResult.IsEmailUnavailable =! false

    [<Test>]
    let ``Calling is email unavailable, with email unavailable command result, returns true`` () =
        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult

        commandResult.IsEmailUnavailable =! true
