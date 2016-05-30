namespace Fooble.UnitTest

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegisterCommandResultTests =

    [<Test>]
    let ``Calling success, returns command result`` () =
        let commandResult = SelfServiceRegisterCommand.successResult

        test <@ box commandResult :? ISelfServiceRegisterCommandResult @>

    [<Test>]
    let ``Calling username unavailable, returns command result`` () =
        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult

        test <@ box commandResult :? ISelfServiceRegisterCommandResult @>

    [<Test>]
    let ``Calling email unavailable, returns command result`` () =
        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult

        test <@ box commandResult :? ISelfServiceRegisterCommandResult @>

    [<Test>]
    let ``Calling is success, with success command result, returns true`` () =
        let commandResult = SelfServiceRegisterCommand.successResult

        test <@ commandResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with username unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult

        test <@ not <| commandResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with email unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult

        test <@ not <| commandResult.IsSuccess @>

    [<Test>]
    let ``Calling is username unavailable, with success command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.successResult

        test <@ not <| commandResult.IsUsernameUnavailable @>

    [<Test>]
    let ``Calling is username unavailable, with username unavailable command result, returns true`` () =
        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult

        test <@ commandResult.IsUsernameUnavailable @>

    [<Test>]
    let ``Calling is username unavailable, with email unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult

        test <@ not <| commandResult.IsUsernameUnavailable @>

    [<Test>]
    let ``Calling is email unavailable, with success command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.successResult

        test <@ not <| commandResult.IsEmailUnavailable @>

    [<Test>]
    let ``Calling is email unavailable, with username unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegisterCommand.usernameUnavailableResult

        test <@ not <| commandResult.IsEmailUnavailable @>

    [<Test>]
    let ``Calling is email unavailable, with email unavailable command result, returns true`` () =
        let commandResult = SelfServiceRegisterCommand.emailUnavailableResult

        test <@ commandResult.IsEmailUnavailable @>
