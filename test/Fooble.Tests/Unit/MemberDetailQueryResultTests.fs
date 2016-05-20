namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
 
[<TestFixture>]
module MemberDetailQueryResultTests =
 
    [<Test>]
    let ``Calling make success, with valid parameters, returns query result`` () =
        let readModel = MemberDetail.makeReadModel <|| (randomGuid (), randomString ())
        let queryResult = MemberDetail.makeSuccessResult readModel

        test <@ box queryResult :? IMemberDetailQueryResult @>

    [<Test>]
    let ``Calling not found, returns query result`` () =
        let queryResult = MemberDetail.notFoundResult

        test <@ box queryResult :? IMemberDetailQueryResult @>

    [<Test>]
    let ``Calling read model, with success query result, returns expected read model`` () =
        let expectedReadModel = MemberDetail.makeReadModel <|| (randomGuid (), randomString ())

        let queryResult = MemberDetail.makeSuccessResult expectedReadModel

        test <@ queryResult.ReadModel = expectedReadModel @>

    [<Test>]
    let ``Calling is success, with success query result, returns true`` () =
        let readModel = MemberDetail.makeReadModel <|| (randomGuid (), randomString ())
        let queryResult = MemberDetail.makeSuccessResult readModel

        test <@ queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with not found query result, returns false`` () =
        let queryResult = MemberDetail.notFoundResult

        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is not found, with success query result, returns false`` () =
        let readModel = MemberDetail.makeReadModel <|| (randomGuid (), randomString ())
        let queryResult = MemberDetail.makeSuccessResult readModel

        test <@ not <| queryResult.IsNotFound @>

    [<Test>]
    let ``Calling is not found, with not found query result, returns true`` () =
        let queryResult = MemberDetail.notFoundResult

        test <@ queryResult.IsNotFound @>
