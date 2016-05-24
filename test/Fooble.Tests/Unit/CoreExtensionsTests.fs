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
            MemberDetail.ReadModel.make (randomGuid ()) (randomString ())
            |> MemberDetail.QueryResult.makeSuccess
        
        raisesWith<InvalidOperationException> <@ queryResult.ToMessageDisplayReadModel() @>
            (fun e -> <@ e.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as not found result of member detail query result, returns expected read model`` () =
        let expectedReadModel =
            MessageDisplay.ReadModel.make "Member" "Detail" 404 MessageDisplay.Severity.warning
                "No matching member could be found."

        let readModel =
            MemberDetail.QueryResult.notFound
            |> CoreExtensions.ToMessageDisplayReadModel

        test <@ readModel = expectedReadModel @>
    
    [<Test>]
    let ``Calling to message display read model, as success result of member list query result, raises expected exception`` () =
        let expectedMessage = "Result was not unsuccessful"

        let queryResult =
            MemberList.ItemReadModel.make (randomGuid ()) (randomString ())
            |> Seq.singleton
            |> MemberList.ReadModel.make
            |> MemberList.QueryResult.makeSuccess
        
        raisesWith<InvalidOperationException> <@ queryResult.ToMessageDisplayReadModel() @>
            (fun e -> <@ e.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as not found result with member list query result, returns expected read model`` () =
        let expectedReadModel =
            MessageDisplay.ReadModel.make "Member" "List" 200 MessageDisplay.Severity.informational
                "No members have yet been added."

        let readModel =
            MemberList.QueryResult.notFound
            |> CoreExtensions.ToMessageDisplayReadModel

        test <@ readModel = expectedReadModel @>
    
    [<Test>]
    let ``Calling to message display read model, as valid result of validation result, raises expected exception`` () =
        let expectedMessage = "Result was not invalid"

        let result = Validation.Result.valid
        
        raisesWith<InvalidOperationException> <@ result.ToMessageDisplayReadModel() @>
            (fun e -> <@ e.Message = expectedMessage @>)

    [<Test>]
    let ``Calling to message display read model, as invalid result of validation result, returns expected read model`` () =
        let innerMessage = randomString ()
        let expectedReadModel =
            MessageDisplay.ReadModel.make "Validation" String.empty 400 MessageDisplay.Severity.error
                (sprintf "Validation was not successful and returned: \"%s\"" innerMessage)

        let readModel =
            Validation.Result.makeInvalid (randomString ()) innerMessage
            |> CoreExtensions.ToMessageDisplayReadModel

        test <@ readModel = expectedReadModel @>
