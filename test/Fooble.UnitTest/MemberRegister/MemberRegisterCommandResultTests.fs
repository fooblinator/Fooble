namespace Fooble.UnitTest

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberRegisterCommandResultTests =

    [<Test>]
    let ``Calling is success, with success command result, returns true`` () =
        let commandResult = MemberRegisterCommand.successResult

        commandResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, with username unavailable command result, returns false`` () =
        let commandResult = MemberRegisterCommand.usernameUnavailableResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is success, with email unavailable command result, returns false`` () =
        let commandResult = MemberRegisterCommand.emailUnavailableResult

        commandResult.IsSuccess =! false

    [<Test>]
    let ``Calling is username unavailable, with success command result, returns false`` () =
        let commandResult = MemberRegisterCommand.successResult

        commandResult.IsUsernameUnavailable =! false

    [<Test>]
    let ``Calling is username unavailable, with username unavailable command result, returns true`` () =
        let commandResult = MemberRegisterCommand.usernameUnavailableResult

        commandResult.IsUsernameUnavailable =! true

    [<Test>]
    let ``Calling is username unavailable, with email unavailable command result, returns false`` () =
        let commandResult = MemberRegisterCommand.emailUnavailableResult

        commandResult.IsUsernameUnavailable =! false

    [<Test>]
    let ``Calling is email unavailable, with success command result, returns false`` () =
        let commandResult = MemberRegisterCommand.successResult

        commandResult.IsEmailUnavailable =! false

    [<Test>]
    let ``Calling is email unavailable, with username unavailable command result, returns false`` () =
        let commandResult = MemberRegisterCommand.usernameUnavailableResult

        commandResult.IsEmailUnavailable =! false

    [<Test>]
    let ``Calling is email unavailable, with email unavailable command result, returns true`` () =
        let commandResult = MemberRegisterCommand.emailUnavailableResult

        commandResult.IsEmailUnavailable =! true
