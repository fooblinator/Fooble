namespace Fooble.UnitTest

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberExistsQueryResultTests =

    [<Test>]
    let ``Calling is success, with success query result, returns true`` () =
        let queryResult = MemberExistsQuery.successResult

        queryResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, with not found query result, returns false`` () =
        let queryResult = MemberExistsQuery.notFoundResult

        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling is not found, with success query result, returns false`` () =
        let queryResult = MemberExistsQuery.successResult

        queryResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, with not found query result, returns true`` () =
        let queryResult = MemberExistsQuery.notFoundResult

        queryResult.IsNotFound =! true
