namespace Fooble.UnitTest.MemberDetail

open Fooble.Core
open Fooble.UnitTest
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDetailReadModelTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel = MemberDetail.ReadModel.make (Guid.random ()) (String.random 32) (String.random 64)

        test <@ box readModel :? IMemberDetailReadModel @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let readModel = MemberDetail.ReadModel.make expectedId (String.random 32) (String.random 64)

        test <@ readModel.Id = expectedId @>

    [<Test>]
    let ``Calling username, returns expected username`` () =
        let expectedUsername = String.random 32

        let readModel = MemberDetail.ReadModel.make (Guid.random ()) expectedUsername (String.random 64)

        test <@ readModel.Username = expectedUsername @>

    [<Test>]
    let ``Calling name, returns expected name`` () =
        let expectedName = String.random 64

        let readModel = MemberDetail.ReadModel.make (Guid.random ()) (String.random 32) expectedName

        test <@ readModel.Name = expectedName @>
