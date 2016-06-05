namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberTests =

    [<Test>]
    let ``Calling validate id, with empty id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        let result = Member.validateId Guid.empty

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate id, with valid id, returns no messages`` () =
        let result = Member.validateId (Guid.random ())

        result.IsValid =! true

    [<Test>]
    let ``Calling validate id string, with null id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        let result = Member.validateIdString null

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate id string, with empty id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        let result = Member.validateIdString String.empty

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate id string, with invalid format id, returns expected validation result`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is not in the correct format (GUID)"

        let result = Member.validateIdString (String.random 64)

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate id string, with valid id, returns no messages`` () =
        let result = Member.validateIdString (Guid.random () |> String.ofGuid)

        result.IsValid =! true

    [<Test>]
    let ``Calling validate username, with null username, returns expected validation result`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is required"

        let result = Member.validateUsername null

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate username, with empty username, returns expected validation result`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is required"

        let result = Member.validateUsername String.empty

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate username, with username shorter than 3, returns expected validation result`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is shorter than 3 characters"

        let result = Member.validateUsername (String.random 2)

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate username, with username longer than 32, returns expected validation result`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is longer than 32 characters"

        let result = Member.validateUsername (String.random 33)

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate username, with username in invalid format, returns expected validation result`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is not in the correct format (lowercase alphanumeric)"

        let result = Member.validateUsername (sprintf "-%s-%s-" (String.random 8) (String.random 8))

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate username, with valid username, returns no messages`` () =
        let result = Member.validateUsername (String.random 32)

        result.IsValid =! true

    [<Test>]
    let ``Calling validate email, with null email, returns expected validation result`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is required"

        let result = Member.validateEmail null

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate email, with empty email, returns expected validation result`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is required"

        let result = Member.validateEmail String.empty

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate email, with email longer than 254, returns expected validation result`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is longer than 254 characters"

        let result = Member.validateEmail (String.random 255)

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate email, with email in invalid format, returns expected validation result`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is not in the correct format"

        let result = Member.validateEmail (String.random 64)

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate email, with valid email, returns no messages`` () =
        let result = Member.validateEmail (EmailAddress.random 32)

        result.IsValid =! true

    [<Test>]
    let ``Calling validate nickname, with null nickname, returns expected validation result`` () =
        let expectedParamName = "nickname"
        let expectedMessage = "Nickname is required"

        let result = Member.validateNickname null

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate nickname, with empty nickname, returns expected validation result`` () =
        let expectedParamName = "nickname"
        let expectedMessage = "Nickname is required"

        let result = Member.validateNickname String.empty

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate nickname, with nickname longer than 64, returns expected validation result`` () =
        let expectedParamName = "nickname"
        let expectedMessage = "Nickname is longer than 64 characters"

        let result = Member.validateNickname (String.random 65)

        result.IsInvalid =! true
        result.ParamName =! expectedParamName
        result.Message =! expectedMessage

    [<Test>]
    let ``Calling validate nickname, with valid nickname, returns no messages`` () =
        let result = Member.validateNickname (String.random 64)

        result.IsValid =! true
