namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDetailQueryTests =

    [<Test>]
    let ``Calling make, with valid parameters, returns query`` () =
        let query = MemberDetail.makeQuery <| randomGuid ()

        test <@ box query :? IMemberDetailQuery @>
        test <@ box query :? IRequest<IMemberDetailQueryResult> @>

    [<Test>]
    let ``Calling id, returns expected id`` () =
        let expectedId = randomGuid ()

        let query = MemberDetail.makeQuery expectedId

        let actualId = query.Id
        test <@ actualId = expectedId @>
