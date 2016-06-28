namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open System

[<TestFixture>]
module MemberDetailReadModelTests =

    [<Test>]
    let ``Calling make, with successful parameters, returns expected read model`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let registered = DateTime.UtcNow
        let passwordChanged = DateTime.UtcNow
        let readModel = MemberDetailReadModel.make id username email nickname avatarData registered passwordChanged
        testMemberDetailReadModel readModel id username email nickname avatarData registered passwordChanged
