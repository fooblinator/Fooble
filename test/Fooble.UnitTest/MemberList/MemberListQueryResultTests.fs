namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListQueryResultTests =

    [<Test>]
    let ``Calling read model, as not found result, raises expected exception`` () =
        let expectedMessage = "Result was not successful"

        let queryResult = MemberListQuery.notFoundResult

        testInvalidOperationException expectedMessage <@ queryResult.ReadModel @>

    [<Test>]
    let ``Calling read model, as success result, returns expected read model`` () =
        let members =
            Seq.init 5 (fun _ -> makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))
        let expectedReadModel = makeTestMemberListReadModel members

        let queryResult = MemberListQuery.makeSuccessResult expectedReadModel

        queryResult.ReadModel =! expectedReadModel

    [<Test>]
    let ``Calling is success, as success result, returns true`` () =
        let members =
            Seq.init 5 (fun _ -> makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))
        let readModel = makeTestMemberListReadModel members

        let queryResult = MemberListQuery.makeSuccessResult readModel

        queryResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, as not found result, returns false`` () =
        let queryResult = MemberListQuery.notFoundResult

        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling is not found, as success result, returns false`` () =
        let members =
            Seq.init 5 (fun _ -> makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))
        let readModel = makeTestMemberListReadModel members

        let queryResult = MemberListQuery.makeSuccessResult readModel

        queryResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, as not found result, returns true`` () =
        let queryResult = MemberListQuery.notFoundResult

        queryResult.IsNotFound =! true

    [<Test>]
    let ``Calling to message display read model, as success result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let queryResult =
            makeTestMemberListItemReadModel (Guid.random ()) (String.random 64)
            |> Seq.singleton
            |> makeTestMemberListReadModel
            |> MemberListQuery.makeSuccessResult

        testInvalidOperationException expectedMessage
            <@ MemberListQueryResult.toMessageDisplayReadModel queryResult @>

    [<Test>]
    let ``Calling to message display read model, as not found result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "List"
        let expectedStatusCode = 200
        let expectedSeverity = MessageDisplayReadModel.informationalSeverity
        let expectedMessage = "No members have yet been added."

        let actualReadModel =
            MemberListQuery.notFoundResult |> MemberListQueryResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
