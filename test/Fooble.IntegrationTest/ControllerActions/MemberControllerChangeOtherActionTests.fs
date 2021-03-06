﻿namespace Fooble.IntegrationTest

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
module MemberControllerChangeOtherActionTests =

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

    type private CommandResult = Success | NotFound

    let private addMemberData (context:IFoobleContext) (memberDataFactory:MemberDataFactory) id nickname avatarData =
        let passwordData = randomPassword 32 |> fun x -> Crypto.hash x 100
        let nickname =
            match nickname with
            | Some(x) -> x
            | None -> randomString 64
        let avatarData =
            match avatarData with
            | Some(x) -> x
            | None -> randomString 32
        memberDataFactory.Invoke(id, randomString 32, passwordData, randomEmail 32, nickname, avatarData,
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
        | QueryResult.Success(id, nickname, avatarData) ->
              addMemberData context memberDataFactory id (Some(nickname)) (Some(avatarData))
        | QueryResult.NotFound -> ()
        // persist changes to the data store
        context.SaveChanges()
        resolver.GetService<MemberController>()

    let private setupForPostActionTest result id nickname avatarData =
        let resolver = AutofacDependencyResolver.Current
        let context = resolver.GetService<IFoobleContext>()
        let memberDataFactory = resolver.GetService<MemberDataFactory>()
        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(includeDeactivated = true))
        // add member to the data store (if required)
        match result with
        | Success -> addMemberData context memberDataFactory id None None
        | NotFound -> ()
        // persist changes to the data store
        context.SaveChanges()
        let controller = resolver.GetService<MemberController>()
        let (viewModel, modelState) = bindMemberChangeOtherViewModel id nickname avatarData
        controller.ModelState.Merge(modelState)
        (controller, viewModel)

    [<Test>]
    let ``Calling change other get action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 32
        let avatarData = randomString 32
        let controller = setupForGetActionTest (QueryResult.Success(id, nickname, avatarData))
        let result = controller.ChangeOther(id)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberChangeOtherViewModel =! true
        let viewModel = viewResult.Model :?> IMemberChangeOtherViewModel
        testMemberChangeOtherViewModel viewModel id nickname avatarData

    [<Test>]
    let ``Calling change other get action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let heading = "Member"
        let subHeading = "Change Other"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let controller = setupForGetActionTest QueryResult.NotFound
        let result = controller.ChangeOther(notFoundId)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling change other post action, with successful parameters, and default submit, returns expected result`` () =
        let id = Guid.NewGuid()
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel) = setupForPostActionTest Success id nickname avatarData
        let result = controller.ChangeOther(id, viewModel, "default")
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
    let ``Calling change other post action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let nickname = randomString 32
        let avatarData = randomString 32
        let heading = "Member"
        let subHeading = "Change Other"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let (controller, viewModel) = setupForPostActionTest NotFound notFoundId nickname avatarData
        let result = controller.ChangeOther(notFoundId, viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message
