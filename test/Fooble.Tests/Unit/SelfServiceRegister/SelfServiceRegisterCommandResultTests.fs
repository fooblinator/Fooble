namespace Fooble.Tests.Unit

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
    let ``Calling duplicate id, returns command result`` () =
        let commandResult = SelfServiceRegister.CommandResult.duplicateId

        test <@ box commandResult :? ISelfServiceRegisterCommandResult @>

    [<Test>]
    let ``Calling is success, with success command result, returns true`` () =
        let commandResult = SelfServiceRegister.CommandResult.success

        test <@ commandResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with duplicate id command result, returns false`` () =
        let commandResult = SelfServiceRegister.CommandResult.duplicateId

        test <@ not <| commandResult.IsSuccess @>

    [<Test>]
    let ``Calling is duplicate id, with success command result, returns false`` () =
        let commandResult = SelfServiceRegister.CommandResult.success

        test <@ not <| commandResult.IsDuplicateId @>

    [<Test>]
    let ``Calling is duplicate id, with duplicate id command result, returns true`` () =
        let commandResult = SelfServiceRegister.CommandResult.duplicateId

        test <@ commandResult.IsDuplicateId @>
