namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open MediatR
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailQueryTests =

    [<Test>]
    let ``Calling make, with empty id, raises expected exception`` () =
        let expectedParamName = "id"
        let expectedMessage = "Id parameter was an empty GUID"

        raisesWith<ArgumentException>
            <@ MemberDetail.Query.make Guid.empty @> (fun x ->
                <@ x.ParamName = expectedParamName && (fixInvalidArgMessage x.Message) = expectedMessage @>)

    [<Test>]
    let ``Calling make, with valid parameters, returns query`` () =
        let query = MemberDetail.Query.make (Guid.random ())

        test <@ box query :? IMemberDetailQuery @>
        test <@ box query :? IRequest<IMemberDetailQueryResult> @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = Guid.random ()

        let query = MemberDetail.Query.make expectedId

        test <@ query.Id = expectedId @>
