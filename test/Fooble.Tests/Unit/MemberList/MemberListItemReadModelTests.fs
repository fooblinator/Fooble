namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListItemReadModelTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel = MemberList.ItemReadModel.make (Guid.random ()) (String.random 64)

        test <@ box readModel :? IMemberListItemReadModel @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let itemReadModel = MemberList.ItemReadModel.make expectedId (String.random 64)

        test <@ itemReadModel.Id = expectedId @>

    [<Test>]
    let ``Calling name, returns expected name`` () =
        let expectedName = String.random 64

        let itemReadModel = MemberList.ItemReadModel.make (Guid.random ()) expectedName

        test <@ itemReadModel.Name = expectedName @>
