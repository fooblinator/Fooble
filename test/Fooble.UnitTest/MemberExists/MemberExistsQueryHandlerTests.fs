namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberExistsQueryHandlerTests =

    [<Test>]
    let ``Calling handle, with no matching member in data store, returns expected result`` () =
        let id = Guid.random ()

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), includeDeactivated = false)).Returns(false).Verifiable()

        let handler = MemberExistsQuery.makeHandler contextMock.Object

        let query = MemberExistsQuery.make id
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        queryResult.IsNotFound =! true
        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling handle, with matching member in data store, returns expected result`` () =
        let id = Guid.random ()

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x ->
            x.ExistsMemberId(any (), includeDeactivated = false)).Returns(true).Verifiable()

        let handler = MemberExistsQuery.makeHandler contextMock.Object

        let query = MemberExistsQuery.make id
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        queryResult.IsSuccess =! true
        queryResult.IsNotFound =! false
