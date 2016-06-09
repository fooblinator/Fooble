namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberExistsQueryResultTests =

    [<Test>]
    let ``Calling is success, as success result, returns true`` () =
        let queryResult = MemberExistsQuery.successResult

        queryResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, as not found result, returns false`` () =
        let queryResult = MemberExistsQuery.notFoundResult

        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling is not found, as success result, returns false`` () =
        let queryResult = MemberExistsQuery.successResult

        queryResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, as not found result, returns true`` () =
        let queryResult = MemberExistsQuery.notFoundResult

        queryResult.IsNotFound =! true

    [<Test>]
    let ``Calling to message display read model, with null sub-heading, raises expected exception`` () =
        let expectedParamName = "subHeading"
        let expectedMessage = "Sub-heading is required"

        let queryResult = MemberExistsQuery.successResult

        testArgumentException expectedParamName expectedMessage
            <@ MemberExistsQueryResult.toMessageDisplayReadModel queryResult null @>

    [<Test>]
    let ``Calling to message display read model, as success result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let queryResult = MemberExistsQuery.successResult

        testInvalidOperationException expectedMessage
            <@ MemberExistsQueryResult.toMessageDisplayReadModel queryResult (String.random 32) @>

    [<Test>]
    let ``Calling to message display read model, as not found result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Change Password"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let actualReadModel =
            MemberExistsQueryResult.toMessageDisplayReadModel MemberExistsQuery.notFoundResult expectedSubHeading

        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
