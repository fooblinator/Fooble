namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberListQueryResultTests =

    [<Test>]
    let ``Calling make success, with valid parameters, returns query result`` () =
        let members = Seq.init 5 (fun _ -> makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))
        let readModel = makeTestMemberListReadModel members
        let queryResult = MemberListQuery.makeSuccessResult readModel

        test <@ box queryResult :? IMemberListQueryResult @>

    [<Test>]
    let ``Calling not found, returns query result`` () =
        let queryResult = MemberListQuery.notFoundResult

        test <@ box queryResult :? IMemberListQueryResult @>

    [<Test>]
    let ``Calling read model, with not found query result, raises expected exception`` () =
        let expectedMessage = "Result was not successful"

        let queryResult = MemberListQuery.notFoundResult
        raisesWith<InvalidOperationException> <@ queryResult.ReadModel @> (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling read model, with success query result, returns expected read model`` () =
        let members = Seq.init 5 (fun _ -> makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))
        let expectedReadModel = makeTestMemberListReadModel members

        let queryResult = MemberListQuery.makeSuccessResult expectedReadModel

        test <@ queryResult.ReadModel = expectedReadModel @>

    [<Test>]
    let ``Calling is success, with success query result, returns true`` () =
        let members = Seq.init 5 (fun _ -> makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))
        let readModel = makeTestMemberListReadModel members

        let queryResult = MemberListQuery.makeSuccessResult readModel

        test <@ queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with not found query result, returns false`` () =
        let queryResult = MemberListQuery.notFoundResult

        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is not found, with success query result, returns false`` () =
        let members = Seq.init 5 (fun _ -> makeTestMemberListItemReadModel (Guid.random ()) (String.random 64))
        let readModel = makeTestMemberListReadModel members

        let queryResult = MemberListQuery.makeSuccessResult readModel

        test <@ not <| queryResult.IsNotFound @>

    [<Test>]
    let ``Calling is not found, with not found query result, returns true`` () =
        let queryResult = MemberListQuery.notFoundResult

        test <@ queryResult.IsNotFound @>
