namespace Fooble.Persistence.Infrastructure

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
        ensureOn connectionString "connectionString" [ (String.IsNullOrEmpty >> not), "Connection string is required" ]
        { ConnectionString = connectionString }

    override this.Load(builder:ContainerBuilder) =
#if DEBUG
        assertWith (validateRequired builder "builder" "Builder")
#endif

        ignore (builder.Register(fun _ ->
            EntityConnection.GetDataContext(this.ConnectionString).DataContext :?> FoobleContext |> wrapFoobleContext))

        ignore (builder.Register(fun _ ->
            MemberDataFactory(
                fun id username passwordData email nickname avatarData registeredOn passwordChangedOn deactivatedOn ->
                    let memberData = wrapMemberData (MemberData())
                    memberData.Id <- id
                    memberData.Username <- username
                    memberData.PasswordData <- passwordData
                    memberData.Email <- email
                    memberData.Nickname <- nickname
                    memberData.AvatarData <- avatarData
                    memberData.RegisteredOn <- registeredOn
                    memberData.PasswordChangedOn <- passwordChangedOn
                    memberData.DeactivatedOn <- deactivatedOn
                    memberData)))
