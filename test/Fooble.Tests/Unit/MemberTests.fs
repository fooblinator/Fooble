namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberTests =

    [<Test>]
    let ``Calling validate id, with empty id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was an empty GUID"

        let result = Member.validateId Guid.empty

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate id, with valid id, returns no messages`` () =
        let result = Member.validateId <| randomGuid ()

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling validate id string, with null id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was null"

        let result = Member.validateIdString null

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate id string, with empty id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was an empty string"

        let result = Member.validateIdString String.empty

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate id string, with invalid format id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was not in GUID format"

        let result = Member.validateIdString <| randomNonGuidString ()

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate id string, with valid id, returns no messages`` () =
        let result = Member.validateIdString <| randomString ()

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling validate name, with null name, returns expected validation result`` () =
        let expectedParamName = "name"
        let expectedMessage = "Name parameter was null"

        let result = Member.validateName null

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate name, with empty name, returns expected validation result`` () =
        let expectedParamName = "name"
        let expectedMessage = "Name parameter was an empty string"

        let result = Member.validateName String.empty

        test <@ result.IsInvalid @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling validate name, with valid name, returns no messages`` () =
        let result = Member.validateName <| randomString ()

        test <@ result.IsValid @>
