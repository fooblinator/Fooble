namespace Fooble.Tests.Unit

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
        ignore <| builder.RegisterModule(AutofacModule(Context = expectedContext))
        let container = builder.Build()
        
        let actualContext = container.Resolve<IFoobleContext>()
        test <@ notIsNull actualContext @>
        test <@ obj.ReferenceEquals(actualContext, expectedContext) @>

    [<Test>]
    let ``Registering autofac container, properly registers expected query handlers`` () =
        let context = Mock.Of<IFoobleContext>()
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = context))
        let container = builder.Build()

        let memberDetailQueryHandler = container.Resolve<IMemberDetailQueryHandler>()
        test <@ notIsNull memberDetailQueryHandler @>

        let memberListQueryHandler = container.Resolve<IMemberListQueryHandler>()
        test <@ notIsNull memberListQueryHandler @>

    [<Test>]
    let ``Registering autofac container, properly registers expected single instance factory`` () =
        let context = Mock.Of<IFoobleContext>()
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = context))
        let container = builder.Build()

        let singleInstanceFactory = container.Resolve<SingleInstanceFactory>()
        test <@ notIsNull singleInstanceFactory @>

        (* test resolution of known request handler types using the factory *)

        let result1 = singleInstanceFactory.Invoke(typeof<IMemberDetailQueryHandler>)
        test <@ notIsNull result1 @>
        test <@ result1 :? IMemberDetailQueryHandler @>

        let result2 = singleInstanceFactory.Invoke(typeof<IMemberListQueryHandler>)
        test <@ notIsNull result2 @>
        test <@ result2 :? IMemberListQueryHandler @>

    [<Test>]
    let ``Registering autofac container, properly registers expected multi instance factory`` () =
        let context = Mock.Of<IFoobleContext>()
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = context))
        let container = builder.Build()

        let multiInstanceFactory = container.Resolve<MultiInstanceFactory>()
        test <@ notIsNull multiInstanceFactory @>

        (* test resolution of known request handler types using the factory *)

        let result1 = multiInstanceFactory.Invoke(typeof<IMemberDetailQueryHandler>)
        test <@ notIsNull result1 @>
        test <@ (Seq.length result1) = 1 @>
        let actualResult1 = (Seq.head result1)
        test <@ actualResult1 :? IMemberDetailQueryHandler @>

        let result2 = multiInstanceFactory.Invoke(typeof<IMemberListQueryHandler>)
        test <@ notIsNull result2 @>
        test <@ (Seq.length result2) = 1 @>
        let actualResult2 = (Seq.head result2)
        test <@ actualResult2 :? IMemberListQueryHandler @>

    [<Test>]
    let ``Registering autofac container, properly registers expected mediator`` () =
        let context = Mock.Of<IFoobleContext>()
        let builder = ContainerBuilder()
        ignore <| builder.RegisterModule(AutofacModule(Context = context))
        let container = builder.Build()

        let mediator = container.Resolve<IMediator>()
        test <@ notIsNull mediator @>
