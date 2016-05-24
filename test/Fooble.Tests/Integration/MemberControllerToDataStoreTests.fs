namespace Fooble.Tests.Integration

open Autofac
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Core.Persistence
open Fooble.Tests
open Fooble.Web.Controllers
open MediatR
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module MemberControllerToDataStoreTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(ConnectionString = connectionString))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        ignore <| new MemberController(mediator)

    [<Test>]
    let ``Calling detail, with matches in data store, returns expected result`` () =
        let expectedId = randomGuid ()
        let expectedName = randomString ()

        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // add matching member to the data store
        let memberData = MemberData(Id = expectedId, Name = expectedName)
        context.MemberData.AddObject(memberData)

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = context))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.Detail(expectedId.ToString())

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
        let expectedReadModel =
            MessageDisplay.ReadModel.make "Member" "Detail" 404 MessageDisplay.Severity.warning
                "No matching member could be found."

        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = context))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.Detail(nonMatchingId.ToString())

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
        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // add members to the data store
        let memberData = List.init 5 <| fun _ -> MemberData(Id = randomGuid (), Name = randomString ())
        List.iter (fun x -> context.MemberData.AddObject(x)) memberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = context))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.List()

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
        let expectedReadModel =
            MessageDisplay.ReadModel.make "Member" "List" 200 MessageDisplay.Severity.informational
                "No members have yet been added."

        let connectionString = Settings.ConnectionStrings.FoobleContext
        use context = makeFoobleContext <| Some connectionString

        // remove all existing members from the data store
        Seq.iter (fun x -> context.MemberData.DeleteObject(x)) context.MemberData

        // persist changes to the data store
        ignore <| context.SaveChanges()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = context))
        let container = builder.Build()
        
        let mediator = container.Resolve<IMediator>()
        let controller = new MemberController(mediator)
        let result = controller.List()

        test <@ notIsNull result @>
        test <@ result :? ViewResult @>

        let viewResult = result :?> ViewResult

        test <@ viewResult.ViewName = "MessageDisplay" @>
        test <@ notIsNull viewResult.Model @>
        test <@ viewResult.Model :? IMessageDisplayReadModel @>

        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        test <@ actualReadModel = expectedReadModel @>
