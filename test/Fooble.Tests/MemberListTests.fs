namespace Fooble.Tests

open Fooble.Core
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberListQueryFailureStatusTests = 
    [<Test>]
    let ``Calling not found, returns expected status``() = 
        let status = MemberList.notFoundQueryFailureStatus
        test <@ (box status) :? IMemberListQueryFailureStatus @>
        test <@ status.IsNotFound @>
    
    [<Test>]
    let ``Calling is not found, with not found status, returns true``() = 
        let status = MemberList.notFoundQueryFailureStatus
        test <@ status.IsNotFound @>

[<TestFixture>]
module MemberListItemReadModelTests = 
    [<Test>]
    let ``Calling make, with null id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be null"
        raisesWith<ArgumentException> <@ MemberList.makeItemReadModel null (Helper.randomGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with empty id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be empty"
        raisesWith<ArgumentException> <@ MemberList.makeItemReadModel "" (Helper.randomGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with invalid guid formatted id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should be in the proper GUID format"
        raisesWith<ArgumentException> 
            <@ MemberList.makeItemReadModel (Helper.randomNonGuidString()) (Helper.randomGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with null name, raises expected exception``() = 
        let expectedParamName = "name"
        let expectedMessage = "Name should not be null"
        raisesWith<ArgumentException> <@ MemberList.makeItemReadModel (Helper.randomGuidString()) null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with empty name, raises expected exception``() = 
        let expectedParamName = "name"
        let expectedMessage = "Name should not be empty"
        raisesWith<ArgumentException> <@ MemberList.makeItemReadModel (Helper.randomGuidString()) "" @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() = 
        let expectedId = Helper.randomGuidString()
        let expectedName = Helper.randomGuidString()
        let readModel = MemberList.makeItemReadModel expectedId expectedName
        test <@ (box readModel) :? IMemberListItemReadModel @>
        let actualId = readModel.Id
        test <@ actualId = expectedId @>
        let actualName = readModel.Name
        test <@ actualName = expectedName @>
    
    [<Test>]
    let ``Calling id, returns expected id``() = 
        let expectedId = Helper.randomGuidString()
        let readModel = MemberList.makeItemReadModel expectedId (Helper.randomGuidString())
        let actualId = readModel.Id
        test <@ actualId = expectedId @>
    
    [<Test>]
    let ``Calling name, returns expected name``() = 
        let expectedName = Helper.randomGuidString()
        let readModel = MemberList.makeItemReadModel (Helper.randomGuidString()) expectedName
        let actualName = readModel.Name
        test <@ actualName = expectedName @>

[<TestFixture>]
module MemberListReadModelTests = 
    [<Test>]
    let ``Calling make, with null members, raises expected exception``() = 
        let expectedParamName = "members"
        let expectedMember = "Member list should not be null"
        raisesWith<ArgumentException> <@ MemberList.makeReadModel null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMember @>)
    
    [<Test>]
    let ``Calling make, with empty members, raises expected exception``() = 
        let expectedParamName = "members"
        let expectedMember = "Member list should not be empty"
        raisesWith<ArgumentException> <@ MemberList.makeReadModel Seq.empty @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMember @>)
    
    //[<Test>]
    //let ``Calling make, with not empty members containing null member, raises expected exception``() = 
    //    let expectedParamName = "members"
    //    let expectedMember = "Member list items should not be null"
    //    raisesWith<ArgumentException> <@ MemberListReadModel.make (Seq.singleton null) @> 
    //        (fun e -> 
    //        <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMember @>)
    //    
    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() = 
        let expectedMembers = [ MemberList.makeItemReadModel (Helper.randomGuidString()) (Helper.randomGuidString()) ]
        let readModel = MemberList.makeReadModel (Seq.ofList expectedMembers)
        test <@ (box readModel) :? IMemberListReadModel @>
        let actualMembers = List.ofSeq readModel.Members
        test <@ actualMembers = expectedMembers @>
    
    [<Test>]
    let ``Calling members, returns expected members``() = 
        let expectedMembers = [ MemberList.makeItemReadModel (Helper.randomGuidString()) (Helper.randomGuidString()) ]
        let readModel = MemberList.makeReadModel (Seq.ofList expectedMembers)
        let actualMembers = List.ofSeq readModel.Members
        test <@ actualMembers = expectedMembers @>

[<TestFixture>]
module MemberListQueryTests = 
    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() = 
        let query = MemberList.makeQuery()
        test <@ (box query) :? IMemberListQuery @>

[<TestFixture>]
module MemberListQueryHandlerTests = 
    [<Test>]
    let ``Calling make, with null context, raises expected exception``() = 
        let expectedParamName = "context"
        let expectedMessage = "Data context should not be null"
        raisesWith<ArgumentException> <@ MemberList.makeQueryHandler null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() = 
        let handler = MemberList.makeQueryHandler (mock())
        test <@ (box handler) :? IMemberListQueryHandler @>

//    [<Test>]
//    let ``Calling handle, with null query, raises expected exception``() = 
//        let expectedParamName = "query"
//        let expectedMessage = "Query should not be null"
//        let handler = MemberListQueryHandler.make (mock())
//        raisesWith<ArgumentException> <@ handler.Handle(null) @> 
//            (fun e -> 
//            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
//    
[<TestFixture>]
module MemberListExtensionTests = 
    [<Test>]
    let ``Calling to message display read model, with success result, returns expected result``() = 
        let expectedHeading = "Member List Query"
        let expectedSeverity = MessageDisplay.informationalSeverity
        let expectedMessages = [ "Member list query was successful" ]
        
        let readModel = 
            [ MemberList.makeItemReadModel (Helper.randomGuidString()) (Helper.randomGuidString()) ]
            |> MemberList.makeReadModel
            |> Result.success
            |> MemberList.toMessageDisplayReadModel
        
        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>
