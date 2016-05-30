namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.UnitTest
open MediatR
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module SelfServiceRegisterCommandTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make Guid.empty (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with null username, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is required"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) null
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with empty username, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is required"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) String.empty
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with username shorter than 3 characters, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is shorter than 3 characters"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) (String.random 2)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with username longer than 32 characters, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is longer than 32 characters"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) (String.random 33)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with username in invalid format, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is not in the correct format (lowercase alphanumeric)"

        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) invalidFormatUsername
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with null email, raises expected exception`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is required"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32) null (String.random 64) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with empty email, raises expected exception`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is required"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32) String.empty (String.random 64) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with email longer than 254 characters, raises expected exception`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is longer than 254 characters"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32) (String.random 255)
                (String.random 64) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with email in invalid format, raises expected exception`` () =
        let expectedParamName = "email"
        let expectedMessage = "Email is not in the correct format"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32) (String.random 64)
                (String.random 64) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with null nickname, raises expected exception`` () =
        let expectedParamName = "nickname"
        let expectedMessage = "Nickname is required"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) null @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with empty nickname, raises expected exception`` () =
        let expectedParamName = "nickname"
        let expectedMessage = "Nickname is required"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) String.empty @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with nickname longer than 64 characters, raises expected exception`` () =
        let expectedParamName = "nickname"
        let expectedMessage = "Nickname is longer than 64 characters"

        raisesWith<ArgumentException>
            <@ SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 65) @>
            (fun x -> <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with valid parameters, returns command`` () =
        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64)

        test <@ box command :? ISelfServiceRegisterCommand @>
        test <@ box command :? IRequest<ISelfServiceRegisterCommandResult> @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let command =
            SelfServiceRegisterCommand.make expectedId (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64)

        test <@ command.Id = expectedId @>

    [<Test>]
    let ``Calling username, returns expected username`` () =
        let expectedUsername = String.random 32

        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) expectedUsername
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64)

        test <@ command.Username = expectedUsername @>

    [<Test>]
    let ``Calling email, returns expected email`` () =
        let expectedEmail = sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)

        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32) expectedEmail (String.random 64)

        test <@ command.Email = expectedEmail @>

    [<Test>]
    let ``Calling nickname, returns expected nickname`` () =
        let expectedNickname = String.random 64

        let command =
            SelfServiceRegisterCommand.make (Guid.random ()) (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) expectedNickname

        test <@ command.Nickname = expectedNickname @>
