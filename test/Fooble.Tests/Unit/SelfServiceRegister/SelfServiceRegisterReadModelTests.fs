namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
 
[<TestFixture>]
module SelfServiceRegisterReadModelTests =
 
    [<Test>]
    let ``Calling make initial, returns read model`` () =
        let readModel = SelfServiceRegister.ReadModel.empty

        test <@ box readModel :? ISelfServiceRegisterReadModel @>
 
    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel = SelfServiceRegister.ReadModel.make <| randomString ()

        test <@ box readModel :? ISelfServiceRegisterReadModel @>

    [<Test>]
    let ``Calling name, with initial read model, returns expected name`` () =
        let expectedName = String.empty

        let readModel = SelfServiceRegister.ReadModel.empty

        let actualName = readModel.Name
        test <@ actualName = expectedName @>

    [<Test>]
    let ``Calling name, with not initial read model, returns expected name`` () =
        let expectedName = randomString ()

        let readModel = SelfServiceRegister.ReadModel.make expectedName

        let actualName = readModel.Name
        test <@ actualName = expectedName @>
