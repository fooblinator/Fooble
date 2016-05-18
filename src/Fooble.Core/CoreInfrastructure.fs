namespace Fooble.Core.Infrastructure

open Autofac
open Autofac.Features.Variance
open Fooble.Core
open Fooble.Core.Persistence
open MediatR
open System.Collections.Generic
open System.Diagnostics

[<AllowNullLiteral>]
type AutofacModule() =
    inherit Autofac.Module()

    [<DefaultValue>]
    val mutable private context:IDataContext
    
    member internal this.Context
        with get () = this.context
        and set (context) =
            Debug.Assert(notIsNull context, "Context parameter was null")
            this.context <- context

    override this.Load(builder:ContainerBuilder) = 
        Debug.Assert(notIsNull builder, "Builder parameter was null")

        (* MediatR *)

        builder.RegisterSource(ContravariantRegistrationSource())

        ignore <| builder.Register(fun ctx ->
                let c = ctx.Resolve<IComponentContext>()
                SingleInstanceFactory(fun t -> c.Resolve(t)))

        ignore <| builder.Register(fun ctx ->
                let c = ctx.Resolve<IComponentContext>()
                MultiInstanceFactory(fun t ->
                    c.Resolve(typedefof<IEnumerable<_>>.MakeGenericType(t)) :?> IEnumerable<obj>))

        ignore <| builder.RegisterType<Mediator>().As<IMediator>()

        (* Fooble *)

        if notIsNull this.Context
            then ignore <| builder.RegisterInstance(this.Context)
            else ignore <| builder.RegisterType<DataContext>().As<IDataContext>()

        ignore <| builder.Register(fun c -> MemberDetail.makeQueryHandler (c.Resolve<IDataContext>()))
            .As<IMemberDetailQueryHandler>()

        ignore <| builder.Register(fun c -> MemberList.makeQueryHandler (c.Resolve<IDataContext>()))
            .As<IMemberListQueryHandler>()
