namespace Fooble.IntegrationTest

open Autofac.Integration.Mvc
open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Fooble.Presentation
open Fooble.Web.Controllers
open MediatR
open NUnit.Framework
open Swensen.Unquote
open System
open System.Web.Mvc

[<TestFixture>]
module MemberControllerRegisterActionTests =

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

    type private Result = Success | UnavailableUsername | UnavailableEmail

    let private addMemberData (context:IFoobleContext) (memberDataFactory:MemberDataFactory) username email =
        let passwordData = randomPassword 32 |> fun x -> Crypto.hash x 100
        let username =
            match username with
            | Some(x) -> x
            | None -> randomString 32
        let email =
            match email with
            | Some(x) -> x
            | None -> randomEmail 32
        memberDataFactory.Invoke(Guid.NewGuid(), username, passwordData, email, randomString 64, randomString 32,
            DateTime(2001, 01, 01), DateTime(2001, 01, 01), None)
        |> context.AddMember

    let private setupForGetActionTest () =
        let resolver = AutofacDependencyResolver.Current
        resolver.GetService<MemberController>()

    let private setupForPostActionTest result idGeneratorResult username password confirmPassword email nickname
        avatarData =

        let resolver = AutofacDependencyResolver.Current
        let context = resolver.GetService<IFoobleContext>()
        let memberDataFactory = resolver.GetService<MemberDataFactory>()
        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(includeDeactivated = true))
        // add member to the data store (if required)
        match result with
        | Success -> ()
        | UnavailableUsername -> addMemberData context memberDataFactory (Some(username)) None
        | UnavailableEmail -> addMemberData context memberDataFactory None (Some(email))
        // persist changes to the data store
        context.SaveChanges()
        let mediator = resolver.GetService<IMediator>()
        let idGenerator = makeIdGenerator idGeneratorResult
        let memberChangePasswordViewModelFactory = resolver.GetService<InitialMemberChangePasswordViewModelFactory>()
        let memberDeactivateViewModelFactory = resolver.GetService<InitialMemberDeactivateViewModelFactory>()
        let memberDetailQueryFactory = resolver.GetService<MemberDetailQueryFactory>()
        let memberEmailQueryFactory = resolver.GetService<MemberEmailQueryFactory>()
        let memberExistsQueryFactory = resolver.GetService<MemberExistsQueryFactory>()
        let memberListQueryFactory = resolver.GetService<MemberListQueryFactory>()
        let memberOtherQueryFactory = resolver.GetService<MemberOtherQueryFactory>()
        let memberRegisterViewModelFactory = resolver.GetService<InitialMemberRegisterViewModelFactory>()
        let memberUsernameQueryFactory = resolver.GetService<MemberUsernameQueryFactory>()
        let controller =
            new MemberController(mediator, idGenerator, memberChangePasswordViewModelFactory,
                memberDeactivateViewModelFactory, memberDetailQueryFactory, memberEmailQueryFactory,
                memberExistsQueryFactory, memberListQueryFactory, memberOtherQueryFactory,
                memberRegisterViewModelFactory, memberUsernameQueryFactory)
        let (viewModel, modelState) =
            bindMemberRegisterViewModel username password confirmPassword email nickname avatarData
        controller.ModelState.Merge(modelState)
        (controller, viewModel)

    [<Test>]
    let ``Calling register get action, returns expected result`` () =
        let controller = setupForGetActionTest ()
        let result = controller.Register()
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        viewModel.AvatarData <>! String.Empty
        testMemberRegisterViewModel viewModel String.Empty String.Empty String.Empty String.Empty String.Empty None

    [<Test>]
    let ``Calling register post action, with successful parameters, and default submit, returns expected result`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel) =
            setupForPostActionTest Success (Some(id)) username password confirmPassword email nickname avatarData
        let result = controller.Register(viewModel, "default")
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
    let ``Calling change username post action, with unavailable username, returns expected result`` () =
        let unavailableUsername = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let email = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel) =
            setupForPostActionTest UnavailableUsername None unavailableUsername password confirmPassword email nickname
                avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel unavailableUsername String.Empty String.Empty email nickname
            (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "username" "Username is unavailable"

    [<Test>]
    let ``Calling change email post action, with unavailable email, returns expected result`` () =
        let username = randomString 32
        let password = randomPassword 32
        let confirmPassword = password
        let unavailableEmail = randomEmail 32
        let nickname = randomString 32
        let avatarData = randomString 32
        let (controller, viewModel) =
            setupForPostActionTest UnavailableEmail None username password confirmPassword unavailableEmail nickname
                avatarData
        let result = controller.Register(viewModel, "default")
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberRegisterViewModel =! true
        let viewModel = viewResult.Model :?> IMemberRegisterViewModel
        testMemberRegisterViewModel viewModel username String.Empty String.Empty unavailableEmail nickname
            (Some(avatarData))
        let modelState = viewResult.ViewData.ModelState
        testModelState modelState "email" "Email is already registered"
