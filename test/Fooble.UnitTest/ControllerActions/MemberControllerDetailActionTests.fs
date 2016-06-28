namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module MemberControllerDetailActionTests =

    type private Result =
        | Success of Guid * string * string * string * string * DateTime * DateTime
        | NotFound

    let private setupForGetActionTest result =
        let result =
            match result with
            | Success(id, username, email, nickname, avatarData, registered, passwordChanged) ->
                  MemberDetailReadModel.make id username email nickname avatarData registered passwordChanged
                  |> MemberDetailQuery.makeSuccessResult
            | NotFound -> MemberDetailQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberDetailQuery>())).Returns(result).End
        let controller = makeMemberController (Some(mediatorMock.Object)) None
        (controller, mediatorMock)

    [<Test>]
    let ``Calling detail get action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let registered = DateTime.UtcNow
        let passwordChanged = DateTime.UtcNow
        let (controller, mediatorMock) =
            setupForGetActionTest (Success(id, username, email, nickname, avatarData, registered, passwordChanged))
        let result = controller.Detail(id)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberDetailQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberDetailReadModel =! true
        let readModel = viewResult.Model :?> IMemberDetailReadModel
        testMemberDetailReadModel readModel id username email nickname avatarData registered passwordChanged

    [<Test>]
    let ``Calling detail get action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let heading = "Member"
        let subHeading = "Detail"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, mediatorMock) = setupForGetActionTest NotFound
        let result = controller.Detail(notFoundId)
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberDetailQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message
