namespace Fooble.Tests

open Fooble.Core
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberListQueryTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() =
        ignore <| MemberList.Query.make()

[<TestFixture>]
module MemberListItemReadModelTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() =
        let expectedId = randomGuidString()
        let expectedName = randomGuidString()

        let readModel = MemberList.ItemReadModel.make expectedId expectedName

        let actualId = readModel.Id
        test <@ actualId = expectedId @>
        let actualName = readModel.Name
        test <@ actualName = expectedName @>

    [<Test>]
    let ``Calling id, returns expected id``() =
        let expectedId = randomGuidString()

        let readModel = MemberList.ItemReadModel.make expectedId (randomGuidString())

        let actualId = readModel.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling name, returns expected name``() =
        let expectedName = randomGuidString()

        let readModel = MemberList.ItemReadModel.make (randomGuidString()) expectedName

        let actualName = readModel.Name
        test <@ actualName = expectedName @>

[<TestFixture>]
module MemberListReadModelTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() =
        let expectedMembers = [ MemberList.ItemReadModel.make (randomGuidString()) (randomGuidString()) ]

        let readModel = MemberList.ReadModel.make (Seq.ofList expectedMembers)

        let actualMembers = List.ofSeq readModel.Members
        test <@ actualMembers = expectedMembers @>

    [<Test>]
    let ``Calling members, returns expected members``() =
        let expectedMembers = [ MemberList.ItemReadModel.make (randomGuidString()) (randomGuidString()) ]

        let readModel = MemberList.ReadModel.make (Seq.ofList expectedMembers)

        let actualMembers = List.ofSeq readModel.Members
        test <@ actualMembers = expectedMembers @>

[<TestFixture>]
module MemberListQueryFailureStatusTests =

    [<Test>]
    let ``Calling not found, returns expected status``() =
        let status = MemberList.QueryFailureStatus.notFound

        test <@ status.IsNotFound @>

    [<Test>]
    let ``Calling is not found, as not found query failure result, returns true``() =
        let status = MemberList.QueryFailureStatus.notFound

        test <@ status.IsNotFound @>

[<TestFixture>]
module MemberListQueryHandlerTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() =
        ignore <| MemberList.QueryHandler.make (mock())
