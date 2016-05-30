namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core.Infrastructure
open Fooble.IntegrationTest
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open Fooble.Presentation
open Fooble.Presentation.Infrastructure
open Fooble.Web.Controllers
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module MemberControllerToQueryHandlerTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        let container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        ignore <| new MemberController(mediator)

    [<Test>]
    let ``Calling detail, with match in data store, returns expected result`` () =
        let expectedId = Guid.random ()
        let expectedUsername = String.random 32
        let expectedEmail = sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)
        let expectedNickname = String.random 64

        let memberData = makeMemberData expectedId expectedUsername expectedEmail expectedNickname
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(Some memberData).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(contextMock.Object, makeMemberDataFactory ()))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        let container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.Detail(expectedId.ToString())

        contextMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? IMemberDetailReadModel @>

        let actualViewModel = viewResult.Model :?> IMemberDetailReadModel

        test <@ actualViewModel.Id = expectedId @>
        test <@ actualViewModel.Username = expectedUsername @>
        test <@ actualViewModel.Email = expectedEmail @>
        test <@ actualViewModel.Nickname = expectedNickname @>

    [<Test>]
    let ``Calling detail, with no match in data store, returns expected result`` () =
        let nonMatchingId = Guid.random ()
        let expectedHeading = "Member"
        let expectedSubHeading = "Detail"
        let expectedStatusCode = 404
        let expectedSeverity = MessageDisplayReadModel.warningSeverity
        let expectedMessage = "No matching member could be found."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(any ())).Returns(None).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(contextMock.Object, makeMemberDataFactory ()))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        let container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.Detail(nonMatchingId.ToString())

        contextMock.Verify()

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
        let members = List.init 5 (fun _ ->
            makeMemberData (Guid.random ()) (String.random 32)
                (sprintf "%s@%s.%s" (String.random 32) (String.random 32) (String.random 3)) (String.random 64))
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMembers()).Returns(members).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(contextMock.Object, makeMemberDataFactory ()))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        let container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.List()

        contextMock.Verify()

        test <@ isNotNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ isNotNull viewResult.Model @>
        test <@ viewResult.Model :? IMemberListReadModel @>

        let actualMembers = Seq.toList (viewResult.Model :?> IMemberListReadModel).Members
        test <@ (List.length actualMembers) = 5 @>
        for current in actualMembers do
            let findResult =
                List.tryFind (fun (x:IMemberData) -> x.Id = current.Id && x.Nickname = current.Nickname) members
            test <@ findResult.IsSome @>

    [<Test>]
    let ``Calling list, with no matches in data store, returns expected result`` () =
        let expectedHeading = "Member"
        let expectedSubHeading = "List"
        let expectedStatusCode = 200
        let expectedSeverity = MessageDisplayReadModel.informationalSeverity
        let expectedMessage = "No members have yet been added."

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMembers()).Returns([]).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(contextMock.Object, makeMemberDataFactory ()))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        let container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.List()

        contextMock.Verify()

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
