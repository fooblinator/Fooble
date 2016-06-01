namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailExtensionsTests =

    [<Test>]
    let ``Calling to message display read model, as success result of member detail query result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let queryResult =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random ())
                (String.random 64)
            |> MemberDetailQuery.makeSuccessResult

        raisesWith<InvalidOperationException> <@ MemberDetailExtensions.toMessageDisplayReadModel queryResult @>
            (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as not found result of member detail query result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Detail"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let readModel = MemberDetailQuery.notFoundResult |> MemberDetailExtensions.toMessageDisplayReadModel

        test <@ readModel.Heading = expectedHeading @>
        test <@ readModel.SubHeading = expectedSubHeading @>
        test <@ readModel.StatusCode = expectedStatusCode @>
        test <@ readModel.Severity = expectedSeverity @>
        test <@ readModel.Message = expectedMessage @>
