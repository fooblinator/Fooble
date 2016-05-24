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

        raisesWith<ArgumentException> <@ new MemberController(null) @> (fun x ->
            <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore <| new MemberController(mock ())

    [<Test>]
    let ``Calling detail, with matches in data store, returns expected result`` () =
        let matchingId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedName = String.random 64

        let readModel = MemberDetail.ReadModel.make matchingId expectedUsername expectedName
        let queryResult = MemberDetail.QueryResult.makeSuccess readModel
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

        let actualViewModel = viewResult.Model :?> IMemberDetailReadModel
        test <@ actualViewModel.Id = matchingId @>
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Name = expectedName @>

    [<Test>]
    let ``Calling detail, with no matches in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedHeading = "Member"
        let expectedSubHeading = "Detail"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplay.Severity.warning
        let expectedMessage = "No matching member could be found."

        let queryResult = MemberDetail.QueryResult.notFound
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
            MemberList.ItemReadModel.make (Guid.random ()) (String.random 64))

        let readModel = MemberList.ReadModel.make <| Seq.ofList expectedMembers
        let queryResult = MemberList.QueryResult.makeSuccess readModel
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
                x.Id = current.Id && x.Name = current.Name) expectedMembers
            test <@ findResult.IsSome @>

    [<Test>]
    let ``Calling list, with no matches in data store, returns expected result`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "List"
        let expectedStatusCode = 200
        let expectedSeverity = MessageDisplay.Severity.informational
        let expectedMessage = "No members have yet been added."

        let queryResult = MemberList.QueryResult.notFound
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
