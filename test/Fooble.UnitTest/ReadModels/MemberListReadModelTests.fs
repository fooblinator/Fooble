namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open System

[<TestFixture>]
module MemberListReadModelTests =

    [<Test>]
    let ``Calling make, with successful parameters, returns expected read model`` () =
        let memberCount = 5
        let members = List.init memberCount (fun _ -> (Guid.NewGuid(), randomString 64, randomString 32))
        let readModel =
            members
            |> Seq.map (fun (id, nickname, avatarData) -> MemberListReadModel.makeItem id nickname avatarData)
            |> fun xs -> MemberListReadModel.make xs memberCount
        testMemberListReadModel readModel members memberCount
