﻿namespace Fooble.Tests.Integration

open Autofac
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Core.Persistence
open Fooble.Tests
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
        ignore <| builder.RegisterModule(AutofacModule(Context = mock ()))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        ignore <| new MemberController(mediator)

    [<Test>]
    let ``Calling detail, with matches in data store, returns expected result`` () =
        let expectedId = randomGuid ()
        let expectedName = randomString ()

        let memberData = MemberData(Id = expectedId, Name = expectedName)
        let memberSetMock = makeObjectSet <| Seq.singleton memberData
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun c -> c.MemberData).Returns(memberSetMock.Object).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = contextMock.Object))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.Detail(expectedId.ToString())

        contextMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? IMemberDetailReadModel @>

        let actualViewModel = viewResult.Model :?> IMemberDetailReadModel

        test <@ actualViewModel.Id = expectedId @>
        test <@ actualViewModel.Name = expectedName @>

    [<Test>]
    let ``Calling detail, with no matches in data store, returns expected result`` () =
        let nonMatchingId = randomGuid ()
        let expectedHeading = "Member Detail"
        let expectedSeverity = MessageDisplay.Severity.error
        let expectedMessages = [ "Member detail query was not successful and returned \"not found\"" ]

        let memberSet = makeObjectSet Seq.empty<MemberData>
        let memberSetMock = Mock<IFoobleContext>()
        memberSetMock.SetupFunc(fun x -> x.MemberData).Returns(memberSet.Object).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = memberSetMock.Object))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.Detail(nonMatchingId.ToString())

        memberSetMock.Verify()

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
        let memberData = List.init 5 <| fun _ -> MemberData(Id = randomGuid (), Name = randomString ())
        let memberSetMock = makeObjectSet <| Seq.ofList memberData
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun c -> c.MemberData).Returns(memberSetMock.Object).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = contextMock.Object))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.List()

        contextMock.Verify()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ String.isEmpty viewResult.ViewName @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? IMemberListReadModel @>

        let actualMembers = Seq.toList (viewResult.Model :?> IMemberListReadModel).Members
        test <@ (List.length actualMembers) = 5 @>
        for current in actualMembers do
            let findResult = List.tryFind (fun (x:MemberData) -> x.Id = current.Id && x.Name = current.Name) memberData
            test <@ findResult.IsSome @>

    [<Test>]
    let ``Calling list, with no matches in data store, returns expected result`` () =
        let expectedHeading = "Member List"
        let expectedSeverity = MessageDisplay.Severity.error
        let expectedMessages = [ "Member list query was not successful and returned \"not found\"" ]

        let memberSetMock = makeObjectSet Seq.empty<MemberData>
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun c -> c.MemberData).Returns(memberSetMock.Object).Verifiable()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = contextMock.Object))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.List()

        contextMock.Verify()

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