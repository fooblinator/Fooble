namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberExistsQueryTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        testArgumentException expectedParamName expectedMessage <@ MemberExistsQuery.make Guid.empty @>

    [<Test>]
    let ``Calling make, with valid parameters, returns query`` () =
        let query = MemberExistsQuery.make (Guid.random ())

        box query :? IRequest<IMemberExistsQueryResult> =! true

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let query = MemberExistsQuery.make expectedId

        query.Id =! expectedId
