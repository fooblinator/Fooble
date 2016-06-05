namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailQueryResultTests =

    [<Test>]
    let ``Calling make success, with valid parameters, returns query result`` () =
        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32)
                (String.random 64) DateTime.Now DateTime.Now
        let queryResult = MemberDetailQuery.makeSuccessResult readModel

        box queryResult :? IMemberDetailQueryResult =! true

    [<Test>]
    let ``Calling not found, returns query result`` () =
        let queryResult = MemberDetailQuery.notFoundResult

        box queryResult :? IMemberDetailQueryResult =! true

    [<Test>]
    let ``Calling read model, with not found query result, raises expected exception`` () =
        let expectedMessage = "Result was not successful"

        let queryResult = MemberDetailQuery.notFoundResult
        raisesWith<InvalidOperationException> <@ queryResult.ReadModel @> (fun x -> <@ x.Message = expectedMessage @>)

    [<Test>]
    let ``Calling read model, with success query result, returns expected read model`` () =
        let expectedReadModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32)
                (String.random 64) DateTime.Now DateTime.Now

        let queryResult = MemberDetailQuery.makeSuccessResult expectedReadModel

        queryResult.ReadModel =! expectedReadModel

    [<Test>]
    let ``Calling is success, with success query result, returns true`` () =
        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32)
                (String.random 64) DateTime.Now DateTime.Now
        let queryResult = MemberDetailQuery.makeSuccessResult readModel

        queryResult.IsSuccess =! true

    [<Test>]
    let ``Calling is success, with not found query result, returns false`` () =
        let queryResult = MemberDetailQuery.notFoundResult

        queryResult.IsSuccess =! false

    [<Test>]
    let ``Calling is not found, with success query result, returns false`` () =
        let readModel =
            makeTestMemberDetailReadModel (Guid.random ()) (String.random 32) (EmailAddress.random 32)
                (String.random 64) DateTime.Now DateTime.Now
        let queryResult = MemberDetailQuery.makeSuccessResult readModel

        queryResult.IsNotFound =! false

    [<Test>]
    let ``Calling is not found, with not found query result, returns true`` () =
        let queryResult = MemberDetailQuery.notFoundResult

        queryResult.IsNotFound =! true
