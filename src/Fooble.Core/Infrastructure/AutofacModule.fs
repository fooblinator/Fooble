namespace Fooble.Core.Infrastructure

open Autofac
open Autofac.Features.Variance
open Fooble.Core
open Fooble.Core.Persistence
open MediatR
open System.Collections.Generic

/// Provides the proper Autofac container registrations for the Fooble.Core assembly.
[<AllowNullLiteral>]
type AutofacModule =
    inherit Autofac.Module

    val private ConnectionString:string option
    val private Context:IFoobleContext option

    /// Constructs an instance of the Autofac module for the Fooble.Core assembly.
    new() = { ConnectionString = None; Context = None }

    /// <summary>
    /// Constructs an instance of the Autofac module for the Fooble.Core assembly.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    new(connectionString) =
        assert (isNotNull connectionString)
        assert (String.isNotEmpty connectionString)
        { ConnectionString = Some connectionString; Context = None }

    internal new(context) =
        assert (isNotNull context)
        { ConnectionString = None; Context = Some context }

    override this.Load(builder:ContainerBuilder) =
        assert (isNotNull builder)

        (* MediatR *)

        builder.RegisterSource(ContravariantRegistrationSource())

        ignore <| builder.Register(fun x ->
                let y = x.Resolve<IComponentContext>()
                SingleInstanceFactory(fun z -> y.Resolve(z)))

        ignore <| builder.Register(fun x ->
                let y = x.Resolve<IComponentContext>()
                MultiInstanceFactory(fun z ->
                    y.Resolve(typedefof<IEnumerable<_>>.MakeGenericType(z)) :?> IEnumerable<obj>))

        ignore <| builder.RegisterType<Mediator>().As<IMediator>()

        (* Fooble *)

        match this.Context with
        | Some x -> builder.RegisterInstance(x).ExternallyOwned()
        | None ->
            match this.ConnectionString with
            | Some x -> builder.Register(fun _ -> makeFoobleContext <| Some x)
            | None -> builder.Register(fun _ -> makeFoobleContext None)
        |> ignore

        ignore <| builder.Register(fun _ -> KeyGenerator.make ()).As<IKeyGenerator>()

        ignore <| builder.Register(fun x -> MemberDetail.QueryHandler.make <| x.Resolve<IFoobleContext>())
            .As<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>()

        ignore <| builder.Register(fun x -> MemberList.QueryHandler.make <| x.Resolve<IFoobleContext>())
            .As<IRequestHandler<IMemberListQuery, IMemberListQueryResult>>()

        ignore <| builder.Register(fun x -> SelfServiceRegister.CommandHandler.make <| x.Resolve<IFoobleContext>())
            .As<IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult>>()
