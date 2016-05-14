namespace Fooble.Tests

open Fooble.Core
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
    let ``Constructing member controller, with null mediator, raises expected exception``() =
        let expectedParamName = "mediator"
        let expectedMessage = "Mediator should not be null"
        raisesWith<ArgumentException> <@ new MemberController(null) @>
            (fun e ->
            <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>)

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result``() =
        new MemberController(mock()) |> ignore

    [<Test>]
    let ``Calling member controller detail, with matches in data store, returns expected result``() =
        let matchingId = randomGuidString()
        let expectedQuery = MemberDetail.makeQuery matchingId
        let expectedViewModel = MemberDetail.makeReadModel matchingId (randomGuidString())
        let queryResult = Result.success expectedViewModel
        let mockMediator = Mock<IMediator>()
        mockMediator.SetupFunc(fun m -> m.Send(It.IsAny<IMemberDetailQuery>())).Returns(queryResult).End
        let controller = new MemberController(mockMediator.Object)
        let result = controller.Detail(matchingId)
        mockMediator.VerifyFunc((fun m -> m.Send(null)), Times.Never())
        mockMediator.VerifyFunc((fun m -> m.Send(expectedQuery)), Times.Once())
        test <@ not (isNull result) @>
        test <@ result :? ViewResult @>
        let viewResult = result :?> ViewResult
        test <@ viewResult.ViewName = "" @>
        test <@ not (isNull viewResult.Model) @>
        test <@ viewResult.Model :? IMemberDetailReadModel @>
        let actualViewModel = viewResult.Model :?> IMemberDetailReadModel
        test <@ actualViewModel = expectedViewModel @>

    [<Test>]
    let ``Calling member controller detail, with no matches in data store, returns expected result``() =
        let nonMatchingId = randomGuidString()
        let expectedQuery = MemberDetail.makeQuery nonMatchingId
        let queryResult = Result.failure MemberDetail.notFoundQueryFailureStatus
        let mockMediator = Mock<IMediator>()
        mockMediator.SetupFunc(fun m -> m.Send(It.IsAny<IMemberDetailQuery>())).Returns(queryResult).End
        let controller = new MemberController(mockMediator.Object)
        let result = controller.Detail(nonMatchingId)
        mockMediator.VerifyFunc((fun m -> m.Send(null)), Times.Never())
        mockMediator.VerifyFunc((fun m -> m.Send(expectedQuery)), Times.Once())
        test <@ not (isNull result) @>
        test <@ result :? ViewResult @>
        let viewResult = result :?> ViewResult
        test <@ viewResult.ViewName = "Detail_NotFound" @>
        test <@ isNull viewResult.Model @>

    [<Test>]
    let ``Calling member controller list, with matches in data store, returns expected result``() =
        let expectedMembers = [ MemberList.makeItemReadModel (randomGuidString()) (randomGuidString()) ]
        let queryResult = Result.success (MemberList.makeReadModel (Seq.ofList expectedMembers))
        let mockMediator = Mock<IMediator>()
        mockMediator.SetupFunc(fun m -> m.Send(It.IsAny<IMemberListQuery>())).Returns(queryResult).End
        let controller = new MemberController(mockMediator.Object)
        let result = controller.List()
        mockMediator.VerifyFunc((fun m -> m.Send(null)), Times.Never())
        mockMediator.VerifyFunc((fun m -> m.Send(It.IsAny<IMemberListQuery>())), Times.Once())
        test <@ not (isNull result) @>
        test <@ result :? ViewResult @>
        let viewResult = result :?> ViewResult
        test <@ viewResult.ViewName = "" @>
        test <@ not (isNull viewResult.Model) @>
        test <@ viewResult.Model :? IMemberListReadModel @>
        let actualMembers = Seq.toList (viewResult.Model :?> IMemberListReadModel).Members
        test <@ actualMembers = expectedMembers @>

    [<Test>]
    let ``Calling member controller list, with no matches in data store, returns expected result``() =
        let queryResult = Result.failure MemberList.notFoundQueryFailureStatus
        let mockMediator = Mock<IMediator>()
        mockMediator.SetupFunc(fun m -> m.Send(It.IsAny<IMemberListQuery>())).Returns(queryResult).End
        let controller = new MemberController(mockMediator.Object)
        let result = controller.List()
        mockMediator.VerifyFunc((fun m -> m.Send(null)), Times.Never())
        mockMediator.VerifyFunc((fun m -> m.Send(It.IsAny<IMemberListQuery>())), Times.Once())
        test <@ not (isNull result) @>
        test <@ result :? ViewResult @>
        let viewResult = result :?> ViewResult
        test <@ viewResult.ViewName = "List_NotFound" @>
        test <@ isNull viewResult.Model @>
