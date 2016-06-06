namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailReadModelTests =

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let readModel =
            makeTestMemberDetailReadModel expectedId (String.random 32) (EmailAddress.random 32) (String.random 64)
                DateTime.UtcNow DateTime.UtcNow

        readModel.Id =! expectedId

    [<Test>]
    let ``Calling username, returns expected username`` () =
        let expectedUsername = String.random 32

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) expectedUsername (EmailAddress.random 32) (String.random 64)
                DateTime.UtcNow DateTime.UtcNow

        readModel.Username =! expectedUsername

    [<Test>]
    let ``Calling email, returns expected email`` () =
        let expectedEmail = EmailAddress.random 32

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) expectedEmail (String.random 64)
                DateTime.UtcNow DateTime.UtcNow

        readModel.Email =! expectedEmail

    [<Test>]
    let ``Calling nickname, returns expected nickname`` () =
        let expectedNickname = String.random 64

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32) expectedNickname
                DateTime.UtcNow DateTime.UtcNow

        readModel.Nickname =! expectedNickname

    [<Test>]
    let ``Calling registered, returns expected registered`` () =
        let expectedRegistered = DateTime.UtcNow

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32) (String.random 64)
                expectedRegistered DateTime.UtcNow

        readModel.Registered =! expectedRegistered

    [<Test>]
    let ``Calling password changed, returns expected password changed`` () =
        let expectedPasswordChanged = DateTime.UtcNow

        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32) (String.random 64)
                DateTime.UtcNow expectedPasswordChanged

        readModel.PasswordChanged =! expectedPasswordChanged
