namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
 
[<TestFixture>]
module MemberListQueryResultTests =
 
    [<Test>]
    let ``Calling make success, with valid parameters, returns query result`` () =
        let members = Seq.init 5 <| fun _ -> MemberList.makeItemReadModel <|| (randomGuid (), randomString ())
        let readModel = MemberList.makeReadModel members
        let queryResult = MemberList.makeSuccessResult readModel

        test <@ box queryResult :? IMemberListQueryResult @>

    [<Test>]
    let ``Calling not found, returns query result`` () =
        let queryResult = MemberList.notFoundResult

        test <@ box queryResult :? IMemberListQueryResult @>

    [<Test>]
    let ``Calling read model, with success query result, returns expected read model`` () =
        let members = Seq.init 5 <| fun _ -> MemberList.makeItemReadModel <|| (randomGuid (), randomString ())
        let expectedReadModel = MemberList.makeReadModel members

        let queryResult = MemberList.makeSuccessResult expectedReadModel

        test <@ queryResult.ReadModel = expectedReadModel @>

    [<Test>]
    let ``Calling is success, with success query result, returns true`` () =
        let members = Seq.init 5 <| fun _ -> MemberList.makeItemReadModel <|| (randomGuid (), randomString ())
        let readModel = MemberList.makeReadModel members

        let queryResult = MemberList.makeSuccessResult readModel

        test <@ queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with not found query result, returns false`` () =
        let queryResult = MemberList.notFoundResult

        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is not found, with success query result, returns false`` () =
        let members = Seq.init 5 <| fun _ -> MemberList.makeItemReadModel <|| (randomGuid (), randomString ())
        let readModel = MemberList.makeReadModel members

        let queryResult = MemberList.makeSuccessResult readModel

        test <@ not <| queryResult.IsNotFound @>

    [<Test>]
    let ``Calling is not found, with not found query result, returns true`` () =
        let queryResult = MemberList.notFoundResult

        test <@ queryResult.IsNotFound @>
