namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDeactivateCommandTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        testArgumentException expectedParamName expectedMessage
            <@ MemberDeactivateCommand.make Guid.empty (Password.random 32) @>

    [<Test>]
    let ``Calling make, with valid parameters, returns command`` () =
        let command = MemberDeactivateCommand.make (Guid.random ()) (Password.random 32)

        box command :? IRequest<IMemberDeactivateCommandResult> =! true

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let command = MemberDeactivateCommand.make expectedId (Password.random 32)

        command.Id =! expectedId

    [<Test>]
    let ``Calling current password, returns expected current password`` () =
        let expectedCurrentPassword = EmailAddress.random 32

        let command =
            MemberDeactivateCommand.make (Guid.random ()) expectedCurrentPassword

        command.CurrentPassword =! expectedCurrentPassword
