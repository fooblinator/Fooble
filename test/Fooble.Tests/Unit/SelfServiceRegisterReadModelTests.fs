namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
 
[<TestFixture>]
module SelfServiceRegisterReadModelTests =
 
    [<Test>]
    let ``Calling make initial, returns read model`` () =
        let readModel = SelfServiceRegister.makeInitialReadModel ()

        test <@ box readModel :? ISelfServiceRegisterReadModel @>
 
    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel = SelfServiceRegister.makeReadModel <| randomString ()

        test <@ box readModel :? ISelfServiceRegisterReadModel @>

    [<Test>]
    let ``Calling name, with initial read model, returns expected name`` () =
        let expectedName = String.empty

        let readModel = SelfServiceRegister.makeInitialReadModel ()

        let actualName = readModel.Name
        test <@ actualName = expectedName @>

    [<Test>]
    let ``Calling name, with not initial read model, returns expected name`` () =
        let expectedName = randomString ()

        let readModel = SelfServiceRegister.makeReadModel expectedName

        let actualName = readModel.Name
        test <@ actualName = expectedName @>
