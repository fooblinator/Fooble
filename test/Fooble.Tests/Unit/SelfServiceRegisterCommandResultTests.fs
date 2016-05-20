namespace Fooble.Tests.Unit

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
 
[<TestFixture>]
module SelfServiceRegisterCommandResultTests =

    [<Test>]
    let ``Calling success, returns command result`` () =
        let commandResult = SelfServiceRegister.successResult

        test <@ box commandResult :? ISelfServiceRegisterCommandResult @>

    [<Test>]
    let ``Calling duplicate id, returns command result`` () =
        let commandResult = SelfServiceRegister.duplicateIdResult

        test <@ box commandResult :? ISelfServiceRegisterCommandResult @>

    [<Test>]
    let ``Calling is success, with success command result, returns true`` () =
        let commandResult = SelfServiceRegister.successResult

        test <@ commandResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with duplicate id command result, returns false`` () =
        let commandResult = SelfServiceRegister.duplicateIdResult

        test <@ not <| commandResult.IsSuccess @>

    [<Test>]
    let ``Calling is duplicate id, with success command result, returns false`` () =
        let commandResult = SelfServiceRegister.successResult

        test <@ not <| commandResult.IsDuplicateId @>

    [<Test>]
    let ``Calling is duplicate id, with duplicate id command result, returns true`` () =
        let commandResult = SelfServiceRegister.duplicateIdResult

        test <@ commandResult.IsDuplicateId @>
