namespace Fooble.Tests.Unit

open Fooble.Core
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListTests =

    [<Test>]
    let ``Calling make query, returns query`` () =
        let query = MemberList.makeQuery ()

        test <@ box query :? IMemberListQuery @>
        test <@ box query :? IRequest<IMemberListQueryResult> @>
