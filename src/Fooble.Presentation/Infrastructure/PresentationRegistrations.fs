namespace Fooble.Presentation.Infrastructure

open Autofac
open Autofac.Integration.Mvc
open Fooble.Common

/// Provides the proper Autofac container registrations for the Fooble.Core assembly.
[<AllowNullLiteral>]
type PresentationRegistrations() =
    inherit Module()

    override this.Load(builder:ContainerBuilder) =
#if DEBUG
        assertWith (validateRequired builder "builder" "Builder")
#endif

        ignore (builder.RegisterModelBinders(this.ThisAssembly))
