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
module MemberControllerListActionTests =

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

    [<NoComparison>]
    type private Result =
        | Success of seq<(Guid * string * string)> * int
        | NotFound

    let private setupForGetActionTest result =
        let resolver = AutofacDependencyResolver.Current
        let context = resolver.GetService<IFoobleContext>()
        let memberDataFactory = resolver.GetService<MemberDataFactory>()
        // remove all existing members from the data store
        List.iter (fun x -> context.DeleteMember(x)) (context.GetMembers(includeDeactivated = true))
        // add member to the data store (if required)
        match result with
        | Success(members, _) ->
            Seq.map (fun (id, nickname, avatarData) ->
                randomPassword 32
                    |> fun x -> Crypto.hash x 100
                    |> fun x ->
                           memberDataFactory.Invoke(id, randomString 32, x, randomEmail 32, nickname, avatarData,
                               DateTime(2001, 1, 1), DateTime(2001, 1, 1), None)) members
            |> Seq.iter (fun x -> context.AddMember(x))
        | NotFound -> ()
        // persist changes to the data store
        context.SaveChanges()
        resolver.GetService<MemberController>()

    [<Test>]
    let ``Calling list get action, with successful parameters, returns expected result`` () =
        let memberCount = 5
        let members = List.init memberCount (fun _ -> (Guid.NewGuid(), randomString 64, randomString 32))
        let controller = setupForGetActionTest (Success(members, memberCount))
        let result = controller.List()
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! String.Empty
        isNull viewResult.Model =! false
        viewResult.Model :? IMemberListReadModel =! true
        let readModel = viewResult.Model :?> IMemberListReadModel
        testMemberListReadModel readModel members memberCount

    [<Test>]
    let ``Calling list get action, with id not found in data store, returns expected result`` () =
        let heading = "Member"
        let subHeading = "List"
        let statusCode = 200
        let severity = MessageDisplayReadModel.informationalSeverity
        let message = "No members have yet been added."
        let controller = setupForGetActionTest NotFound
        let result = controller.List()
        isNull result =! false
        result :? ViewResult =! true
        let viewResult = result :?> ViewResult
        viewResult.ViewName =! "MessageDisplay"
        isNull viewResult.Model =! false
        viewResult.Model :? IMessageDisplayReadModel =! true
        let actualReadModel = viewResult.Model :?> IMessageDisplayReadModel
        testMessageDisplayReadModel actualReadModel heading subHeading statusCode severity message
