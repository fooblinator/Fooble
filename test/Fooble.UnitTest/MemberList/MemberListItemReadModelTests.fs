namespace Fooble.UnitTest

open Fooble.Common
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListItemReadModelTests =

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let itemReadModel = makeTestMemberListItemReadModel expectedId (String.random 64)

        itemReadModel.Id =! expectedId

    [<Test>]
    let ``Calling nickname, returns expected nickname`` () =
        let expectedNickname = String.random 64

        let itemReadModel = makeTestMemberListItemReadModel (Guid.random ()) expectedNickname

        itemReadModel.Nickname =! expectedNickname
