namespace Fooble.IntegrationTest

open Autofac
open Fooble.Core
open Fooble.Core.Infrastructure
open Fooble.Persistence.Infrastructure
open Fooble.Presentation.Infrastructure
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module RegistrationsTests =

    [<Test>]
    let ``Registering autofac container, properly registers expected query handlers`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let memberDetailQueryHandler =
            container.Resolve<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>()
        isNull memberDetailQueryHandler =! false

        let memberListQueryHandler = container.Resolve<IRequestHandler<IMemberListQuery, IMemberListQueryResult>>()
        isNull memberListQueryHandler =! false

        let memberRegisterCommandHandler =
            container.Resolve<IRequestHandler<IMemberRegisterCommand, IMemberRegisterCommandResult>>()
        isNull memberRegisterCommandHandler =! false

    [<Test>]
    let ``Registering autofac container, properly registers expected single instance factory`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let singleInstanceFactory = container.Resolve<SingleInstanceFactory>()
        isNull singleInstanceFactory =! false

        (* test resolution of known request handler types using the factory *)

        let result =
            singleInstanceFactory.Invoke(typeof<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>)
        isNull result =! false
        result :? IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> =! true

        let result = singleInstanceFactory.Invoke(typeof<IRequestHandler<IMemberListQuery, IMemberListQueryResult>>)
        isNull result =! false
        result :? IRequestHandler<IMemberListQuery, IMemberListQueryResult> =! true

        let result =
            singleInstanceFactory
                .Invoke(typeof<IRequestHandler<IMemberRegisterCommand, IMemberRegisterCommandResult>>)
        isNull result =! false
        result :? IRequestHandler<IMemberRegisterCommand, IMemberRegisterCommandResult> =! true

    [<Test>]
    let ``Registering autofac container, properly registers expected multi instance factory`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore (builder.RegisterModule(CoreRegistrations()))
        ignore (builder.RegisterModule(PersistenceRegistrations(connectionString)))
        ignore (builder.RegisterModule(PresentationRegistrations()))
        use container = builder.Build()

        let multiInstanceFactory = container.Resolve<MultiInstanceFactory>()
        isNull multiInstanceFactory =! false

        (* test resolution of known request handler types using the factory *)

        let result =
            multiInstanceFactory.Invoke(typeof<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>)
        isNull result =! false
        Seq.length result =! 1
        let actualResult = (Seq.head result)
        actualResult :? IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> =! true

        let result = multiInstanceFactory.Invoke(typeof<IRequestHandler<IMemberListQuery, IMemberListQueryResult>>)
        isNull result =! false
        Seq.length result =! 1
        let actualResult = (Seq.head result)
        actualResult :? IRequestHandler<IMemberListQuery, IMemberListQueryResult> =! true

        let result =
            multiInstanceFactory
                .Invoke(typeof<IRequestHandler<IMemberRegisterCommand, IMemberRegisterCommandResult>>)
        isNull result =! false
        Seq.length result =! 1
        let actualResult = (Seq.head result)
        actualResult :? IRequestHandler<IMemberRegisterCommand, IMemberRegisterCommandResult> =! true
