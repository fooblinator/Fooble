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
module MemberChangeUsernameCommandTests =

    type private CommandResult = Success | NotFound | IncorrectPassword | Unchanged | UnavailableUsername

    let private setupGetMember (contextMock:Mock<IFoobleContext>) id currentPassword username =
        currentPassword
        |> Option.map (fun x -> Crypto.hash x 100)
        |> fun x -> makeMemberData (Some(id)) username x None None None None None None
        |> fun x -> contextMock.SetupFunc(fun y -> y.GetMember(id, false)).Returns(Some(x)).End

    let private setupForHandlerTest result id currentPassword username =
        let contextMock = Mock<IFoobleContext>()
        match result with
        | Success ->
              setupGetMember contextMock id (Some(currentPassword)) None
              contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(username, true)).Returns(false).End
              contextMock.SetupAction(fun x -> x.SaveChanges()).End
        | NotFound -> contextMock.SetupFunc(fun x -> x.GetMember(id, false)).Returns(None).End
        | IncorrectPassword -> setupGetMember contextMock id None None
        | Unchanged -> setupGetMember contextMock id (Some(currentPassword)) (Some(username))
        | UnavailableUsername ->
              setupGetMember contextMock id (Some(currentPassword)) None
              contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(username, true)).Returns(true).End
        let command = MemberChangeUsernameCommand.make id currentPassword username
        let handler = MemberChangeUsernameCommand.makeHandler contextMock.Object
        (handler, command, contextMock)

    [<Test>]
    let ``Calling make, with successful parameters, returns expected command`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let username = randomString 32
        let command = MemberChangeUsernameCommand.make id currentPassword username
        box command :? IRequest<IMemberChangeUsernameCommandResult> =! true
        testMemberChangeUsernameCommand command id currentPassword username

    [<Test>]
    let ``Calling success result, returns expected state`` () =
        let commandResult = MemberChangeUsernameCommand.successResult
        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling not found result, returns expected state`` () =
        let commandResult = MemberChangeUsernameCommand.notFoundResult
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! true
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling incorrect password result, returns expected state`` () =
        let commandResult = MemberChangeUsernameCommand.incorrectPasswordResult
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! true
        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling unavailable username result, returns expected state`` () =
        let commandResult = MemberChangeUsernameCommand.unavailableUsernameResult
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableUsername =! true

    [<Test>]
    let ``Calling extension add model error, as success result, does not modify model state`` () =
        let modelState = ModelStateDictionary()
        MemberChangeUsernameCommand.successResult.AddModelErrors(modelState)
        modelState.IsValid =! true

    [<Test>]
    let ``Calling extension add model error, as incorrect password result, modifies model state appropriately`` () =
        let modelState = ModelStateDictionary()
        MemberChangeUsernameCommand.incorrectPasswordResult.AddModelErrors(modelState)
        testModelState modelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling extension add model error, as unavailable username result, modifies model state appropriately`` () =
        let modelState = ModelStateDictionary()
        MemberChangeUsernameCommand.unavailableUsernameResult.AddModelErrors(modelState)
        testModelState modelState "username" "Username is unavailable"

    [<Test>]
    let ``Calling extension to message display read model, as success result, raises expected exception`` () =
        let commandResult = MemberChangeUsernameCommand.successResult
        testInvalidOperationException "Result was not unsuccessful" <@ commandResult.MapMessageDisplayReadModel() @>

    [<Test>]
    let ``Calling extension to message display read model, as not found result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let readModel =
            MemberChangeUsernameCommand.notFoundResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling extension to message display read model, as incorrect password result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 400
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "Supplied password is incorrect."
        let readModel =
            MemberChangeUsernameCommand.incorrectPasswordResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling extension to message display read model, as unavailable username result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 400
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "Requested username is unavailable."
        let readModel =
            MemberChangeUsernameCommand.unavailableUsernameResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling handler handle, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let username = randomString 32
        let (handler, command, contextMock) = setupForHandlerTest Success id currentPassword username
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, false)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberUsername(username, true)), Times.Once())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Once())
        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling handler handle, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let username = randomString 32
        let (handler, command, contextMock) = setupForHandlerTest NotFound notFoundId currentPassword username
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(notFoundId, false)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberUsername(username, true)), Times.Never())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! true
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling handler handle, with incorrect password, returns expected result`` () =
        let id = Guid.NewGuid()
        let incorrectPassword = randomPassword 32
        let username = randomString 32
        let (handler, command, contextMock) = setupForHandlerTest IncorrectPassword id incorrectPassword username
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, false)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberUsername(username, true)), Times.Never())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! true
        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling handler handle, with no change from current username, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let unchangedUsername = randomString 32
        let (handler, command, contextMock) = setupForHandlerTest Unchanged id currentPassword unchangedUsername
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, false)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberUsername(unchangedUsername, true)), Times.Never())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableUsername =! false

    [<Test>]
    let ``Calling handler handle, with unavailable username, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let unavailableUsername = randomString 32
        let (handler, command, contextMock) =
            setupForHandlerTest UnavailableUsername id currentPassword unavailableUsername
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, false)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberUsername(unavailableUsername, true)), Times.Once())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! false
        commandResult.IsIncorrectPassword =! false
        commandResult.IsUnavailableUsername =! true
