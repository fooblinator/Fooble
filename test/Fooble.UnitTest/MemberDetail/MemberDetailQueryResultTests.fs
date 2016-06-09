namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailQueryResultTests =

    [<Test>]
    let ``Calling read model, as not found result, raises expected exception`` () =
        let expectedMessage = "Result was not successful"

        let queryResult = MemberDetailQuery.notFoundResult

        testInvalidOperationException expectedMessage <@ queryResult.ReadModel @>

    [<Test>]
    let ``Calling read model, as success result, returns expected read model`` () =
        let expectedReadModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32)
                (String.random 64) DateTime.UtcNow DateTime.UtcNow

        let queryResult = MemberDetailQuery.makeSuccessResult expectedReadModel

        queryResult.ReadModel =! expectedReadModel

    [<Test>]
    let ``Calling is success, as success result, returns true`` () =
        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32)
                (String.random 64) DateTime.UtcNow DateTime.UtcNow
        let queryResult = MemberDetailQuery.makeSuccessResult readModel

        queryResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, as not found result, returns false`` () =
        let queryResult = MemberDetailQuery.notFoundResult

        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling is not found, as success result, returns false`` () =
        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32)
                (String.random 64) DateTime.UtcNow DateTime.UtcNow
        let queryResult = MemberDetailQuery.makeSuccessResult readModel

        queryResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, as not found result, returns true`` () =
        let queryResult = MemberDetailQuery.notFoundResult

        queryResult.IsNotFound =! true

    [<Test>]
    let ``Calling to message display read model, as success result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let queryResult =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32)
                (String.random 64) DateTime.UtcNow DateTime.UtcNow
            |> MemberDetailQuery.makeSuccessResult

        testInvalidOperationException expectedMessage
            <@ MemberDetailQueryResult.toMessageDisplayReadModel queryResult @>

    [<Test>]
    let ``Calling to message display read model, as not found result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Detail"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let actualReadModel =
            MemberDetailQuery.notFoundResult |> MemberDetailQueryResult.toMessageDisplayReadModel

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
