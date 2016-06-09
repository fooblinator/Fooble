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
open System
open System.Web.Mvc

[<TestFixture>]
module MemberControllerDetailActionTests =

    [<Test>]
    let ``Calling detail, with matches in data store, returns expected result`` () =
        let matchingId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedEmail = EmailAddress.random 32
        let expectedNickname = String.random 64
        let expectedRegistered = DateTime.UtcNow
        let expectedPasswordChanged = DateTime.UtcNow

        let queryResult =
            makeTestMemberDetailReadModel matchingId expectedUsername expectedEmail expectedNickname DateTime.UtcNow
                DateTime.UtcNow
            |> MemberDetailQuery.makeSuccessResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)
        let result = controller.Detail(matchingId)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        String.isEmpty viewResult.ViewName =! true
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberDetailReadModel =! true

        let actualReadModel = viewResult.Model :?> IMemberDetailReadModel
        testMemberDetailReadModel actualReadModel matchingId expectedUsername expectedEmail expectedNickname
            expectedRegistered expectedPasswordChanged

    [<Test>]
    let ``Calling detail, with no matches in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedHeading = "Member"
        let expectedSubHeading = "Detail"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let queryResult = MemberDetailQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let keyGenerator = makeTestKeyGenerator None
        let controller = new MemberController(mediatorMock.Object, keyGenerator)
        let result = controller.Detail(nonMatchingId)

        mediatorMock.Verify()

        isNull result =! false
        result :? ViewResult =! true

        let viewResult = result :?> ViewResult

        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel expectedHeading expectedSubHeading expectedStatusCode
            expectedSeverity expectedMessage
