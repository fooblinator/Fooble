namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListReadModelTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let members = Seq.init 5 (fun _ -> makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))
        let readModel = makeTestMemberListReadModel members

        test <@ box readModel :? IMemberListReadModel @>

    [<Test>]
    let ``Calling members, returns expected members`` () =
        let expectedMembers = List.init 5 (fun _ ->
            makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))

        let actualReadModel = makeTestMemberListReadModel (Seq.ofList expectedMembers)

        testMemberListReadModel2 actualReadModel expectedMembers
