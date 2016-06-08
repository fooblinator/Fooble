﻿namespace Fooble.Persistence.Infrastructure

open Autofac
open Fooble.Common
open Fooble.Persistence
open System

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
        ensureOn connectionString "connectionString" [ (String.isNotNullOrEmpty), "Connection string is required" ]
        { ConnectionString = connectionString }

    override this.Load(builder:ContainerBuilder) =
#if DEBUG
        assertWith (validateRequired builder "builder" "Builder")
#endif

        ignore (builder.Register(fun _ ->
            EntityConnection.GetDataContext(this.ConnectionString).DataContext :?> FoobleContext |> wrapFoobleContext))

        ignore (builder.Register(fun _ ->
            MemberDataFactory(fun id username passwordData email nickname ->
                MemberData(Id = id, Username = username, PasswordData = passwordData, Email = email,
                    Nickname = nickname, Registered = DateTime.UtcNow, PasswordChanged = DateTime.UtcNow)
                |> wrapMemberData)))
