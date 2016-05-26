namespace Fooble.UnitTest.MemberList

open Fooble.Core
open Fooble.UnitTest
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberListExtensionsTests =

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
