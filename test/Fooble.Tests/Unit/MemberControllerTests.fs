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
        let expectedMessage = "Mediator should not be null"

        raisesWith<ArgumentException> <@ new MemberController(null) @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixInvalidArgMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore <| new MemberController(mock ())

    [<Test>]
    let ``Calling detail, with matches in data store, returns expected result`` () =
        let matchingId = randomGuid ()
        let expectedQuery = MemberDetail.makeQuery matchingId
        let expectedViewModel = MemberDetail.makeReadModel <|| (matchingId, randomString ())

        let queryResult = MemberDetail.makeSuccessResult expectedViewModel
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
        let expectedQuery = MemberDetail.makeQuery nonMatchingId
        let expectedHeading = "Member Detail Query"
        let expectedSeverity = MessageDisplay.errorSeverity
        let expectedMessages = [ "Member detail query was not successful and returned not found" ]

        let queryResult = MemberDetail.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(any ())).Returns(queryResult).Verifiable()

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.Detail(nonMatchingId.ToString())

        mediatorMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "Detail_NotFound" @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualViewModel = viewResult.Model :?> IMessageDisplayReadModel

        let actualHeading = actualViewModel.Heading
        test <@ actualHeading = expectedHeading @>

        let actualSeverity = actualViewModel.Severity
        test <@ actualSeverity = expectedSeverity @>

        let actualMessages = List.ofSeq actualViewModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling list, with matches in data store, returns expected result`` () =
        let expectedMembers = [ MemberList.makeItemReadModel <|| (randomGuid (), randomString ()) ]

        let readModel = MemberList.makeReadModel <| Seq.ofList expectedMembers
        let queryResult = MemberList.makeSuccessResult readModel
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
        let expectedHeading = "Member List Query"
        let expectedSeverity = MessageDisplay.errorSeverity
        let expectedMessages = [ "Member list query was not successful and returned not found" ]

        let queryResult = MemberList.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(any ())).Returns(queryResult).Verifiable()

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.List()

        mediatorMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "List_NotFound" @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualViewModel = viewResult.Model :?> IMessageDisplayReadModel

        let actualHeading = actualViewModel.Heading
        test <@ actualHeading = expectedHeading @>

        let actualSeverity = actualViewModel.Severity
        test <@ actualSeverity = expectedSeverity @>

        let actualMessages = List.ofSeq actualViewModel.Messages
        test <@ actualMessages = expectedMessages @>
