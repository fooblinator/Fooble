namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open Fooble.Web.Controllers
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module MemberControllerTests =

    [<Test>]
    let ``Constructing, with null mediator, raises expected exception`` () =
        let expectedParamName = "mediator"
        let expectedMessage = "Mediator parameter was null"

        raisesWith<ArgumentException> <@ new MemberController(null) @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore <| new MemberController(mock ())

    [<Test>]
    let ``Calling detail, with matches in data store, returns expected result`` () =
        let matchingId = randomGuid ()
        let expectedQuery = MemberDetail.Query.make matchingId
        let expectedViewModel = MemberDetail.ReadModel.make <|| (matchingId, randomString ())

        let queryResult = MemberDetail.QueryResult.makeSuccess expectedViewModel
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(any ())).Returns(queryResult).Verifiable()

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.Detail(matchingId.ToString())

        mediatorMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? IMemberDetailReadModel @>

        let actualViewModel = viewResult.Model :?> IMemberDetailReadModel
        test <@ actualViewModel = expectedViewModel @>

    [<Test>]
    let ``Calling detail, with no matches in data store, returns expected result`` () =
        let nonMatchingId = randomGuid ()
        let expectedReadModel =
            MessageDisplay.ReadModel.make "Member" "Detail" 404 MessageDisplay.Severity.warning
                "No matching member could be found."

        let queryResult = MemberDetail.QueryResult.notFound
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(any ())).Returns(queryResult).Verifiable()

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.Detail(nonMatchingId.ToString())

        mediatorMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "MessageDisplay" @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        test <@ actualReadModel = expectedReadModel @>

    [<Test>]
    let ``Calling list, with matches in data store, returns expected result`` () =
        let expectedMembers = [ MemberList.ItemReadModel.make <|| (randomGuid (), randomString ()) ]

        let readModel = MemberList.ReadModel.make <| Seq.ofList expectedMembers
        let queryResult = MemberList.QueryResult.makeSuccess readModel
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(any ())).Returns(queryResult).Verifiable()

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.List()
 
        mediatorMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? IMemberListReadModel @>

        let actualMembers = Seq.toList (viewResult.Model :?> IMemberListReadModel).Members
        test <@ actualMembers = expectedMembers @>

    [<Test>]
    let ``Calling list, with no matches in data store, returns expected result`` () =
        let expectedReadModel =
            MessageDisplay.ReadModel.make "Member" "List" 200 MessageDisplay.Severity.informational
                "No members have yet been added."

        let queryResult = MemberList.QueryResult.notFound
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(any ())).Returns(queryResult).Verifiable()

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.List()

        mediatorMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "MessageDisplay" @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        test <@ actualReadModel = expectedReadModel @>
