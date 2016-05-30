namespace Fooble.Presentation.Infrastructure

open Autofac
open Fooble.Common
open Fooble.Presentation

/// Provides the proper Autofac container registrations for the Fooble.Presentation assembly.
[<AllowNullLiteral>]
type PresentationRegistrations() =
    inherit Module()

    override this.Load(builder:ContainerBuilder) =
        assert (isNotNull builder)

        ignore <| builder.Register(fun _ -> MemberDetailReadModelFactory(MemberDetailReadModel.make))
        ignore <| builder.Register(fun _ -> MemberListItemReadModelFactory(MemberListReadModel.makeItem))
        ignore <| builder.Register(fun _ -> MemberListReadModelFactory(MemberListReadModel.make))
