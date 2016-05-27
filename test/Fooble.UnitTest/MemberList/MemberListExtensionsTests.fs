namespace Fooble.UnitTest.MemberList

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberListExtensionsTests =

    [<Test>]
    let ``Calling to message display read model, as success result of member list query result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let queryResult =
            MemberListReadModel.makeItem (Guid.random ()) (String.random 64)
            |> Seq.singleton
            |> MemberListReadModel.make
            |> MemberListQuery.makeSuccessResult

        raisesWith<InvalidOperationException> <@ MemberList.toMessageDisplayReadModel queryResult @>
            (fun e -> <@ e.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as not found result with member list query result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "List"
        let expectedStatusCode = 200
        let expectedSeverity = MessageDisplay.informationalSeverity
        let expectedMessage = "No members have yet been added."

        let readModel = MemberListQuery.notFoundResult |> MemberList.toMessageDisplayReadModel

        test <@ readModel.Heading = expectedHeading @>
        test <@ readModel.SubHeading = expectedSubHeading @>
        test <@ readModel.StatusCode = expectedStatusCode @>
        test <@ readModel.Severity = expectedSeverity @>
        test <@ readModel.Message = expectedMessage @>
