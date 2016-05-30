namespace Fooble.Persistence.Infrastructure

open Autofac
open Fooble.Common
open Fooble.Persistence

/// Provides the proper Autofac container registrations for the Fooble.Persistence assembly.
[<AllowNullLiteral>]
type PersistenceRegistrations =
    inherit Module

    val private ConnectionString:string
    val private Context:IFoobleContext option
    val private MemberDataFactory:MemberDataFactory option

    /// <summary>
    /// Constructs an instance of the Autofac module for the Fooble.Persistence assembly.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    new(connectionString) =
        assert (String.isNotNullOrEmpty connectionString)
        { ConnectionString = connectionString
          Context = None
          MemberDataFactory = None }

    internal new(context, memberDataFactory) =
        assert (isNotNull context)
        assert (isNotNull memberDataFactory)
        { ConnectionString = String.empty
          Context = Some context
          MemberDataFactory = Some memberDataFactory }

    override this.Load(builder:ContainerBuilder) =
        assert (isNotNull builder)

        match this.Context with
        | Some x -> ignore <| builder.RegisterInstance(x).ExternallyOwned()
        | None -> ignore <| builder.Register(fun _ ->
            EntityConnection.GetDataContext(this.ConnectionString).DataContext :?> FoobleContext |> wrapFoobleContext)

        match this.MemberDataFactory with
        | Some x -> ignore <| builder.RegisterInstance(x).ExternallyOwned()
        | None -> ignore <| builder.Register(fun _ -> MemberDataFactory(fun id username email nickname ->
            MemberData(Id = id, Username = username, Email = email, Nickname = nickname) |> wrapMemberData))
