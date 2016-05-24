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
type AutofacModule =
    inherit Autofac.Module

    val private ConnectionString:string option
    val private Context:IFoobleContext option

    /// <summary>
    /// Constructs an instance of the Autofac module for the Fooble.Core assembly.
    /// </summary>
    new() = 
        { ConnectionString = None
          Context = None }

    /// <summary>
    /// Constructs an instance of the Autofac module for the Fooble.Core assembly.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    new(connectionString) =
        Debug.Assert(notIsNull connectionString, "Connection string parameter was null")
        Debug.Assert(String.notIsEmpty connectionString, "Connection string parameter was an empty string")
        { ConnectionString = Some connectionString
          Context = None }

    internal new(context) =
        Debug.Assert(notIsNull context, "Context parameter was null")
        { ConnectionString = None;
          Context = Some context }

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

        match this.Context with
        | Some x -> builder.RegisterInstance(x).ExternallyOwned()
        | None ->
            match this.ConnectionString with
            | Some x -> builder.Register(fun _ -> makeFoobleContext <| Some x)
            | None -> builder.Register(fun _ -> makeFoobleContext None)
        |> ignore

        ignore <| builder.Register(fun c -> KeyGenerator.make ()).As<IKeyGenerator>()

        ignore <| builder.Register(fun c -> MemberDetail.QueryHandler.make <| c.Resolve<IFoobleContext>())
            .As<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>()

        ignore <| builder.Register(fun c -> MemberList.QueryHandler.make <| c.Resolve<IFoobleContext>())
            .As<IRequestHandler<IMemberListQuery, IMemberListQueryResult>>()

        ignore <| builder.Register(fun c -> SelfServiceRegister.CommandHandler.make <| c.Resolve<IFoobleContext>())
            .As<IRequestHandler<ISelfServiceRegisterCommand, Unit>>()
