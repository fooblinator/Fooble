namespace Fooble.Presentation.Infrastructure

open Autofac
open Fooble.Common
open Fooble.Presentation

/// Provides the proper Autofac container registrations for the Fooble.Presentation assembly.
[<AllowNullLiteral>]
type PresentationRegistrations() =
    inherit Module()

    override __.Load(builder:ContainerBuilder) =
#if DEBUG
        assertWith (validateRequired builder "builder" "Builder")
#endif

        ignore (builder.Register(fun _ -> MemberDetailReadModelFactory(MemberDetailReadModel.make)))
        ignore (builder.Register(fun _ -> MemberListItemReadModelFactory(MemberListReadModel.makeItem)))
        ignore (builder.Register(fun _ -> MemberListReadModelFactory(MemberListReadModel.make)))
