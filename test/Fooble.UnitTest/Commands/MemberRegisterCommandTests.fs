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
module MemberRegisterCommandTests =

    type private CommandResult = Success | UnavailableUsername | UnavailableEmail

    let private makeMemberDataFactory () =
        MemberDataFactory(
            fun id username passwordData email nickname avatarData registeredOn passwordChangedOn deactivatedOn ->
                makeMemberData (Some(id)) (Some(username)) (Some(passwordData)) (Some(email)) (Some(nickname))
                    (Some(avatarData)) (Some(registeredOn)) (Some(passwordChangedOn)) deactivatedOn)

    let private setupForHandlerTest result id username password email nickname avatarData =
        let contextMock = Mock<IFoobleContext>()
        match result with
        | Success ->
              contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(username, true)).Returns(false).End
              contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(email, true)).Returns(false).End
              contextMock.SetupAction(fun x -> x.AddMember(It.IsAny<IMemberData>())).End
              contextMock.SetupAction(fun x -> x.SaveChanges()).End
        | UnavailableUsername ->
              contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(username, true)).Returns(true).End
              contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(email, true)).Returns(false).End
        | UnavailableEmail ->
              contextMock.SetupFunc(fun x -> x.ExistsMemberUsername(username, true)).Returns(false).End
              contextMock.SetupFunc(fun x -> x.ExistsMemberEmail(email, true)).Returns(true).End
        let command = MemberRegisterCommand.make id username password email nickname avatarData
        let handler = MemberRegisterCommand.makeHandler contextMock.Object (makeMemberDataFactory ())
        (handler, command, contextMock)

    [<Test>]
    let ``Calling make, with successful parameters, returns expected command`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let password = randomPassword 32
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let command = MemberRegisterCommand.make id username password email nickname avatarData
        box command :? IRequest<IMemberRegisterCommandResult> =! true
        testMemberRegisterCommand command id username password email nickname avatarData

    [<Test>]
    let ``Calling success result, returns expected state`` () =
        let commandResult = MemberRegisterCommand.successResult
        commandResult.IsSuccess =! true
        commandResult.IsUnavailableUsername =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling unavailable username result, returns expected state`` () =
        let commandResult = MemberRegisterCommand.unavailableUsernameResult
        commandResult.IsSuccess =! false
        commandResult.IsUnavailableUsername =! true
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling unavailable email result, returns expected state`` () =
        let commandResult = MemberRegisterCommand.unavailableEmailResult
        commandResult.IsSuccess =! false
        commandResult.IsUnavailableUsername =! false
        commandResult.IsUnavailableEmail =! true

    [<Test>]
    let ``Calling extension add model error, as success result, does not modify model state`` () =
        let modelState = ModelStateDictionary()
        MemberRegisterCommand.successResult.AddModelErrors(modelState)
        modelState.IsValid =! true

    [<Test>]
    let ``Calling extension add model error, as unavailable username result, modifies model state appropriately`` () =
        let modelState = ModelStateDictionary()
        MemberRegisterCommand.unavailableUsernameResult.AddModelErrors(modelState)
        testModelState modelState "username" "Username is unavailable"

    [<Test>]
    let ``Calling extension add model error, as unavailable email result, modifies model state appropriately`` () =
        let modelState = ModelStateDictionary()
        MemberRegisterCommand.unavailableEmailResult.AddModelErrors(modelState)
        testModelState modelState "email" "Email is already registered"

    [<Test>]
    let ``Calling extension to message display read model, as success result, raises expected exception`` () =
        let commandResult = MemberRegisterCommand.successResult
        testInvalidOperationException "Result was not unsuccessful" <@ commandResult.MapMessageDisplayReadModel() @>

    [<Test>]
    let ``Calling extension to message display read model, as unavailable username result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Register"
        let statusCode = 400
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "Requested username is unavailable."
        let readModel =
            MemberRegisterCommand.unavailableUsernameResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling extension to message display read model, as unavailable email result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Register"
        let statusCode = 400
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "Supplied email is already registered."
        let readModel =
            MemberRegisterCommand.unavailableEmailResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling handler handle, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let password = randomPassword 32
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (handler, command, contextMock) =
            setupForHandlerTest Success id username password email nickname avatarData
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.ExistsMemberUsername(username, includeDeactivated = true)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberEmail(email, includeDeactivated = true)), Times.Once())
        contextMock.VerifyAction((fun x -> x.AddMember(It.IsAny<IMemberData>())), Times.Once())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Once())
        commandResult.IsSuccess =! true
        commandResult.IsUnavailableUsername =! false
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handler handle, with unavailable username, returns expected result`` () =
        let id = Guid.NewGuid()
        let unavailableUsername = randomString 32
        let password = randomPassword 32
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (handler, command, contextMock) =
            setupForHandlerTest UnavailableUsername id unavailableUsername password email nickname avatarData
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x ->
            x.ExistsMemberUsername(unavailableUsername, includeDeactivated = true)), Times.Once())
        contextMock.VerifyFunc((fun x -> x.ExistsMemberEmail(email, includeDeactivated = true)), Times.Once())
        contextMock.VerifyAction((fun x -> x.AddMember(It.IsAny<IMemberData>())), Times.Never())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! false
        commandResult.IsUnavailableUsername =! true
        commandResult.IsUnavailableEmail =! false

    [<Test>]
    let ``Calling handler handle, with unavailable email, returns expected result`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let password = randomPassword 32
        let unavailableEmail = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let (handler, command, contextMock) =
            setupForHandlerTest UnavailableEmail id username password unavailableEmail nickname avatarData
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.ExistsMemberUsername(username, includeDeactivated = true)), Times.Once())
        contextMock.VerifyFunc((fun x ->
            x.ExistsMemberEmail(unavailableEmail, includeDeactivated = true)), Times.Once())
        contextMock.VerifyAction((fun x -> x.AddMember(It.IsAny<IMemberData>())), Times.Never())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! false
        commandResult.IsUnavailableUsername =! false
        commandResult.IsUnavailableEmail =! true
