namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailReadModelTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns read model`` () =
        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32)
                (String.random 64) DateTime.Now DateTime.Now

        box readModel :? IMemberDetailReadModel =! true

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let readModel =
            makeTestMemberDetailReadModel expectedId (String.random 32) (EmailAddress.random 32) (String.random 64)
                DateTime.Now DateTime.Now

        readModel.Id =! expectedId

    [<Test>]
    let ``Calling username, returns expected username`` () =
        let expectedUsername = String.random 32

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) expectedUsername (EmailAddress.random 32) (String.random 64)
                DateTime.Now DateTime.Now

        readModel.Username =! expectedUsername

    [<Test>]
    let ``Calling email, returns expected email`` () =
        let expectedEmail = EmailAddress.random 32

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) expectedEmail (String.random 64)
                DateTime.Now DateTime.Now

        readModel.Email =! expectedEmail

    [<Test>]
    let ``Calling nickname, returns expected nickname`` () =
        let expectedNickname = String.random 64

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32) expectedNickname
                DateTime.Now DateTime.Now

        readModel.Nickname =! expectedNickname

    [<Test>]
    let ``Calling registered, returns expected registered`` () =
        let expectedRegistered = DateTime.Now

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32) (String.random 64)
                expectedRegistered DateTime.Now

        readModel.Registered =! expectedRegistered

    [<Test>]
    let ``Calling password changed, returns expected password changed`` () =
        let expectedPasswordChanged = DateTime.Now

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32) (String.random 64)
                DateTime.Now expectedPasswordChanged

        readModel.PasswordChanged =! expectedPasswordChanged
