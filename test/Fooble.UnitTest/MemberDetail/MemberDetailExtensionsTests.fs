namespace Fooble.UnitTest.MemberDetail

open Fooble.Core
open Fooble.UnitTest
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailExtensionsTests =

    [<Test>]
    let ``Calling to message display read model, as success result of member detail query result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let queryResult =
            MemberDetail.ReadModel.make (Guid.random ()) (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64)
            |> MemberDetail.QueryResult.makeSuccess

        raisesWith<InvalidOperationException> <@ MessageDisplay.ofMemberDetailQueryResult queryResult @> (fun x ->
            <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as not found result of member detail query result, returns expected read model`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "Detail"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplay.Severity.warning
        let expectedMessage = "No matching member could be found."

        let readModel = MemberDetail.QueryResult.notFound |> MessageDisplay.ofMemberDetailQueryResult

        test <@ readModel.Heading = expectedHeading @>
        test <@ readModel.SubHeading = expectedSubHeading @>
        test <@ readModel.StatusCode = expectedStatusCode @>
        test <@ readModel.Severity = expectedSeverity @>
        test <@ readModel.Message = expectedMessage @>
