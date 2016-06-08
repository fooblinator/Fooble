namespace Fooble.UnitTest

open Fooble.Common
open NUnit.Framework

[<TestFixture>]
module MemberListReadModelTests =

    [<Test>]
    let ``Calling members, returns expected members`` () =
        let expectedMembers = List.init 5 (fun _ ->
            makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))

        let actualReadModel = makeTestMemberListReadModel (Seq.ofList expectedMembers)

        testMemberListReadModel2 actualReadModel expectedMembers
