﻿namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
 
[<TestFixture>]
module MemberListItemReadModelTests =
 
    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel = MemberList.ItemReadModel.make <|| (randomGuid (), randomString ())

        test <@ box readModel :? IMemberListItemReadModel @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = randomGuid ()

        let itemReadModel = MemberList.ItemReadModel.make <|| (expectedId, randomString ())

        let actualId = itemReadModel.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling name, returns expected name`` () =
        let expectedName = randomString ()

        let itemReadModel = MemberList.ItemReadModel.make <|| (randomGuid (), expectedName)

        let actualName = itemReadModel.Name
        test <@ actualName = expectedName @>