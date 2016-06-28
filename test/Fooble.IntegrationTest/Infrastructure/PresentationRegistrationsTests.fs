namespace Fooble.IntegrationTest

open Autofac.Integration.Mvc
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System.Web.Mvc

[<TestFixture>]
module PresentationRegistrationsTests =

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

    [<Test>]
    let ``Registering autofac container, properly registers expected model binders`` () =
        let resolver = AutofacDependencyResolver.Current
        let modelBinders = resolver.GetServices<IModelBinder>()
        isNull modelBinders =! false
        Seq.isEmpty modelBinders =! false
        Seq.length modelBinders =! 6
        for modelBinder in modelBinders do
            let matched =
                match modelBinder with
                | :? MemberChangeEmailViewModelBinder
                | :? MemberChangeOtherViewModelBinder
                | :? MemberChangePasswordViewModelBinder
                | :? MemberChangeUsernameViewModelBinder
                | :? MemberDeactivateViewModelBinder
                | :? MemberRegisterViewModelBinder -> true
                | _ -> false
            matched =! true
