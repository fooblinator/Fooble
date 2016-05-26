namespace Fooble.UnitTest.SelfServiceRegister

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
            <@ SelfServiceRegister.Command.make Guid.empty (String.random 32) (String.random 64) @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with null username, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is required"

        raisesWith<ArgumentException>
            <@ SelfServiceRegister.Command.make (Guid.random ()) null (String.random 64) @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with empty username, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is required"

        raisesWith<ArgumentException>
            <@ SelfServiceRegister.Command.make (Guid.random ()) String.empty (String.random 64) @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with username shorter than 3 characters, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is shorter than 3 characters"

        raisesWith<ArgumentException>
            <@ SelfServiceRegister.Command.make (Guid.random ()) (String.random 2) (String.random 64) @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with username longer than 32 characters, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is longer than 32 characters"

        raisesWith<ArgumentException>
            <@ SelfServiceRegister.Command.make (Guid.random ()) (String.random 33) (String.random 64) @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with username in invalid format, raises expected exception`` () =
        let expectedParamName = "username"
        let expectedMessage = "Username is not in the correct format (lowercase alphanumeric)"

        let invalidFormatUsername = sprintf "-%s-%s-" (String.random 8) (String.random 8)
        raisesWith<ArgumentException>
            <@ SelfServiceRegister.Command.make (Guid.random ()) invalidFormatUsername (String.random 64) @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with null name, raises expected exception`` () =
        let expectedParamName = "name"
        let expectedMessage = "Name is required"

        raisesWith<ArgumentException>
            <@ SelfServiceRegister.Command.make (Guid.random ()) (String.random 32) null @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with empty name, raises expected exception`` () =
        let expectedParamName = "name"
        let expectedMessage = "Name is required"

        raisesWith<ArgumentException>
            <@ SelfServiceRegister.Command.make (Guid.random ()) (String.random 32) String.empty @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with valid parameters, returns command`` () =
        let command = SelfServiceRegister.Command.make (Guid.random ()) (String.random 32) (String.random 64)

        test <@ box command :? ISelfServiceRegisterCommand @>
        test <@ box command :? IRequest<ISelfServiceRegisterCommandResult> @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let command = SelfServiceRegister.Command.make expectedId (String.random 32) (String.random 64)

        test <@ command.Id = expectedId @>

    [<Test>]
    let ``Calling username, returns expected username`` () =
        let expectedUsername = String.random 32

        let command = SelfServiceRegister.Command.make (Guid.random ()) expectedUsername (String.random 64)

        test <@ command.Username = expectedUsername @>

    [<Test>]
    let ``Calling name, returns expected name`` () =
        let expectedName = String.random 64

        let command = SelfServiceRegister.Command.make (Guid.random ()) (String.random 32) expectedName

        test <@ command.Name = expectedName @>
