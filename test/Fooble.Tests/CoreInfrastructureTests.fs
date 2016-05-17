namespace Fooble.Tests

open Autofac
open Fooble.Core
open Fooble.Core.Infrastructure
open MediatR
open Moq
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module AutofacModuleTests =

    [<Test>]
    let ``Registering autofac container, with defaults, properly registers expected data context``() =
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule<AutofacModule>()
        let container = builder.Build()
        
        let context = container.Resolve<IDataContext>()
        test <@ not (isNull context) @>

    [<Test>]
    let ``Registering autofac container, with custom data context, properly registers expected data context``() =
        let customContext = Mock.Of<IDataContext>()

        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = customContext))
        let container = builder.Build()
        
        let context = container.Resolve<IDataContext>()
        test <@ not (isNull context) @>
        test <@ obj.ReferenceEquals(context, customContext) @>

    [<Test>]
    let ``Registering autofac container, properly registers expected query handlers``() =
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule<AutofacModule>()
        let container = builder.Build()

        let memberDetailQueryHandler = container.Resolve<IMemberDetailQueryHandler>()
        test <@ not (isNull memberDetailQueryHandler) @>

        let memberListQueryHandler = container.Resolve<IMemberListQueryHandler>()
        test <@ not (isNull memberListQueryHandler) @>

    [<Test>]
    let ``Registering autofac container, properly registers expected single instance factory``() =
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule<AutofacModule>()
        let container = builder.Build()

        let singleInstanceFactory = container.Resolve<SingleInstanceFactory>()
        test <@ not (isNull singleInstanceFactory) @>

        (* test resolution of known request handler types using the factory *)

        let result1 = singleInstanceFactory.Invoke(typeof<IMemberDetailQueryHandler>)
        test <@ not (isNull result1) @>
        test <@ result1 :? IMemberDetailQueryHandler @>

        let result2 = singleInstanceFactory.Invoke(typeof<IMemberListQueryHandler>)
        test <@ not (isNull result2) @>
        test <@ result2 :? IMemberListQueryHandler @>

    [<Test>]
    let ``Registering autofac container, properly registers expected multi instance factory``() =
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule<AutofacModule>()
        let container = builder.Build()

        let multiInstanceFactory = container.Resolve<MultiInstanceFactory>()
        test <@ not (isNull multiInstanceFactory) @>

        (* test resolution of known request handler types using the factory *)

        let result1 = multiInstanceFactory.Invoke(typeof<IMemberDetailQueryHandler>)
        test <@ not (isNull result1) @>
        test <@ (Seq.length result1) = 1 @>
        let actualResult1 = (Seq.head result1)
        test <@ actualResult1 :? IMemberDetailQueryHandler @>

        let result2 = multiInstanceFactory.Invoke(typeof<IMemberListQueryHandler>)
        test <@ not (isNull result2) @>
        test <@ (Seq.length result2) = 1 @>
        let actualResult2 = (Seq.head result2)
        test <@ actualResult2 :? IMemberListQueryHandler @>

    [<Test>]
    let ``Registering autofac container, properly registers expected mediator``() =
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule<AutofacModule>()
        let container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        test <@ not (isNull mediator) @>
