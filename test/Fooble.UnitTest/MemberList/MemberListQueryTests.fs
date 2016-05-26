namespace Fooble.UnitTest.MemberList

open Fooble.Core
open MediatR
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberListQueryTests =

    [<Test>]
    let ``Calling make, returns query`` () =
        let query = MemberList.Query.make ()

        test <@ box query :? IMemberListQuery @>
        test <@ box query :? IRequest<IMemberListQueryResult> @>
