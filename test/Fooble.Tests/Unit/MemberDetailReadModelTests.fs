namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
 
[<TestFixture>]
module MemberDetailReadModelTests =
 
    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel = MemberDetail.makeReadModel <|| (randomGuid (), randomString ())

        test <@ box readModel :? IMemberDetailReadModel @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = randomGuid ()

        let readModel = MemberDetail.makeReadModel <|| (expectedId, randomString ())

        let actualId = readModel.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling name, returns expected name`` () =
        let expectedName = randomString ()

        let readModel = MemberDetail.makeReadModel <|| (randomGuid (), expectedName)

        let actualName = readModel.Name
        test <@ actualName = expectedName @>
