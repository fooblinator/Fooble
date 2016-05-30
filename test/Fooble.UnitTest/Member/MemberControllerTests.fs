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
module MemberControllerTests =

    [<Test>]
    let ``Constructing, with null mediator, raises expected exception`` () =
        let expectedParamName = "mediator"
        let expectedMessage = "Mediator is required"

        raisesWith<ArgumentException> <@ new MemberController(null) @> (fun x ->
            <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore <| new MemberController(mock ())

    [<Test>]
    let ``Calling detail, with matches in data store, returns expected result`` () =
        let matchingId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedEmail = sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)
        let expectedNickname = String.random 64

        let queryResult =
            makeMemberDetailReadModel matchingId expectedUsername expectedEmail expectedNickname
            |> MemberDetailQuery.makeSuccessResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.Detail(String.ofGuid matchingId)

        mediatorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? IMemberDetailReadModel @>

        let actualReadModel = viewResult.Model :?> IMemberDetailReadModel
        test <@ actualReadModel.Id = matchingId @>
        test <@ actualReadModel.Username = expectedUsername @>
        test <@ actualReadModel.Email = expectedEmail @>
        test <@ actualReadModel.Nickname = expectedNickname @>

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

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.Detail(nonMatchingId.ToString())

        mediatorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "MessageDisplay" @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        test <@ actualReadModel.Heading = expectedHeading @>
        test <@ actualReadModel.SubHeading = expectedSubHeading @>
        test <@ actualReadModel.StatusCode = expectedStatusCode @>
        test <@ actualReadModel.Severity = expectedSeverity @>
        test <@ actualReadModel.Message = expectedMessage @>

    [<Test>]
    let ``Calling list, with matches in data store, returns expected result`` () =
        let expectedMembers = List.init 5 (fun _ ->
            makeMemberListItemReadModel (Guid.random ()) (String.random 64))

        let queryResult =
            makeMemberListReadModel <| Seq.ofList expectedMembers
            |> MemberListQuery.makeSuccessResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun x -> x.Send(any ())).Returns(queryResult).Verifiable()

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.List()

        mediatorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? IMemberListReadModel @>

        let actualMembers = Seq.toList (viewResult.Model :?> IMemberListReadModel).Members
        test <@ List.length actualMembers = 5 @>
        for current in actualMembers do
            let findResult = List.tryFind (fun (x:IMemberListItemReadModel) ->
                x.Id = current.Id && x.Nickname = current.Nickname) expectedMembers
            test <@ findResult.IsSome @>

    [<Test>]
    let ``Calling list, with no matches in data store, returns expected result`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "List"
        let expectedStatusCode = 200
        let expectedSeverity = MessageDisplayReadModel.informationalSeverity
        let expectedMessage = "No members have yet been added."

        let queryResult = MemberListQuery.notFoundResult
        let mediatorMock = Mock<IMediator>()
        mediatorMock.SetupFunc(fun m -> m.Send(any ())).Returns(queryResult).Verifiable()

        let controller = new MemberController(mediatorMock.Object)
        let result = controller.List()

        mediatorMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "MessageDisplay" @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        test <@ actualReadModel.Heading = expectedHeading @>
        test <@ actualReadModel.SubHeading = expectedSubHeading @>
        test <@ actualReadModel.StatusCode = expectedStatusCode @>
        test <@ actualReadModel.Severity = expectedSeverity @>
        test <@ actualReadModel.Message = expectedMessage @>
