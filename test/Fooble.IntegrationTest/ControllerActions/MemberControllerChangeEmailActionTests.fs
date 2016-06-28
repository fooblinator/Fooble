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
module MemberControllerChangeEmailActionTests =

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
        | Success of Guid * string * string
        | NotFound

    type private CommandResult = Success | NotFound | IncorrectPassword | UnavailableEmail

    let private addMemberData (context:IFoobleContext) (memberDataFactory:MemberDataFactory) id currentPassword email =
        let id =
            match id with
            | Some(x) -> x
            | None -> Guid.NewGuid()
        let passwordData =
            match currentPassword with
            | Some(x) -> x
            | None -> randomPassword 32
            |> fun x -> Crypto.hash x 100
        let email =
            match email with
            | Some(x) -> x
            | None -> randomEmail 32
        memberDataFactory.Invoke(id, randomString 32, passwordData, email, randomString 64, randomString 32,
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
        | QueryResult.Success(id, currentPassword, email) ->
              addMemberData context memberDataFactory (Some(id)) (Some(currentPassword)) (Some(email))
        | QueryResult.NotFound -> ()
        // persist changes to the data store
        context.SaveChanges()
        resolver.GetService<MemberController>()

    let private setupForPostActionTest result id currentPassword email =
        let resolver = AutofacDependencyResolver.Current
        let context = resolver.GetService<IFoobleContext>()
        let memberDataFactory = resolver.GetService<MemberDataFactory>()
        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(includeDeactivated = true))
        // add member to the data store (if required)
        match result with
        | Success -> addMemberData context memberDataFactory (Some(id)) (Some(currentPassword)) None
        | NotFound -> ()
        | IncorrectPassword -> addMemberData context memberDataFactory (Some(id)) None None
        | UnavailableEmail ->
              addMemberData context memberDataFactory  None None (Some(email))
              addMemberData context memberDataFactory (Some(id)) (Some(currentPassword)) None
        // persist changes to the data store
        context.SaveChanges()
        let controller = resolver.GetService<MemberController>()
        let (viewModel, modelState) = bindMemberChangeEmailViewModel id currentPassword email
        controller.ModelState.Merge(modelState)
        (controller, viewModel)

    [<Test>]
    let ``Calling change email get action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let email = randomEmail 32
        let controller = setupForGetActionTest (QueryResult.Success(id, String.Empty, email))
        let result = controller.ChangeEmail(id)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel viewModel id String.Empty email

    [<Test>]
    let ``Calling change email get action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let heading = "Member"
        let subHeading = "Change Email"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let controller = setupForGetActionTest QueryResult.NotFound
        let result = controller.ChangeEmail(notFoundId)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change email post action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let email = randomEmail 32
        let (controller, viewModel) = setupForPostActionTest Success id currentPassword email
        let result = controller.ChangeEmail(id, viewModel)
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
    let ``Calling change email post action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let email = randomEmail 32
        let heading = "Member"
        let subHeading = "Change Email"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, viewModel) = setupForPostActionTest NotFound notFoundId currentPassword email
        let result = controller.ChangeEmail(notFoundId, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change email post action, with incorrect current password, returns expected result`` () =
        let id = Guid.NewGuid()
        let incorrectPassword = randomPassword 32
        let email = randomEmail 32
        let (controller, viewModel) = setupForPostActionTest IncorrectPassword id incorrectPassword email
        let result = controller.ChangeEmail(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel viewModel id String.Empty email
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change email post action, with unavailable email, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let unavailableEmail = randomEmail 32
        let (controller, viewModel) = setupForPostActionTest UnavailableEmail id currentPassword unavailableEmail
        let result = controller.ChangeEmail(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeEmailViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeEmailViewModel
        testMemberChangeEmailViewModel viewModel id String.Empty unavailableEmail
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is already registered"
