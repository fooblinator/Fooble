namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDetailQueryTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        testArgumentException expectedParamName expectedMessage <@ MemberDetailQuery.make Guid.empty @>

    [<Test>]
    let ``Calling make, with valid parameters, returns query`` () =
        let query = MemberDetailQuery.make (Guid.random ())

        box query :? IRequest<IMemberDetailQueryResult> =! true

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let query = MemberDetailQuery.make expectedId

        query.Id =! expectedId
