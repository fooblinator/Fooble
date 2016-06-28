namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberListItemReadModelTests =

    let private testMemberListItemReadModel (actual:IMemberListItemReadModel) id nickname avatarData =
        actual.Id =! id
        actual.Nickname =! nickname
        actual.AvatarData =! avatarData

    [<Test>]
    let ``Calling make item, with successful parameters, returns expected read model`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 64
        let avatarData = randomString 32
        let readModel = MemberListReadModel.makeItem id nickname avatarData
        testMemberListItemReadModel readModel id nickname avatarData
