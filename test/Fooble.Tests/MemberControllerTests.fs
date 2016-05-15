﻿namespace Fooble.Tests

open Autofac
open Fooble.Core
open Fooble.Core.Infrastructure
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

        raisesWith<ArgumentException> <@ new MemberController(null) @> <|
            fun e -> <@ e.ParamName = expectedParamName && (fixArgumentExceptionMessage e.Message) = expectedMessage @>

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result``() =
        new MemberController(mock()) |> ignore

    [<Test>]
    let ``Calling member controller detail, with matches in data store, returns expected result``() =
        let matchingId = randomGuidString()
        let expectedQuery = MemberDetail.makeQuery matchingId
        let expectedViewModel = MemberDetail.makeReadModel matchingId (randomGuidString())

        let queryResult = successResult expectedViewModel
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(It.IsAny<IMemberDetailQuery>())).Returns(queryResult).End

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.Detail(matchingId)

        mediatorMock.VerifyFunc((fun m -> m.Send(null)), Times.Never())
        mediatorMock.VerifyFunc((fun m -> m.Send(expectedQuery)), Times.Once())

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
        let expectedHeading = "Member Detail Query"
        let expectedSeverity = MessageDisplay.errorSeverity
        let expectedMessages = [ "Member detail query was not successful and returned not found" ]

        let queryResult = failureResult MemberDetail.notFoundQueryFailureStatus
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(It.IsAny<IMemberDetailQuery>())).Returns(queryResult).End

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.Detail(nonMatchingId)

        mediatorMock.VerifyFunc((fun m -> m.Send(null)), Times.Never())
        mediatorMock.VerifyFunc((fun m -> m.Send(expectedQuery)), Times.Once())

        test <@ not (isNull result) @>
        test <@ result :? ViewResult @>
        let viewResult = result :?> ViewResult
        test <@ viewResult.ViewName = "Detail_NotFound" @>
        test <@ not (isNull viewResult.Model) @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>
        let actualViewModel = viewResult.Model :?> IMessageDisplayReadModel
        let actualHeading = actualViewModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = actualViewModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq actualViewModel.Messages
        test <@ actualMessages = expectedMessages @>

    [<Test>]
    let ``Calling member controller list, with matches in data store, returns expected result``() =
        let expectedMembers = [ MemberList.makeItemReadModel (randomGuidString()) (randomGuidString()) ]

        let queryResult = successResult (MemberList.makeReadModel (Seq.ofList expectedMembers))
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(It.IsAny<IMemberListQuery>())).Returns(queryResult).End

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.List()
 
        mediatorMock.VerifyFunc((fun m -> m.Send(null)), Times.Never())
        mediatorMock.VerifyFunc((fun m -> m.Send(It.IsAny<IMemberListQuery>())), Times.Once())

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
        let expectedHeading = "Member List Query"
        let expectedSeverity = MessageDisplay.errorSeverity
        let expectedMessages = [ "Member list query was not successful and returned not found" ]

        let queryResult = failureResult MemberList.notFoundQueryFailureStatus
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(It.IsAny<IMemberListQuery>())).Returns(queryResult).End

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.List()

        mediatorMock.VerifyFunc((fun m -> m.Send(null)), Times.Never())
        mediatorMock.VerifyFunc((fun m -> m.Send(It.IsAny<IMemberListQuery>())), Times.Once())

        test <@ not (isNull result) @>
        test <@ result :? ViewResult @>
        let viewResult = result :?> ViewResult
        test <@ viewResult.ViewName = "List_NotFound" @>
        test <@ not (isNull viewResult.Model) @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>
        let actualViewModel = viewResult.Model :?> IMessageDisplayReadModel
        let actualHeading = actualViewModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = actualViewModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq actualViewModel.Messages
        test <@ actualMessages = expectedMessages @>
