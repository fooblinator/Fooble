namespace Fooble.Tests

open Autofac
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Core.Persistence
open Fooble.Web.Controllers
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System.Data.Entity
open System.Web.Mvc

[<TestFixture>]
module MemberControllerIntegrationTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result``() =
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = mock()))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        new MemberController(mediator) |> ignore

    [<Test>]
    let ``Calling detail, with matches in data store, returns expected result``() =
        let matchingId = randomGuidString()

        let memberData = MemberData(Id = matchingId, Name = randomGuidString()) :> IMemberData
        let memberSetMock = Mock<IDbSet<IMemberData>>()
        memberSetMock.SetupFunc(fun ms -> ms.Find(It.IsAny<string>())).Returns(memberData).End
        let contextMock = Mock<IDataContext>()
        contextMock.SetupFunc(fun c -> c.Members).Returns(memberSetMock.Object).End

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = contextMock.Object))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.Detail(matchingId)

        contextMock.VerifyFunc((fun c -> c.Members), Times.Once())
        memberSetMock.VerifyFunc((fun s -> s.Find(It.IsAny<string>())), Times.Once())

        test <@ not (isNull result) @>
        test <@ result :? ViewResult @>
        let viewResult = result :?> ViewResult
        test <@ viewResult.ViewName = "" @>
        test <@ not (isNull viewResult.Model) @>
        test <@ viewResult.Model :? IMemberDetailReadModel @>
        let actualViewModel = viewResult.Model :?> IMemberDetailReadModel
        test <@ actualViewModel.Id = memberData.Id @>
        test <@ actualViewModel.Name = memberData.Name @>

    [<Test>]
    let ``Calling detail, with no matches in data store, returns expected result``() =
        let nonMatchingId = randomGuidString()
        let expectedHeading = "Member Detail Query"
        let expectedSeverity = MessageDisplay.errorSeverity
        let expectedMessages = [ "Member detail query was not successful and returned not found" ]

        let memberSetMock = Mock<IDbSet<IMemberData>>()
        memberSetMock.SetupFunc(fun ms -> ms.Find(It.IsAny<string>())).Returns(null :> IMemberData).End
        let contextMock = Mock<IDataContext>()
        contextMock.SetupFunc(fun c -> c.Members).Returns(memberSetMock.Object).End

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = contextMock.Object))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.Detail(nonMatchingId)

        contextMock.VerifyFunc((fun c -> c.Members), Times.Once())
        memberSetMock.VerifyFunc((fun s -> s.Find(It.IsAny<string>())), Times.Once())

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
    let ``Calling list, with matches in data store, returns expected result``() =
        let memberData = 
            [ MemberData(Id = randomGuidString(), Name = randomGuidString()) :> IMemberData
              MemberData(Id = randomGuidString(), Name = randomGuidString()) :> IMemberData
              MemberData(Id = randomGuidString(), Name = randomGuidString()) :> IMemberData
              MemberData(Id = randomGuidString(), Name = randomGuidString()) :> IMemberData
              MemberData(Id = randomGuidString(), Name = randomGuidString()) :> IMemberData ]
        let memberSet = makeDbSet (Seq.ofList memberData)
        let contextMock = Mock<IDataContext>()
        contextMock.SetupFunc(fun c -> c.Members).Returns(memberSet).End

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = contextMock.Object))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.List()

        contextMock.VerifyFunc((fun c -> c.Members), Times.Once())

        test <@ not (isNull result) @>
        test <@ result :? ViewResult @>
        let viewResult = result :?> ViewResult
        test <@ viewResult.ViewName = "" @>
        test <@ not (isNull viewResult.Model) @>
        test <@ viewResult.Model :? IMemberListReadModel @>
        let actualMembers = Seq.toList (viewResult.Model :?> IMemberListReadModel).Members
        test <@ (List.length actualMembers) = 5 @>
        for current in actualMembers do
            let findResult = List.tryFind (fun (m:IMemberData) -> m.Id = current.Id && m.Name = current.Name) memberData
            test <@ findResult.IsSome @>

    [<Test>]
    let ``Calling list, with no matches in data store, returns expected result``() =
        let expectedHeading = "Member List Query"
        let expectedSeverity = MessageDisplay.errorSeverity
        let expectedMessages = [ "Member list query was not successful and returned not found" ]

        let memberSet = makeDbSet Seq.empty<IMemberData>
        let contextMock = Mock<IDataContext>()
        contextMock.SetupFunc(fun c -> c.Members).Returns(memberSet).End

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = contextMock.Object))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.List()

        contextMock.VerifyFunc((fun c -> c.Members), Times.Once())

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
