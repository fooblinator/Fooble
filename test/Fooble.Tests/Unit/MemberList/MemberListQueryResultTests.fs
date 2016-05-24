namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote
open System
 
[<TestFixture>]
module MemberListQueryResultTests =
 
    [<Test>]
    let ``Calling make success, with valid parameters, returns query result`` () =
        let members = Seq.init 5 <| fun _ -> MemberList.ItemReadModel.make <|| (randomGuid (), randomString ())
        let readModel = MemberList.ReadModel.make members
        let queryResult = MemberList.QueryResult.makeSuccess readModel

        test <@ box queryResult :? IMemberListQueryResult @>

    [<Test>]
    let ``Calling not found, returns query result`` () =
        let queryResult = MemberList.QueryResult.notFound

        test <@ box queryResult :? IMemberListQueryResult @>
    
    [<Test>]
    let ``Calling read model, with not found query result, raises expected exception`` () =
        let expectedMessage = "Result was not successful"
        
        let queryResult = MemberList.QueryResult.notFound
        raisesWith<InvalidOperationException> <@ queryResult.ReadModel @> (fun e -> <@ e.Message = expectedMessage @>)

    [<Test>]
    let ``Calling read model, with success query result, returns expected read model`` () =
        let members = Seq.init 5 <| fun _ -> MemberList.ItemReadModel.make <|| (randomGuid (), randomString ())
        let expectedReadModel = MemberList.ReadModel.make members

        let queryResult = MemberList.QueryResult.makeSuccess expectedReadModel

        test <@ queryResult.ReadModel = expectedReadModel @>

    [<Test>]
    let ``Calling is success, with success query result, returns true`` () =
        let members = Seq.init 5 <| fun _ -> MemberList.ItemReadModel.make <|| (randomGuid (), randomString ())
        let readModel = MemberList.ReadModel.make members

        let queryResult = MemberList.QueryResult.makeSuccess readModel

        test <@ queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is success, with not found query result, returns false`` () =
        let queryResult = MemberList.QueryResult.notFound

        test <@ not <| queryResult.IsSuccess @>

    [<Test>]
    let ``Calling is not found, with success query result, returns false`` () =
        let members = Seq.init 5 <| fun _ -> MemberList.ItemReadModel.make <|| (randomGuid (), randomString ())
        let readModel = MemberList.ReadModel.make members

        let queryResult = MemberList.QueryResult.makeSuccess readModel

        test <@ not <| queryResult.IsNotFound @>

    [<Test>]
    let ``Calling is not found, with not found query result, returns true`` () =
        let queryResult = MemberList.QueryResult.notFound

        test <@ queryResult.IsNotFound @>
