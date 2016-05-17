﻿namespace Fooble.Core.Infrastructure

open Autofac
open Autofac.Features.Variance
open Fooble.Core
open Fooble.Core.Persistence
open MediatR
open System
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
            Debug.Assert(notIsNull context, "(AutofacModule.Context) context argument was null value")
            this.context <- context

    override this.Load(builder:ContainerBuilder) = 
        Debug.Assert(notIsNull builder, "(AutofacModule.Load) builder argument was null value")

        match not (isNull this.Context) with
        | true -> ignore <| builder.RegisterInstance(this.Context)
        | _ -> ignore <| builder.RegisterType<DataContext>().As<IDataContext>()

        builder.RegisterSource(ContravariantRegistrationSource())

        ignore <| builder.RegisterAssemblyTypes(base.ThisAssembly).AsClosedTypesOf(typedefof<IRequestHandler<_, _>>)

        ignore <| builder.Register<SingleInstanceFactory>(
            Func<IComponentContext, SingleInstanceFactory>(fun ctx ->
                let c = ctx.Resolve<IComponentContext>()
                SingleInstanceFactory(fun t ->
                    c.Resolve(t))))

        ignore <| builder.Register<MultiInstanceFactory>(
            Func<IComponentContext, MultiInstanceFactory>(fun ctx ->
                let c = ctx.Resolve<IComponentContext>()
                MultiInstanceFactory(fun t ->
                    c.Resolve(typedefof<IEnumerable<_>>.MakeGenericType(t)) :?> IEnumerable<obj>)))

        ignore <| builder.RegisterType<Mediator>().As<IMediator>()
