namespace Fooble.UnitTest

open Fooble.Core
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListQueryTests =

    [<Test>]
    let ``Calling make, returns query`` () =
        let query = MemberListQuery.make ()

        test <@ box query :? IMemberListQuery @>
        test <@ box query :? IRequest<IMemberListQueryResult> @>
