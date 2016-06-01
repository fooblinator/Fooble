namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListItemReadModelTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel = makeTestMemberListItemReadModel (Guid.random ()) (String.random 64)

        test <@ box readModel :? IMemberListItemReadModel @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let itemReadModel = makeTestMemberListItemReadModel expectedId (String.random 64)

        test <@ itemReadModel.Id = expectedId @>

    [<Test>]
    let ``Calling nickname, returns expected nickname`` () =
        let expectedNickname = String.random 64

        let itemReadModel = makeTestMemberListItemReadModel (Guid.random ()) expectedNickname

        test <@ itemReadModel.Nickname = expectedNickname @>
