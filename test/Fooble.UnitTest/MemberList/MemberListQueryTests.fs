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

        box query :? IMemberListQuery =! true
        box query :? IRequest<IMemberListQueryResult> =! true
