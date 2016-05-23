namespace Fooble.Core.Infrastructure

open Autofac
open Autofac.Features.Variance
open Fooble.Core
open Fooble.Core.Persistence
open MediatR
open System.Collections.Generic
open System.Diagnostics

/// <summary>
/// Provides the proper Autofac container registrations for the Fooble.Core assembly.
/// </summary>
[<AllowNullLiteral>]
type AutofacModule() =
    inherit Autofac.Module()

    [<DefaultValue>]
    val mutable private connectionString:string option

    [<DefaultValue>]
    val mutable private context:IFoobleContext option
    
    /// <summary>
    /// Sets the connection string to use when registering the data context.
    /// </summary>
    /// <param name="value">The connection string to use.</param>
    member this.ConnectionString
        with set(value) =
            Debug.Assert(notIsNull value, "Connection string property was null")
            Debug.Assert(String.notIsEmpty value, "Connection string property was empty")
            this.connectionString <- Some value
    
    member internal this.Context
        with set(value) =
            Debug.Assert(notIsNull value, "Context property was null")
            this.context <- Some value

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

        match this.context with
        | Some x -> builder.RegisterInstance(x).ExternallyOwned()
        | None ->
            match this.connectionString with
            | Some x -> builder.Register(fun _ -> makeFoobleContext <| Some x)
            | None -> builder.Register(fun _ -> makeFoobleContext None)
        |> ignore

        ignore <| builder.Register(fun c -> MemberDetail.QueryHandler.make <| c.Resolve<IFoobleContext>())
            .As<IMemberDetailQueryHandler>()

        ignore <| builder.Register(fun c -> MemberList.QueryHandler.make <| c.Resolve<IFoobleContext>())
            .As<IMemberListQueryHandler>()

        ignore <| builder.Register(fun c -> SelfServiceRegister.CommandHandler.make <| c.Resolve<IFoobleContext>())
            .As<ISelfServiceRegisterCommandHandler>()
