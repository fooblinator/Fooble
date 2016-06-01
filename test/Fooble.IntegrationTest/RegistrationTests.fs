namespace Fooble.IntegrationTest

open Autofac
open Fooble.Common
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
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let memberDetailQueryHandler =
            container.Resolve<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>()
        test <@ isNotNull memberDetailQueryHandler @>

        let memberListQueryHandler = container.Resolve<IRequestHandler<IMemberListQuery, IMemberListQueryResult>>()
        test <@ isNotNull memberListQueryHandler @>

        let selfServiceRegisterCommandHandler =
            container.Resolve<IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult>>()
        test <@ isNotNull selfServiceRegisterCommandHandler @>

    [<Test>]
    let ``Registering autofac container, properly registers expected single instance factory`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let singleInstanceFactory = container.Resolve<SingleInstanceFactory>()
        test <@ isNotNull singleInstanceFactory @>

        (* test resolution of known request handler types using the factory *)

        let result1 =
            singleInstanceFactory.Invoke(typeof<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>)
        test <@ isNotNull result1 @>
        test <@ result1 :? IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> @>

        let result2 = singleInstanceFactory.Invoke(typeof<IRequestHandler<IMemberListQuery, IMemberListQueryResult>>)
        test <@ isNotNull result2 @>
        test <@ result2 :? IRequestHandler<IMemberListQuery, IMemberListQueryResult> @>

        let result3 =
            singleInstanceFactory
                .Invoke(typeof<IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult>>)
        test <@ isNotNull result3 @>
        test <@ result3 :? IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult> @>

    [<Test>]
    let ``Registering autofac container, properly registers expected multi instance factory`` () =
        let connectionString = Settings.ConnectionStrings.FoobleContext
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(CoreRegistrations())
        ignore <| builder.RegisterModule(PersistenceRegistrations(connectionString))
        ignore <| builder.RegisterModule(PresentationRegistrations())
        use container = builder.Build()

        let multiInstanceFactory = container.Resolve<MultiInstanceFactory>()
        test <@ isNotNull multiInstanceFactory @>

        (* test resolution of known request handler types using the factory *)

        let result1 =
            multiInstanceFactory.Invoke(typeof<IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>>)
        test <@ isNotNull result1 @>
        test <@ (Seq.length result1) = 1 @>
        let actualResult1 = (Seq.head result1)
        test <@ actualResult1 :? IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult> @>

        let result2 = multiInstanceFactory.Invoke(typeof<IRequestHandler<IMemberListQuery, IMemberListQueryResult>>)
        test <@ isNotNull result2 @>
        test <@ (Seq.length result2) = 1 @>
        let actualResult2 = (Seq.head result2)
        test <@ actualResult2 :? IRequestHandler<IMemberListQuery, IMemberListQueryResult> @>

        let result3 =
            multiInstanceFactory
                .Invoke(typeof<IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult>>)
        test <@ isNotNull result3 @>
        test <@ (Seq.length result3) = 1 @>
        let actualResult3 = (Seq.head result3)
        test <@ actualResult3 :? IRequestHandler<ISelfServiceRegisterCommand, ISelfServiceRegisterCommandResult> @>
