namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open Fooble.Web.Controllers
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module MemberControllerListActionTests =

    [<Test>]
    let ``Calling list, with no members in data store, returns expected result`` () =
        let heading = "Member"
        let subHeading = "List"
        let statusCode = 200
        let severity = MessageDisplayReadModel.informationalSeverity
        let message = "No members have yet been added."

        let queryResult = MemberListQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)
        let result = controller.List()

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling list, with members in data store, returns expected result`` () =
        let memberCount = 5
        let members = List.init memberCount (fun _ ->
            makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))

        let queryResult =
            makeTestMemberListReadModel (Seq.ofList members) memberCount
            |> MemberListQuery.makeSuccessResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)
        let result = controller.List()

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberListReadModel =! true

        let actualReadModel = viewResult.Model :?> IMemberListReadModel
        testMemberListReadModel2 actualReadModel members memberCount
