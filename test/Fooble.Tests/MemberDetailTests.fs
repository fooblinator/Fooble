﻿namespace Fooble.Tests

open Fooble.Core
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailQueryTests = 
    [<Test>]
    let ``Calling make query, with null id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be null"
        raisesWith<ArgumentException> <@ MemberDetail.makeQuery null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make query, with empty id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be empty"
        raisesWith<ArgumentException> <@ MemberDetail.makeQuery "" @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make query, with invalid guid formatted id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should be in the proper GUID format"
        raisesWith<ArgumentException> <@ MemberDetail.makeQuery (Helper.randomNonGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make query, with valid parameters, returns expected result``() = 
        let expectedId = Helper.randomGuidString()
        let query = MemberDetail.makeQuery expectedId
        test <@ (box query) :? IMemberDetailQuery @>
        let actualId = query.Id
        test <@ actualId = expectedId @>
    
    [<Test>]
    let ``Calling query id, returns expected id``() = 
        let expectedId = Helper.randomGuidString()
        let query = MemberDetail.makeQuery expectedId
        let actualId = query.Id
        test <@ actualId = expectedId @>

[<TestFixture>]
module MemberDetailReadModelTests = 
    [<Test>]
    let ``Calling make read model, with null id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be null"
        raisesWith<ArgumentException> <@ MemberDetail.makeReadModel null (Helper.randomGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make read model, with empty id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be empty"
        raisesWith<ArgumentException> <@ MemberDetail.makeReadModel "" (Helper.randomGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make read model, with invalid guid formatted id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should be in the proper GUID format"
        raisesWith<ArgumentException> 
            <@ MemberDetail.makeReadModel (Helper.randomNonGuidString()) (Helper.randomGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make read model, with null name, raises expected exception``() = 
        let expectedParamName = "name"
        let expectedMessage = "Name should not be null"
        raisesWith<ArgumentException> <@ MemberDetail.makeReadModel (Helper.randomGuidString()) null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make read model, with empty name, raises expected exception``() = 
        let expectedParamName = "name"
        let expectedMessage = "Name should not be empty"
        raisesWith<ArgumentException> <@ MemberDetail.makeReadModel (Helper.randomGuidString()) "" @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make read model, with valid parameters, returns expected result``() = 
        let expectedId = Helper.randomGuidString()
        let expectedName = Helper.randomGuidString()
        let readModel = MemberDetail.makeReadModel expectedId expectedName
        test <@ (box readModel) :? IMemberDetailReadModel @>
        let actualId = readModel.Id
        test <@ actualId = expectedId @>
        let actualName = readModel.Name
        test <@ actualName = expectedName @>
    
    [<Test>]
    let ``Calling read model id, returns expected id``() = 
        let expectedId = Helper.randomGuidString()
        let readModel = MemberDetail.makeReadModel expectedId (Helper.randomGuidString())
        let actualId = readModel.Id
        test <@ actualId = expectedId @>
    
    [<Test>]
    let ``Calling read model name, returns expected name``() = 
        let expectedName = Helper.randomGuidString()
        let readModel = MemberDetail.makeReadModel (Helper.randomGuidString()) expectedName
        let actualName = readModel.Name
        test <@ actualName = expectedName @>

[<TestFixture>]
module MemberDetailQueryFailureStatusTests = 
    [<Test>]
    let ``Calling not found query failure status, returns expected status``() = 
        let status = MemberDetail.notFoundQueryFailureStatus
        test <@ (box status) :? IMemberDetailQueryFailureStatus @>
        test <@ status.IsNotFound @>
    
    [<Test>]
    let ``Calling query failure status is not found, as not found, returns true``() = 
        let status = MemberDetail.notFoundQueryFailureStatus
        test <@ status.IsNotFound @>

[<TestFixture>]
module MemberDetailQueryHandlerTests = 
    [<Test>]
    let ``Calling make query handler, with null context, raises expected exception``() = 
        let expectedParamName = "context"
        let expectedMessage = "Data context should not be null"
        raisesWith<ArgumentException> <@ MemberDetail.makeQueryHandler null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make query handler, with valid parameters, returns expected result``() = 
        let handler = MemberDetail.makeQueryHandler (mock())
        test <@ (box handler) :? IMemberDetailQueryHandler @>

//    [<Test>]
//    let ``Calling query handler handle, with null query, raises expected exception``() = 
//        let expectedParamName = "query"
//        let expectedMessage = "Query should not be null"
//        let handler = MemberDetailQueryHandler.make (mock())
//        raisesWith<ArgumentException> <@ handler.Handle(null) @> 
//            (fun e -> 
//            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
//    
[<TestFixture>]
module MemberDetailExtensionTests = 
    [<Test>]
    let ``Calling to message display read model, as success result with member detail read model, returns expected result``() = 
        let expectedHeading = "Member Detail Query"
        let expectedSeverity = MessageDisplay.informationalSeverity
        let expectedMessages = [ "Member detail query was successful" ]
        
        let readModel = 
            MemberDetail.makeReadModel (Helper.randomGuidString()) (Helper.randomGuidString())
            |> Result.success
            |> MemberDetail.toMessageDisplayReadModel
        
        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>
