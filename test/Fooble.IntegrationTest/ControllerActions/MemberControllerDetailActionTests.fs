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
module MemberControllerDetailActionTests =

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

    type private Result =
        | Success of Guid * string * string * string * string * DateTime * DateTime
        | NotFound

    let private setupForGetActionTest result =
        let resolver = AutofacDependencyResolver.Current
        let context = resolver.GetService<IFoobleContext>()
        let memberDataFactory = resolver.GetService<MemberDataFactory>()
        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(includeDeactivated = true))
        // add member to the data store (if required)
        match result with
        | Success(id, username, email, nickname, avatarData, registeredOn, passwordChangedOn) ->
              randomPassword 32
              |> fun x -> Crypto.hash x 100
              |> fun x ->
                     memberDataFactory.Invoke(id, username, x, email, nickname, avatarData, registeredOn,
                         passwordChangedOn, None)
              |> context.AddMember
        | NotFound -> ()
        // persist changes to the data store
        context.SaveChanges()
        resolver.GetService<MemberController>()

    [<Test>]
    let ``Calling detail get action, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let registered = DateTime.UtcNow
        let passwordChanged = DateTime.UtcNow
        let controller =
            setupForGetActionTest (Success(id, username, email, nickname, avatarData, registered, passwordChanged))
        let result = controller.Detail(id)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberDetailReadModel =! true
        let readModel = viewResult.Model :?> IMemberDetailReadModel
        testMemberDetailReadModel readModel id username email nickname avatarData registered passwordChanged

    [<Test>]
    let ``Calling detail get action, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let heading = "Member"
        let subHeading = "Detail"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let controller = setupForGetActionTest NotFound
        let result = controller.Detail(notFoundId)
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let readModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message
