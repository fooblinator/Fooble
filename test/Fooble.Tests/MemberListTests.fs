namespace Fooble.Tests

open Fooble.Core
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberListTests =

    [<Test>]
    let ``Calling make query, with valid parameters, returns expected result``() =
        let query = MemberList.makeQuery()
        test <@ (box query) :? IMemberListQuery @>

    [<Test>]
    let ``Calling make item read model, with valid parameters, returns expected result``() =
        let expectedId = randomGuidString()
        let expectedName = randomGuidString()
        let readModel = MemberList.makeItemReadModel expectedId expectedName
        test <@ (box readModel) :? IMemberListItemReadModel @>
        let actualId = readModel.Id
        test <@ actualId = expectedId @>
        let actualName = readModel.Name
        test <@ actualName = expectedName @>

    [<Test>]
    let ``Calling item read model id, returns expected id``() =
        let expectedId = randomGuidString()
        let readModel = MemberList.makeItemReadModel expectedId (randomGuidString())
        let actualId = readModel.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling item read model name, returns expected name``() =
        let expectedName = randomGuidString()
        let readModel = MemberList.makeItemReadModel (randomGuidString()) expectedName
        let actualName = readModel.Name
        test <@ actualName = expectedName @>

    [<Test>]
    let ``Calling make read model, with valid parameters, returns expected result``() =
        let expectedMembers = [ MemberList.makeItemReadModel (randomGuidString()) (randomGuidString()) ]
        let readModel = MemberList.makeReadModel (Seq.ofList expectedMembers)
        test <@ (box readModel) :? IMemberListReadModel @>
        let actualMembers = List.ofSeq readModel.Members
        test <@ actualMembers = expectedMembers @>

    [<Test>]
    let ``Calling read model members, returns expected members``() =
        let expectedMembers = [ MemberList.makeItemReadModel (randomGuidString()) (randomGuidString()) ]
        let readModel = MemberList.makeReadModel (Seq.ofList expectedMembers)
        let actualMembers = List.ofSeq readModel.Members
        test <@ actualMembers = expectedMembers @>

    [<Test>]
    let ``Calling not found query failure status, returns expected status``() =
        let status = MemberList.notFoundQueryFailureStatus
        test <@ (box status) :? IMemberListQueryFailureStatus @>
        test <@ status.IsNotFound @>

    [<Test>]
    let ``Calling query failure status is not found, as not found, returns true``() =
        let status = MemberList.notFoundQueryFailureStatus
        test <@ status.IsNotFound @>

    [<Test>]
    let ``Calling make query handler, with valid parameters, returns expected result``() =
        let handler = MemberList.makeQueryHandler (mock())
        test <@ (box handler) :? IMemberListQueryHandler @>

    [<Test>]
    let ``Calling to message display read model, with null result, raises expected exception``() =
        let expectedParamName = "result"
        let expectedMessage = "Result was null value"
        raisesWith<ArgumentException> <@ MemberList.toMessageDisplayReadModel null @>
            (fun e ->
            <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as success result with member list read model, returns expected result``() =
        let expectedHeading = "Member List Query"
        let expectedSeverity = MessageDisplay.informationalSeverity
        let expectedMessages = [ "Member list query was successful" ]

        let readModel =
            [ MemberList.makeItemReadModel (randomGuidString()) (randomGuidString()) ]
            |> MemberList.makeReadModel
            |> Result.success
            |> MemberList.toMessageDisplayReadModel

        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>
