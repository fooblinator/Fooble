﻿namespace Fooble.UnitTest.MemberList

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListReadModelTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let members = Seq.init 5 (fun _ -> MemberListReadModel.makeItem (Guid.random ()) (String.random 64))
        let readModel = MemberListReadModel.make members

        test <@ box readModel :? IMemberListReadModel @>

    [<Test>]
    let ``Calling members, returns expected members`` () =
        let expectedMembers = List.init 5 (fun _ ->
            MemberListReadModel.makeItem (Guid.random ()) (String.random 64))

        let readModel = MemberListReadModel.make (Seq.ofList expectedMembers)

        let actualMembers = Seq.toList readModel.Members
        test <@ List.length actualMembers = 5 @>
        for current in actualMembers do
            let findResult =
                List.tryFind (fun (x:IMemberListItemReadModel) -> x.Id = current.Id && x.Nickname = current.Nickname)
                    expectedMembers
            test <@ findResult.IsSome @>
