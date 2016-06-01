namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDetailReadModelTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random ())
                (String.random 64)

        test <@ box readModel :? IMemberDetailReadModel @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let readModel =
            makeTestMemberDetailReadModel expectedId (String.random 32) (EmailAddress.random ()) (String.random 64)

        test <@ readModel.Id = expectedId @>

    [<Test>]
    let ``Calling username, returns expected username`` () =
        let expectedUsername = String.random 32

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) expectedUsername (EmailAddress.random ()) (String.random 64)

        test <@ readModel.Username = expectedUsername @>

    [<Test>]
    let ``Calling email, returns expected email`` () =
        let expectedEmail = EmailAddress.random ()

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) expectedEmail (String.random 64)

        test <@ readModel.Email = expectedEmail @>

    [<Test>]
    let ``Calling nickname, returns expected nickname`` () =
        let expectedNickname = String.random 64

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random ()) expectedNickname

        test <@ readModel.Nickname = expectedNickname @>
