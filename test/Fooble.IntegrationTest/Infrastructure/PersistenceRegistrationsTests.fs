namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Persistence
open Fooble.Persistence.Infrastructure
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module PersistenceRegistrationsTests =

    let private testMemberData (actual:IMemberData) id username passwordData email nickname avatarData
        (registeredOn:DateTime) (passwordChangedOn:DateTime) (deactivatedOn:DateTime option) =

        actual.Id =! id
        actual.Username =! username
        actual.PasswordData =! passwordData
        actual.Email =! email
        actual.Nickname =! nickname
        actual.AvatarData =! avatarData
        let actualRegisteredOn = actual.RegisteredOn
        let actualPasswordChangedOn = actual.PasswordChangedOn
        actualRegisteredOn.Date =! registeredOn.Date
        actualPasswordChangedOn.Date =! passwordChangedOn.Date
        actual.DeactivatedOn =! deactivatedOn

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        ignore (PersistenceRegistrations(connectionString))

    [<Test>]
    let ``Constructing, with null connection string, raises expected exception`` () =
        testArgumentException "connectionString" "Connection string is required"
            <@ PersistenceRegistrations(null) @>

    [<Test>]
    let ``Constructing, with empty connection string, raises expected exception`` () =
        testArgumentException "connectionString" "Connection string is required"
            <@ PersistenceRegistrations(String.Empty) @>

    [<Test>]
    let ``Registering autofac container, properly registers fooble context`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()
        let context = box (container.Resolve<IFoobleContext>())
        isNull context =! false

    [<Test>]
    let ``Registering autofac container, properly registers member data factory`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        use container = builder.Build()
        let memberDataFactory = container.Resolve<MemberDataFactory>()
        isNull memberDataFactory =! false
        let id = Guid.NewGuid()
        let username = randomString 32
        let passwordData = randomPassword 32 |> fun x -> Crypto.hash x 100
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let registeredOn = DateTime.UtcNow
        let passwordChangedOn = DateTime.UtcNow
        let deactivatedOn = None
        let memberData =
            memberDataFactory.Invoke(id, username, passwordData, email, nickname, avatarData, registeredOn,
                passwordChangedOn, deactivatedOn)
        testMemberData memberData id username passwordData email nickname avatarData registeredOn passwordChangedOn
            deactivatedOn
