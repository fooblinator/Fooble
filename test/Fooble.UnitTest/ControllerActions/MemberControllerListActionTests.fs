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
module MemberControllerListActionTests =

    [<NoComparison>]
    type private Result =
        | Success of seq<(Guid * string * string)> * int
        | NotFound

    let private setupForGetActionTest result =
        let result =
            match result with
            | Success(members, memberCount) ->
                  members
                  |> Seq.map (fun (id, nickname, avatarData) -> MemberListReadModel.makeItem id nickname avatarData)
                  |> fun xs -> MemberListReadModel.make xs memberCount
                  |> MemberListQuery.makeSuccessResult
            | NotFound -> MemberListQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(It.IsAny<IMemberListQuery>())).Returns(result).End
        let controller = makeMemberController (Some(mediatorMock.Object)) None
        (controller, mediatorMock)

    [<Test>]
    let ``Calling list get action, with successful parameters, returns expected result`` () =
        let memberCount = 5
        let members = List.init memberCount (fun _ -> (Guid.NewGuid(), randomString 64, randomString 32))
        let (controller, mediatorMock) = setupForGetActionTest (Success(members, memberCount))
        let result = controller.List()
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberListQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberListReadModel =! true
        let readModel = viewResult.Model :?> IMemberListReadModel
        testMemberListReadModel readModel members memberCount

    [<Test>]
    let ``Calling list get action, with id not found in data store, returns expected result`` () =
        let heading = "Member"
        let subHeading = "List"
        let statusCode = 200
        let severity = MessageDisplayReadModel.informationalSeverity
        let message = "No members have yet been added."
        let (controller, mediatorMock) = setupForGetActionTest NotFound
        let result = controller.List()
        mediatorMock.VerifyFunc((fun x -> x.Send(It.IsAny<IMemberListQuery>())), Times.Once())
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message
