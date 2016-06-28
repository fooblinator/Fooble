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
module MemberControllerChangeUsernameActionTests =

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

    type private CommandResult = Success | NotFound | IncorrectPassword | UnavailableUsername

    let private addMemberData (context:IFoobleContext) (memberDataFactory:MemberDataFactory) id currentPassword
        username =

        let id =
            match id with
            | Some(x) -> x
            | None -> Guid.NewGuid()
        let passwordData =
            match currentPassword with
            | Some(x) -> x
            | None -> randomPassword 32
            |> fun x -> Crypto.hash x 100
        let username =
            match username with
            | Some(x) -> x
            | None -> randomString 32
        memberDataFactory.Invoke(id,username, passwordData, randomEmail 32, randomString 64, randomString 32,
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
        | QueryResult.Success(id, currentPassword, username) ->
              addMemberData context memberDataFactory (Some(id)) (Some(currentPassword)) (Some(username))
        | QueryResult.NotFound -> ()
        // persist changes to the data store
        context.SaveChanges()
        resolver.GetService<MemberController>()

    let private setupForPostActionTest result id currentPassword username =
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
        | UnavailableUsername ->
              addMemberData context memberDataFactory None None (Some(username))
              addMemberData context memberDataFactory (Some(id)) (Some(currentPassword)) None
        // persist changes to the data store
        context.SaveChanges()
        let controller = resolver.GetService<MemberController>()
        let (viewModel, modelState) = bindMemberChangeUsernameViewModel id currentPassword username
        controller.ModelState.Merge(modelState)
        (controller, viewModel)

    [<Test>]
    let ``Calling change username get action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let controller = setupForGetActionTest (QueryResult.Success(id, String.Empty, username))
        let result = controller.ChangeUsername(id)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel viewModel id String.Empty username

    [<Test>]
    let ``Calling change username get action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let controller = setupForGetActionTest QueryResult.NotFound
        let result = controller.ChangeUsername(notFoundId)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change username post action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let username = randomString 32
        let (controller, viewModel) = setupForPostActionTest Success id currentPassword username
        let result = controller.ChangeUsername(id, viewModel)
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
    let ``Calling change username post action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let username = randomString 32
        let heading = "Member"
        let subHeading = "Change Username"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, viewModel) = setupForPostActionTest NotFound notFoundId currentPassword username
        let result = controller.ChangeUsername(notFoundId, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change username post action, with incorrect current password, returns expected result`` () =
        let id = Guid.NewGuid()
        let incorrectPassword = randomPassword 32
        let username = randomString 32
        let (controller, viewModel) = setupForPostActionTest IncorrectPassword id incorrectPassword username
        let result = controller.ChangeUsername(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel viewModel id String.Empty username
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "currentPassword" "Current password is incorrect"

    [<Test>]
    let ``Calling change username post action, with unavailable username, returns expected result`` () =
        let id = Guid.NewGuid()
        let currentPassword = randomPassword 32
        let unavailableUsername = randomString 32
        let (controller, viewModel) = setupForPostActionTest UnavailableUsername id currentPassword unavailableUsername
        let result = controller.ChangeUsername(id, viewModel)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeUsernameViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeUsernameViewModel
        testMemberChangeUsernameViewModel viewModel id String.Empty unavailableUsername
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is unavailable"
