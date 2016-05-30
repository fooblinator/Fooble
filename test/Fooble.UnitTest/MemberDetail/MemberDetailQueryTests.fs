namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.UnitTest
open MediatR
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailQueryTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id is required"

        raisesWith<ArgumentException>
            <@ MemberDetailQuery.make Guid.empty @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with valid parameters, returns query`` () =
        let query = MemberDetailQuery.make (Guid.random ())

        test <@ box query :? IMemberDetailQuery @>
        test <@ box query :? IRequest<IMemberDetailQueryResult> @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let query = MemberDetailQuery.make expectedId

        test <@ query.Id = expectedId @>
