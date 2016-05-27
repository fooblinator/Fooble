﻿namespace Fooble.UnitTest.MemberList

open Fooble.Core
open Fooble.UnitTest
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListReadModelTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let members = Seq.init 5 (fun _ -> MemberList.ItemReadModel.make (Guid.random ()) (String.random 64))
        let readModel = MemberList.ReadModel.make members

        test <@ box readModel :? IMemberListReadModel @>

    [<Test>]
    let ``Calling members, returns expected members`` () =
        let expectedMembers = List.init 5 (fun _ ->
            MemberList.ItemReadModel.make (Guid.random ()) (String.random 64))

        let readModel = MemberList.ReadModel.make (Seq.ofList expectedMembers)

        let actualMembers = Seq.toList readModel.Members
        test <@ List.length actualMembers = 5 @>
        for current in actualMembers do
            let findResult =
                List.tryFind (fun (x:IMemberListItemReadModel) -> x.Id = current.Id && x.Nickname = current.Nickname)
                    expectedMembers
            test <@ findResult.IsSome @>
