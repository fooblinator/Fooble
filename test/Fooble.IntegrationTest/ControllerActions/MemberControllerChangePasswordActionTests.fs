namespace Fooble.IntegrationTest

open Autofac.Integration.Mvc
open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Fooble.Presentation
open Fooble.Web.Controllers
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module MemberControllerChangePasswordActionTests =

    let mutable private originalResolver = null
    let mutable private scopeProvider = null

    [<OneTimeSetUp>]
    let oneTimeSetup () =
        let result = setupDependencyResolver ()
        scopeProvider <- fst result
        originalResolver <- snd result

    [<OneTimeTearDown>]
    let oneTimeTeardown () = DependencyResolver.SetResolver(originalResolver)

    [<TearDown>]
    let teardown () = scopeProvider.EndLifetimeScope()

    type private QueryResult =
        | Success of Guid
        | NotFound

    type private CommandResult = Success | NotFound | IncorrectPassword

    let private addMemberData (context:IFoobleContext) (memberDataFactory:MemberDataFactory) id currentPassword =
        let passwordData =
            match currentPassword with
            | Some(x) -> x
            | None -> randomPassword 32
            |> fun x -> Crypto.hash x 100
        memberDataFactory.Invoke(id, randomString 32, passwordData, randomEmail 32, randomString 64, randomString 32,
            DateTime(2001, 01, 01), DateTime(2001, 01, 01), None)
        |> context.AddMember

    let private setupForGetActionTest result =
        let resolver = AutofacDependencyResolver.Current
        let context = resolver.GetService<IFoobleContext>()
        let memberDataFactory = resolver.GetService<MemberDataFactory>()
        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(includeDeactivated = true))
        // add member to the data store (if required)
        match result with
        | QueryResult.Success(id) -> addMemberData context memberDataFactory id None
        | QueryResult.NotFound -> ()
        // persist changes to the data store
        context.SaveChanges()
        resolver.GetService<MemberController>()

    let private setupForPostActionTest result id currentPassword password confirmPassword =
        let resolver = AutofacDependencyResolver.Current
        let context = resolver.GetService<IFoobleContext>()
        let memberDataFactory = resolver.GetService<MemberDataFactory>()
        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(includeDeactivated = true))
        // add member to the data store (if required)
        match result with
        | Success -> addMemberData context memberDataFactory id (Some(currentPassword))
        | NotFound -> ()
        | IncorrectPassword -> addMemberData context memberDataFactory id None
        // persist changes to the data store
        context.SaveChanges()
        let controller = resolver.GetService<MemberController>()
        let (viewModel, modelState) = bindMemberChangePasswordViewModel id currentPassword password confirmPassword
        controller.ModelState.Merge(modelState)
        (controller, viewModel)

    [<Test>]
    let ``Calling change password get action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let controller = setupForGetActionTest (QueryResult.Success(id))
        let result = controller.ChangePassword(id)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty

    [<Test>]
    let ``Calling change password get action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let heading = "Member"
        let subHeading = "Change Password"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let controller = setupForGetActionTest QueryResult.NotFound
        let result = controller.ChangePassword(notFoundId)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change password post action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let password = randomPassword 32
        let confirmPassword = password
        let (controller, viewModel) = setupForPostActionTest Success id currentPassword password confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? RedirectToRouteResult =! true
        let redirectResult = result :?> RedirectToRouteResult
        let routeValues = redirectResult.RouteValues
        routeValues.ContainsKey("controller") =! true
        routeValues.["controller"].ToString().ToLowerInvariant() =! "member"
        routeValues.ContainsKey("action") =! true
        routeValues.["action"].ToString().ToLowerInvariant() =! "detail"
        routeValues.ContainsKey("id") =! true
        routeValues.["id"].ToString().ToLowerInvariant() =! string id

    [<Test>]
    let ``Calling change password post action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let password = randomPassword 32
        let confirmPassword = password
        let heading = "Member"
        let subHeading = "Change Password"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, viewModel) = setupForPostActionTest NotFound notFoundId currentPassword password confirmPassword
        let result = controller.ChangePassword(notFoundId, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change password post action, with incorrect current password, returns expected result`` () =
        let id = Guid.NewGuid()
        let incorrectPassword = randomPassword 32
        let password = randomPassword 32
        let confirmPassword = password
        let (controller, viewModel) = setupForPostActionTest IncorrectPassword id incorrectPassword password confirmPassword
        let result = controller.ChangePassword(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangePasswordViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangePasswordViewModel
        testMemberChangePasswordViewModel viewModel id String.Empty String.Empty String.Empty
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "currentPassword" "Current password is incorrect"
