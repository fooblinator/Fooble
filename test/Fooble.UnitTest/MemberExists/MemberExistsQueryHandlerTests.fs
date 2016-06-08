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
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberId(any ())).Returns(false).Verifiable()

        let handler = MemberExistsQuery.makeHandler contextMock.Object

        let query = MemberExistsQuery.make (Guid.random ())
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        queryResult.IsNotFound =! true
        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling handle, with matching member in data store, returns expected result`` () =
        let expectedId = Guid.random ()

        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.ExistsMemberId(any ())).Returns(true).Verifiable()

        let handler = MemberExistsQuery.makeHandler contextMock.Object

        let query = MemberExistsQuery.make expectedId
        let queryResult = handler.Handle(query)

        contextMock.Verify()

        queryResult.IsSuccess =! true
        queryResult.IsNotFound =! false
