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

[<TestFixture>]
module MemberChangeOtherCommandTests =

    type private CommandResult = Success | NotFound | Unchanged

    let private setupGetMember (contextMock:Mock<IFoobleContext>) id nickname avatarData =
        makeMemberData (Some(id)) None None None nickname avatarData None None None
        |> fun x -> contextMock.SetupFunc(fun y -> y.GetMember(id, false)).Returns(Some(x)).End

    let private setupForHandlerTest result id nickname avatarData =
        let contextMock = Mock<IFoobleContext>()
        match result with
        | Success ->
              setupGetMember contextMock id None None
              contextMock.SetupAction(fun x -> x.SaveChanges()).End
        | NotFound -> contextMock.SetupFunc(fun x -> x.GetMember(id, false)).Returns(None).End
        | Unchanged -> setupGetMember contextMock id (Some(nickname)) (Some(avatarData))
        let command = MemberChangeOtherCommand.make id nickname avatarData
        let handler = MemberChangeOtherCommand.makeHandler contextMock.Object
        (handler, command, contextMock)

    [<Test>]
    let ``Calling make, with successful parameters, returns expected command`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 64
        let avatarData = randomString 32
        let command = MemberChangeOtherCommand.make id nickname avatarData
        box command :? IRequest<IMemberChangeOtherCommandResult> =! true
        testMemberChangeOtherCommand command id nickname avatarData

    [<Test>]
    let ``Calling success result, returns expected state`` () =
        let commandResult = MemberChangeEmailCommand.successResult
        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling not found result, returns expected state`` () =
        let commandResult = MemberChangeEmailCommand.notFoundResult
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! true

    [<Test>]
    let ``Calling extension to message display read model, as success result, raises expected exception`` () =
        let commandResult = MemberChangeOtherCommand.successResult
        testInvalidOperationException "Result was not unsuccessful" <@ commandResult.MapMessageDisplayReadModel() @>

    [<Test>]
    let ``Calling extension to message display read model, as not found result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Change Other"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let readModel =
            MemberChangeOtherCommand.notFoundResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling handler handle, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 64
        let avatarData = randomString 32
        let (handler, command, contextMock) = setupForHandlerTest Success id nickname avatarData
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, includeDeactivated = false)), Times.Once())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Once())
        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false

    [<Test>]
    let ``Calling handler handle, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let nickname = randomString 64
        let avatarData = randomString 32
        let (handler, command, contextMock) = setupForHandlerTest NotFound notFoundId nickname avatarData
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(notFoundId, includeDeactivated = false)), Times.Once())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! false
        commandResult.IsNotFound =! true

    [<Test>]
    let ``Calling handler handle, with no change from current nickname or avatar data, returns expected result`` () =
        let id = Guid.NewGuid()
        let unchangedNickname = randomString 64
        let unchangedAvatarData = randomString 32
        let (handler, command, contextMock) = setupForHandlerTest Unchanged id unchangedNickname unchangedAvatarData
        let commandResult = handler.Handle(command)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, includeDeactivated = false)), Times.Once())
        contextMock.VerifyAction((fun x -> x.SaveChanges()), Times.Never())
        commandResult.IsSuccess =! true
        commandResult.IsNotFound =! false
