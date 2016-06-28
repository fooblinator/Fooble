namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Fooble.Presentation
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module MemberChangeEmailCommandTests =

    type private CommandResult = Success | NotFound | IncorrectPassword | Unchanged | UnavailableEmail

    let private setupGetMember (contextMock:Mock<IFoobleContext>) id currentPassword email =
        currentPassword
        |> Option.map (fun x -> Crypto.hash x 100)
        |> fun x -> makeMemberData (Some(id)) None x email None None None None None
        |> fun x -> contextMock.SetupFunc(fun y -> y.GetMember(id, false)).Returns(Some(x)).End

    let private setupForHandlerTest result id currentPassword email =
        let contextMock = Mock<IFoobleContext>()
        match result with
        | Success ->
              setupGetMember contextMock id (Some(currentPassword)) None
              contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(email, true)).Returns(false).End
              contextMock.SetupAction(fun x -> x.SaveChanges()).End
        | NotFound -> contextMock.SetupFunc(fun x -> x.GetMember(id, false)).Returns(None).End
        | IncorrectPassword -> setupGetMember contextMock id None None
        | Unchanged -> setupGetMember contextMock id (Some(currentPassword)) (Some(email))
        | UnavailableEmail ->
              setupGetMember contextMock id (Some(currentPassword)) None
              contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(email, true)).Returns(true).End
        let command = MemberChangeEmailCommand.make id currentPassword email
        let handler = MemberChangeEmailCommand.makeHandler contextMock.Object
        (handler, command, contextMock)

    [<Test>]
    let ``Calling make, with successful parameters, returns expected command`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let email = randomEmail 32
        let command = MemberChangeEmailCommand.make id currentPassword email
        box command :? IRequest<IMemberChangeEmailCommandResult> =! true
        testMemberChangeEmailCommand command id currentPassword email

    [<Test>]
    let ``Calling success result, returns expected state`` () =
        let commandResult = MemberChangeEmailCommand.successResult
        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling not found result, returns expected state`` () =
        let commandResult = MemberChangeEmailCommand.notFoundResult
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! true
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling incorrect password result, returns expected state`` () =
        let commandResult = MemberChangeEmailCommand.incorrectPasswordResult
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! true
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling unavailable email result, returns expected state`` () =
        let commandResult = MemberChangeEmailCommand.unavailableEmailResult
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableEmail =! true

    [<Test>]
    let ``Calling extension add model error, as success result, does not modify model state`` () =
        let modelState = ModelStateDictionary()
        MemberChangeEmailCommand.successResult.AddModelErrors(modelState)
        modelState.IsValid =! true

    [<Test>]
    let ``Calling extension add model error, as incorrect password result, modifies model state appropriately`` () =
        let modelState = ModelStateDictionary()
        MemberChangeEmailCommand.incorrectPasswordResult.AddModelErrors(modelState)
        testModelState modelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling extension add model error, as unavailable email result, modifies model state appropriately`` () =
        let modelState = ModelStateDictionary()
        MemberChangeEmailCommand.unavailableEmailResult.AddModelErrors(modelState)
        testModelState modelState "email" "Email is already registered"

    [<Test>]
    let ``Calling extension to message display read model, as success result, raises expected exception`` () =
        let commandResult = MemberChangeEmailCommand.successResult
        testInvalidOperationException "Result was not unsuccessful" <@ commandResult.MapMessageDisplayReadModel() @>

    [<Test>]
    let ``Calling extension to message display read model, as not found result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Change Email"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let readModel =
            MemberChangeEmailCommand.notFoundResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling extension to message display read model, as incorrect password result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Change Email"
        let statusCode = 400
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "Supplied password is incorrect."
        let readModel =
            MemberChangeEmailCommand.incorrectPasswordResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling extension to message display read model, as unavailable email result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Change Email"
        let statusCode = 400
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "Supplied email is already registered."
        let readModel =
            MemberChangeEmailCommand.unavailableEmailResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling handler handle, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let email = randomEmail 32
        let (handler, command, contextMock) = setupForHandlerTest Success id currentPassword email
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, false)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberEmail(email, true)), Times.Once())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Once())
        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handler handle, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let email = randomEmail 32
        let (handler, command, contextMock) = setupForHandlerTest NotFound notFoundId currentPassword email
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(notFoundId, false)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberEmail(email, true)), Times.Never())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! true
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handler handle, with incorrect password, returns expected result`` () =
        let id = Guid.NewGuid()
        let incorrectPassword = randomPassword 32
        let email = randomEmail 32
        let (handler, command, contextMock) = setupForHandlerTest IncorrectPassword id incorrectPassword email
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, false)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberEmail(email, true)), Times.Never())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! true
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handler handle, with no change from current email, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let unchangedEmail = randomEmail 32
        let (handler, command, contextMock) = setupForHandlerTest Unchanged id currentPassword unchangedEmail
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, false)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberEmail(unchangedEmail, true)), Times.Never())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handler handle, with unavailable email, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let unavailableEmail = randomEmail 32
        let (handler, command, contextMock) = setupForHandlerTest UnavailableEmail id currentPassword unavailableEmail
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, false)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberEmail(unavailableEmail, true)), Times.Once())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableEmail =! true
