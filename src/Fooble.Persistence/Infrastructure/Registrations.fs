namespace Fooble.Persistence.Infrastructure

open Autofac
open Fooble.Common
open Fooble.Persistence

/// Provides the proper Autofac container registrations for the Fooble.Persistence assembly.
[<AllowNullLiteral>]
type PersistenceRegistrations =
    inherit Module

    val private ConnectionString:string

    /// <summary>
    /// Constructs an instance of the Autofac module for the Fooble.Persistence assembly.
    /// </summary>
    /// <param name="connectionString">The connection string to use.</param>
    new(connectionString) =
        [ (String.isNotNullOrEmpty), "Connection string is required" ]
        |> validate connectionString "connectionString" |> enforce
        { ConnectionString = connectionString }

    override this.Load(builder:ContainerBuilder) =
        assert (isNotNull builder)

        ignore <| builder.Register(fun _ ->
            EntityConnection.GetDataContext(this.ConnectionString).DataContext :?> FoobleContext |> wrapFoobleContext)

        ignore <| builder.Register(fun _ ->
            MemberDataFactory(fun id username password email nickname ->
                MemberData(Id = id, Username = username, Password = password, Email = email, Nickname = nickname)
                |> wrapMemberData))
