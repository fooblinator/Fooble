namespace Fooble.UnitTest.SelfServiceRegister

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegisterCommandResultTests =

    [<Test>]
    let ``Calling success, returns command result`` () =
        let commandResult = SelfServiceRegister.CommandResult.success

        test <@ box commandResult :? ISelfServiceRegisterCommandResult @>

    [<Test>]
    let ``Calling username unavailable, returns command result`` () =
        let commandResult = SelfServiceRegister.CommandResult.usernameUnavailable

        test <@ box commandResult :? ISelfServiceRegisterCommandResult @>

    [<Test>]
    let ``Calling email unavailable, returns command result`` () =
        let commandResult = SelfServiceRegister.CommandResult.emailUnavailable

        test <@ box commandResult :? ISelfServiceRegisterCommandResult @>

    [<Test>]
    let ``Calling is success, with success command result, returns true`` () =
        let commandResult = SelfServiceRegister.CommandResult.success

        test <@ commandResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with username unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegister.CommandResult.usernameUnavailable

        test <@ not <| commandResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with email unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegister.CommandResult.emailUnavailable

        test <@ not <| commandResult.IsSuccess @>

    [<Test>]
    let ``Calling is username unavailable, with success command result, returns false`` () =
        let commandResult = SelfServiceRegister.CommandResult.success

        test <@ not <| commandResult.IsUsernameUnavailable @>

    [<Test>]
    let ``Calling is username unavailable, with username unavailable command result, returns true`` () =
        let commandResult = SelfServiceRegister.CommandResult.usernameUnavailable

        test <@ commandResult.IsUsernameUnavailable @>

    [<Test>]
    let ``Calling is username unavailable, with email unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegister.CommandResult.emailUnavailable

        test <@ not <| commandResult.IsUsernameUnavailable @>

    [<Test>]
    let ``Calling is email unavailable, with success command result, returns false`` () =
        let commandResult = SelfServiceRegister.CommandResult.success

        test <@ not <| commandResult.IsEmailUnavailable @>

    [<Test>]
    let ``Calling is email unavailable, with username unavailable command result, returns false`` () =
        let commandResult = SelfServiceRegister.CommandResult.usernameUnavailable

        test <@ not <| commandResult.IsEmailUnavailable @>

    [<Test>]
    let ``Calling is email unavailable, with email unavailable command result, returns true`` () =
        let commandResult = SelfServiceRegister.CommandResult.emailUnavailable

        test <@ commandResult.IsEmailUnavailable @>
