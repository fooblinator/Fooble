namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module SelfServiceRegisterCommandTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns command`` () =
        let command = SelfServiceRegister.Command.make <|| (randomGuid (), randomString ())

        test <@ box command :? ISelfServiceRegisterCommand @>
        test <@ box command :? IRequest<Unit> @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = randomGuid ()

        let command = SelfServiceRegister.Command.make <|| (expectedId, randomString ())

        let actualId = command.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling name, returns expected name`` () =
        let expectedName = randomString ()

        let command = SelfServiceRegister.Command.make <|| (randomGuid (), expectedName)

        let actualName = command.Name
        test <@ actualName = expectedName @>
