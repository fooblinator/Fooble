namespace Fooble.IntegrationTest

open Autofac
open Autofac.Core.Lifetime
open Autofac.Integration.Mvc
open Fooble.Core.Infrastructure
open Fooble.Persistence.Infrastructure
open Fooble.Presentation.Infrastructure
open Fooble.Web.Controllers
open FSharp.Configuration
open System
open System.Web.Mvc

type internal Settings = AppSettings<"App.config">

[<AutoOpen>]
module internal IntegrationTestHelpers =

    let private makeLifetimeScopeProvider (container:IContainer) =
        let container = container :> ILifetimeScope
        let mutable scope:ILifetimeScope = null
        { new ILifetimeScopeProvider with
              member __.ApplicationContainer with get() = container
              member __.EndLifetimeScope() = if not (isNull scope) then scope.Dispose(); scope <- null
              member __.GetLifetimeScope(configurationAction:Action<ContainerBuilder>) =
                  if isNull scope then
                      if isNull configurationAction then
                          scope <- container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag)
                      else
                          scope <- container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag,
                              configurationAction)
                  scope }

    let setupDependencyResolver () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        ignore (builder.RegisterControllers(typeof<MemberController>.Assembly))
        let container = builder.Build()
        let scopeProvider = makeLifetimeScopeProvider container
        let resolver = AutofacDependencyResolver(container, scopeProvider)
        let originalResolver = DependencyResolver.Current
        DependencyResolver.SetResolver(resolver)
        (scopeProvider, originalResolver)
