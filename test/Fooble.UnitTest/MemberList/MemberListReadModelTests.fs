namespace Fooble.UnitTest

open Fooble.Common
open NUnit.Framework

[<TestFixture>]
module MemberListReadModelTests =

    [<Test>]
    let ``Calling members, returns expected members`` () =
        let expectedMemberCount = 5
        let expectedMembers = List.init expectedMemberCount (fun _ ->
            makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))

        let actualReadModel = makeTestMemberListReadModel (Seq.ofList expectedMembers) expectedMemberCount

        testMemberListReadModel2 actualReadModel expectedMembers expectedMemberCount

    [<Test>]
    let ``Calling member count, returns expected member count`` () =
        let expectedMemberCount = 5

        let members = List.init expectedMemberCount (fun _ ->
            makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))
        let actualReadModel = makeTestMemberListReadModel (Seq.ofList members) expectedMemberCount

        testMemberListReadModel2 actualReadModel members expectedMemberCount
