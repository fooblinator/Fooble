namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module CoreExtensionsTests =

    [<Test>]
    let ``Calling to message display read model, as success result of member detail query result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let queryResult =
            MemberDetail.ReadModel.make (Guid.random ()) (String.random 32) (String.random 64)
            |> MemberDetail.QueryResult.makeSuccess

        raisesWith<InvalidOperationException> <@ queryResult.ToMessageDisplayReadModel() @> (fun x ->
            <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as not found result of member detail query result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Detail"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplay.Severity.warning
        let expectedMessage = "No matching member could be found."

        let readModel =
            MemberDetail.QueryResult.notFound
            |> CoreExtensions.ToMessageDisplayReadModel

        test <@ readModel.Heading = expectedHeading @>
        test <@ readModel.SubHeading = expectedSubHeading @>
        test <@ readModel.StatusCode = expectedStatusCode @>
        test <@ readModel.Severity = expectedSeverity @>
        test <@ readModel.Message = expectedMessage @>

    [<Test>]
    let ``Calling to message display read model, as success result of member list query result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let queryResult =
            MemberList.ItemReadModel.make (Guid.random ()) (String.random 64)
            |> Seq.singleton
            |> MemberList.ReadModel.make
            |> MemberList.QueryResult.makeSuccess

        raisesWith<InvalidOperationException> <@ queryResult.ToMessageDisplayReadModel() @>
            (fun e -> <@ e.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as not found result with member list query result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "List"
        let expectedStatusCode = 200
        let expectedSeverity = MessageDisplay.Severity.informational
        let expectedMessage = "No members have yet been added."

        let readModel =
            MemberList.QueryResult.notFound
            |> CoreExtensions.ToMessageDisplayReadModel

        test <@ readModel.Heading = expectedHeading @>
        test <@ readModel.SubHeading = expectedSubHeading @>
        test <@ readModel.StatusCode = expectedStatusCode @>
        test <@ readModel.Severity = expectedSeverity @>
        test <@ readModel.Message = expectedMessage @>

    [<Test>]
    let ``Calling to message display read model, as success result of self-service register command result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let commandResult = SelfServiceRegister.CommandResult.success

        raisesWith<InvalidOperationException> <@ commandResult.ToMessageDisplayReadModel() @> (fun x ->
            <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as username unavailable result of self-service register command result, returns expected read model`` () =
        let expectedHeading = "Self-Service"
        let expectedSubHeading = "Register"
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplay.Severity.warning
        let expectedMessage = "Requested username is unavailable."

        let readModel =
            SelfServiceRegister.CommandResult.usernameUnavailable
            |> CoreExtensions.ToMessageDisplayReadModel

        test <@ readModel.Heading = expectedHeading @>
        test <@ readModel.SubHeading = expectedSubHeading @>
        test <@ readModel.StatusCode = expectedStatusCode @>
        test <@ readModel.Severity = expectedSeverity @>
        test <@ readModel.Message = expectedMessage @>

    [<Test>]
    let ``Calling to message display read model, as valid result of validation result, raises expected exception`` () =
        let expectedMessage = "Result was not invalid"

        let result = Validation.Result.valid

        raisesWith<InvalidOperationException> <@ result.ToMessageDisplayReadModel() @> (fun x ->
            <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as invalid result of validation result, returns expected read model`` () =
        let innerMessage = String.random 64
        let expectedHeading = "Validation"
        let expectedSubHeading = String.empty
        let expectedStatusCode = 400
        let expectedSeverity = MessageDisplay.Severity.error
        let expectedMessage = (sprintf "Validation was not successful and returned: \"%s\"" innerMessage)

        let readModel =
            Validation.Result.makeInvalid (String.random 64) innerMessage
            |> CoreExtensions.ToMessageDisplayReadModel

        test <@ readModel.Heading = expectedHeading @>
        test <@ readModel.SubHeading = expectedSubHeading @>
        test <@ readModel.StatusCode = expectedStatusCode @>
        test <@ readModel.Severity = expectedSeverity @>
        test <@ readModel.Message = expectedMessage @>
