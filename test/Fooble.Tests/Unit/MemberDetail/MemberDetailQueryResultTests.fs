namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
open System
 
[<TestFixture>]
module MemberDetailQueryResultTests =
 
    [<Test>]
    let ``Calling make success, with valid parameters, returns query result`` () =
        let readModel = MemberDetail.ReadModel.make <|| (randomGuid (), randomString ())
        let queryResult = MemberDetail.QueryResult.makeSuccess readModel

        test <@ box queryResult :? IMemberDetailQueryResult @>

    [<Test>]
    let ``Calling not found, returns query result`` () =
        let queryResult = MemberDetail.QueryResult.notFound

        test <@ box queryResult :? IMemberDetailQueryResult @>
    
    [<Test>]
    let ``Calling read model, with not found query result, raises expected exception`` () =
        let expectedMessage = "Result was not successful"
        
        let queryResult = MemberDetail.QueryResult.notFound
        raisesWith<InvalidOperationException> <@ queryResult.ReadModel @> (fun e -> <@ e.Message = expectedMessage @>)

    [<Test>]
    let ``Calling read model, with success query result, returns expected read model`` () =
        let expectedReadModel = MemberDetail.ReadModel.make <|| (randomGuid (), randomString ())

        let queryResult = MemberDetail.QueryResult.makeSuccess expectedReadModel

        test <@ queryResult.ReadModel = expectedReadModel @>

    [<Test>]
    let ``Calling is success, with success query result, returns true`` () =
        let readModel = MemberDetail.ReadModel.make <|| (randomGuid (), randomString ())
        let queryResult = MemberDetail.QueryResult.makeSuccess readModel

        test <@ queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with not found query result, returns false`` () =
        let queryResult = MemberDetail.QueryResult.notFound

        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is not found, with success query result, returns false`` () =
        let readModel = MemberDetail.ReadModel.make <|| (randomGuid (), randomString ())
        let queryResult = MemberDetail.QueryResult.makeSuccess readModel

        test <@ not <| queryResult.IsNotFound @>

    [<Test>]
    let ``Calling is not found, with not found query result, returns true`` () =
        let queryResult = MemberDetail.QueryResult.notFound

        test <@ queryResult.IsNotFound @>
