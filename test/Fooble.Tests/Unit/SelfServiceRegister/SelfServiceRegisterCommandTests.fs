namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open MediatR
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module SelfServiceRegisterCommandTests =
    
    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was an empty GUID"
        
        raisesWith<ArgumentException>
            <@ SelfServiceRegister.Command.make Guid.empty (randomString ()) @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with null name, raises expected exception`` () =
        let expectedParamName = "name"
        let expectedMessage = "Name parameter was null"
        
        raisesWith<ArgumentException>
            <@ SelfServiceRegister.Command.make (randomGuid ()) null @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>
    
    [<Test>]
    let ``Calling make, with empty name, raises expected exception`` () =
        let expectedParamName = "name"
        let expectedMessage = "Name parameter was an empty string"
        
        raisesWith<ArgumentException>
            <@ SelfServiceRegister.Command.make (randomGuid ()) String.empty @> <|
                fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Calling make, with valid parameters, returns command`` () =
        let command = SelfServiceRegister.Command.make (randomGuid ()) (randomString ())

        test <@ box command :? ISelfServiceRegisterCommand @>
        test <@ box command :? IRequest<Unit> @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = randomGuid ()

        let command = SelfServiceRegister.Command.make expectedId (randomString ())

        let actualId = command.Id
        test <@ actualId = expectedId @>

    [<Test>]
    let ``Calling name, returns expected name`` () =
        let expectedName = randomString ()

        let command = SelfServiceRegister.Command.make (randomGuid ()) expectedName

        let actualName = command.Name
        test <@ actualName = expectedName @>
