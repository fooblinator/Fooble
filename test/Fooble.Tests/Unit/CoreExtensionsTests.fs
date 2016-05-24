namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module CoreExtensionsTests =

    [<Test>]
    let ``Calling to message display read model, as success result of member detail query result, returns expected read model`` () =
        let expectedReadModel =
            MessageDisplay.ReadModel.make "Member" "Detail" 200 MessageDisplay.Severity.informational
                "Member detail query was successful"

        let readModel =
            MemberDetail.ReadModel.make <|| (randomGuid (), randomString ())
            |> MemberDetail.QueryResult.makeSuccess
            |> CoreExtensions.ToMessageDisplayReadModel

        test <@ readModel = expectedReadModel @>

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
    let ``Calling to message display read model, as success result with member list query result, returns expected read model`` () =
        let expectedReadModel =
            MessageDisplay.ReadModel.make "Member" "List" 200 MessageDisplay.Severity.informational
                "Member list query was successful"

        let readModel =
            MemberList.ItemReadModel.make <|| (randomGuid (), randomString ())
            |> Seq.singleton
            |> MemberList.ReadModel.make
            |> MemberList.QueryResult.makeSuccess
            |> CoreExtensions.ToMessageDisplayReadModel

        test <@ readModel = expectedReadModel @>

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
    let ``Calling to message display read model, as valid result of validation result, returns expected read model`` () =
        let expectedReadModel =
            MessageDisplay.ReadModel.make "Validation" String.empty 200 MessageDisplay.Severity.informational
                "Validation was successful"

        let readModel =
            Validation.Result.valid
            |> CoreExtensions.ToMessageDisplayReadModel

        test <@ readModel = expectedReadModel @>

    [<Test>]
    let ``Calling to message display read model, as invalid result of validation result, returns expected read model`` () =
        let innerMessage = randomString ()
        let expectedReadModel =
            MessageDisplay.ReadModel.make "Validation" String.empty 400 MessageDisplay.Severity.error
                (sprintf "Validation was not successful and returned: \"%s\"" innerMessage)

        let readModel =
            Validation.Result.makeInvalid <|| (randomString (), innerMessage)
            |> CoreExtensions.ToMessageDisplayReadModel

        test <@ readModel = expectedReadModel @>
