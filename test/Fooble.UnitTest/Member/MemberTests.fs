﻿namespace Fooble.UnitTest.Member

open Fooble.Core
open Fooble.UnitTest
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberTests =

    [<Test>]
    let ``Calling validate id, with empty id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        let result = Member.validateId Guid.empty

        test <@ result.IsInvalid @>
        test <@ result.ParamName = expectedParamName @>
        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling validate id, with valid id, returns no messages`` () =
        let result = Member.validateId (Guid.random ())

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling validate id string, with null id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        let result = Member.validateIdString null

        test <@ result.IsInvalid @>
        test <@ result.ParamName = expectedParamName @>
        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling validate id string, with empty id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        let result = Member.validateIdString String.empty

        test <@ result.IsInvalid @>
        test <@ result.ParamName = expectedParamName @>
        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling validate id string, with invalid format id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is not in the correct format (GUID)"

        let result = Member.validateIdString (String.random 64)

        test <@ result.IsInvalid @>
        test <@ result.ParamName = expectedParamName @>
        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling validate id string, with valid id, returns no messages`` () =
        let result = Member.validateIdString (Guid.random () |> String.ofGuid)

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling validate username, with null username, returns expected validation result`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is required"

        let result = Member.validateUsername null

        test <@ result.IsInvalid @>
        test <@ result.ParamName = expectedParamName @>
        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling validate username, with empty username, returns expected validation result`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is required"

        let result = Member.validateUsername String.empty

        test <@ result.IsInvalid @>
        test <@ result.ParamName = expectedParamName @>
        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling validate username, with username shorter than 3, returns expected validation result`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is shorter than 3 characters"

        let result = Member.validateUsername (String.random 2)

        test <@ result.IsInvalid @>
        test <@ result.ParamName = expectedParamName @>
        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling validate username, with username longer than 32, returns expected validation result`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is longer than 32 characters"

        let result = Member.validateUsername (String.random 33)

        test <@ result.IsInvalid @>
        test <@ result.ParamName = expectedParamName @>
        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling validate username, with valid username, returns no messages`` () =
        let result = Member.validateUsername (String.random 32)

        test <@ result.IsValid @>

    [<Test>]
    let ``Calling validate name, with null name, returns expected validation result`` () =
        let expectedParamName = "name"
        let expectedMessage = "Name is required"

        let result = Member.validateName null

        test <@ result.IsInvalid @>
        test <@ result.ParamName = expectedParamName @>
        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling validate name, with empty name, returns expected validation result`` () =
        let expectedParamName = "name"
        let expectedMessage = "Name is required"

        let result = Member.validateName String.empty

        test <@ result.IsInvalid @>
        test <@ result.ParamName = expectedParamName @>
        test <@ result.Message = expectedMessage @>

    [<Test>]
    let ``Calling validate name, with valid name, returns no messages`` () =
        let result = Member.validateName (String.random 64)

        test <@ result.IsValid @>
