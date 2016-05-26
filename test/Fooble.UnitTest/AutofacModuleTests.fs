namespace Fooble.UnitTest

open Autofac
open Fooble.Core
open Fooble.Core.Persistence
open Fooble.Core.Infrastructure
open MediatR
open Moq
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module AutofacModuleTests =

    [<Test>]
    let ``Registering autofac container, with custom data context, properly registers expected data context`` () =
        let expectedContext = Mock.Of<IFoobleContext>()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(expectedContext))
        let container = builder.Build()

        let actualContext = container.Resolve<IFoobleContext>()
        test <@ isNotNull actualContext @>
        test <@ obj.ReferenceEquals(actualContext, expectedContext) @>

    [<Test>]
    let ``Registering autofac container, properly registers expected query handlers`` () =
        let context = Mock.Of<IFoobleContext>()
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(context))
        let container = builder.Build()

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
        let context = Mock.Of<IFoobleContext>()
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(context))
        let container = builder.Build()

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
        let context = Mock.Of<IFoobleContext>()
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(context))
        let container = builder.Build()

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

    [<Test>]
    let ``Registering autofac container, properly registers expected mediator`` () =
        let context = Mock.Of<IFoobleContext>()
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(context))
        let container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        test <@ isNotNull mediator @>

    [<Test>]
    let ``Registering autofac container, properly registers expected key generator`` () =
        let context = Mock.Of<IFoobleContext>()
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(context))
        let container = builder.Build()

        let keyGenerator = container.Resolve<IKeyGenerator>()
        test <@ isNotNull <| box keyGenerator @>
