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
module MemberDeactivateCommandTests =

    type private CommandResult = Success | NotFound | IncorrectPassword

    let private setupGetMember (contextMock:Mock<IFoobleContext>) id currentPassword =
        currentPassword
        |> Option.map (fun x -> Crypto.hash x 100)
        |> fun x -> makeMemberData (Some(id)) None x None None None None None None
        |> fun x -> contextMock.SetupFunc(fun y -> y.GetMember(id, false)).Returns(Some(x)).End

    let private setupForHandlerTest result id currentPassword =
        let contextMock = Mock<IFoobleContext>()
        match result with
        | Success ->
              setupGetMember contextMock id (Some(currentPassword))
              contextMock.SetupAction(fun x -> x.SaveChanges()).End
        | NotFound -> contextMock.SetupFunc(fun x -> x.GetMember(id, false)).Returns(None).End
        | IncorrectPassword -> setupGetMember contextMock id None
        let command = MemberDeactivateCommand.make id currentPassword
        let handler = MemberDeactivateCommand.makeHandler contextMock.Object
        (handler, command, contextMock)

    [<Test>]
    let ``Calling make, with successful parameters, returns expected command`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let command = MemberDeactivateCommand.make id currentPassword
        box command :? IRequest<IMemberDeactivateCommandResult> =! true
        testMemberDeactivateCommand command id currentPassword

    [<Test>]
    let ``Calling success result, returns expected state`` () =
        let commandResult = MemberDeactivateCommand.successResult
        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling not found result, returns expected state`` () =
        let commandResult = MemberDeactivateCommand.notFoundResult
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! true
        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling incorrect password result, returns expected state`` () =
        let commandResult = MemberDeactivateCommand.incorrectPasswordResult
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! true

    [<Test>]
    let ``Calling extension add model error, as success result, does not modify model state`` () =
        let modelState = ModelStateDictionary()
        MemberDeactivateCommand.successResult.AddModelErrors(modelState)
        modelState.IsValid =! true

    [<Test>]
    let ``Calling extension add model error, as incorrect password result, modifies model state appropriately`` () =
        let modelState = ModelStateDictionary()
        MemberDeactivateCommand.incorrectPasswordResult.AddModelErrors(modelState)
        testModelState modelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling extension to message display read model, as success result, raises expected exception`` () =
        let commandResult = MemberDeactivateCommand.successResult
        testInvalidOperationException "Result was not unsuccessful" <@ commandResult.MapMessageDisplayReadModel() @>

    [<Test>]
    let ``Calling extension to message display read model, as not found result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Deactivate"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let readModel =
            MemberDeactivateCommand.notFoundResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling extension to message display read model, as incorrect password result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Deactivate"
        let statusCode = 400
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "Supplied password is incorrect."
        let readModel =
            MemberDeactivateCommand.incorrectPasswordResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling handler handle, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let (handler, command, contextMock) = setupForHandlerTest Success id currentPassword
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, false)), Times.Once())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Once())
        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling handler handle, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let (handler, command, contextMock) = setupForHandlerTest NotFound notFoundId currentPassword
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(notFoundId, false)), Times.Once())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! true
        commandResult.IsIncorrectPassword =! false

    [<Test>]
    let ``Calling handler handle, with incorrect password, returns expected result`` () =
        let id = Guid.NewGuid()
        let incorrectPassword = randomPassword 32
        let (handler, command, contextMock) = setupForHandlerTest IncorrectPassword id incorrectPassword
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, false)), Times.Once())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! true
