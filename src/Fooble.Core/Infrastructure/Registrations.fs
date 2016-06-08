namespace Fooble.Core.Infrastructure

open Autofac
open Autofac.Features.Variance
open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Fooble.Presentation
open MediatR
open System.Collections.Generic

/// Provides the proper Autofac container registrations for the Fooble.Core assembly.
[<AllowNullLiteral>]
type CoreRegistrations =
    inherit Module

    val private Context:IFoobleContext option
    val private MemberDataFactory:MemberDataFactory option

    /// Constructs an instance of the Autofac module for the Fooble.Persistence assembly.
    new() = { Context = None; MemberDataFactory = None }

    /// <summary>
    /// Constructs an instance of the Autofac module for the Fooble.Persistence assembly.
    /// </summary>
    /// <param name="context">The data context to use.</param>
    /// <param name="memberDataFactory">The member data factory to use.</param>
    new(context, memberDataFactory) =
        ensureWith (validateRequired context "context" "Context")
        ensureWith (validateRequired memberDataFactory "memberDataFactory" "Member data factory")
        { Context = Some(context); MemberDataFactory = Some(memberDataFactory) }

    override this.Load(builder:ContainerBuilder) =
#if DEBUG
        assertWith (validateRequired builder "builder" "Builder")
#endif

        (* MediatR *)

        builder.RegisterSource(ContravariantRegistrationSource())

        ignore (builder.Register(fun x ->
            let y = x.Resolve<IComponentContext>()
            SingleInstanceFactory(fun z -> y.Resolve(z))))

        ignore (builder.Register(fun x ->
            let y = x.Resolve<IComponentContext>()
            MultiInstanceFactory(fun z ->
                y.Resolve(typedefof<IEnumerable<_>>.MakeGenericType(z)) :?> IEnumerable<obj>)))

        ignore (builder.RegisterType<Mediator>().As<IMediator>())

        (* Fooble.Core *)

        if this.Context.IsSome then
            ignore (builder.RegisterInstance(this.Context.Value).ExternallyOwned())

        if this.MemberDataFactory.IsSome then
            ignore (builder.RegisterInstance(this.MemberDataFactory.Value).ExternallyOwned())

        ignore (builder.Register(fun _ -> KeyGenerator(fun () -> Guid.random ())))

        ignore (builder.Register(fun x -> MemberChangePasswordCommand.makeHandler (x.Resolve<IFoobleContext>())))

        ignore (builder.Register(fun x ->
            MemberDetailQuery.makeHandler (x.Resolve<IFoobleContext>()) (x.Resolve<MemberDetailReadModelFactory>())))

        ignore (builder.Register(fun x -> MemberExistsQuery.makeHandler (x.Resolve<IFoobleContext>())))

        ignore (builder.Register(fun x ->
            MemberListQuery.makeHandler (x.Resolve<IFoobleContext>()) (x.Resolve<MemberListItemReadModelFactory>())
                (x.Resolve<MemberListReadModelFactory>())))

        ignore (builder.Register(fun x ->
            MemberRegisterCommand.makeHandler (x.Resolve<IFoobleContext>()) (x.Resolve<MemberDataFactory>())))
