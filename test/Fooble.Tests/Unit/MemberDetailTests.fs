namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberDetailTests =

    [<Test>]
    let ``Calling make query, with valid parameters, returns query`` () =
        let query = MemberDetail.makeQuery <| randomGuid ()

        test <@ box query :? IMemberDetailQuery @>
        test <@ box query :? IRequest<IMemberDetailQueryResult> @>
