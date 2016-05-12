namespace Fooble.Tests

open Fooble.Core
open Foq
open MediatR
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailQueryFailureStatusTests = 
    [<Test>]
    let ``Calling not found, returns expected status``() = 
        let status = MemberDetailQueryFailureStatus.notFound
        test <@ (box status) :? IMemberDetailQueryFailureStatus @>
        test <@ status.IsNotFound @>
    
    [<Test>]
    let ``Calling is not found, with not found status, returns true``() = 
        let status = MemberDetailQueryFailureStatus.notFound
        test <@ status.IsNotFound @>

[<TestFixture>]
module MemberDetailReadModelTests = 
    [<Test>]
    let ``Calling make, with null id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be null"
        raisesWith<ArgumentException> <@ MemberDetailReadModel.make null (Helper.randomGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with empty id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be empty"
        raisesWith<ArgumentException> <@ MemberDetailReadModel.make "" (Helper.randomGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with invalid guid formatted id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should be in the proper GUID format"
        raisesWith<ArgumentException> 
            <@ MemberDetailReadModel.make (Helper.randomNonGuidString()) (Helper.randomGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with null name, raises expected exception``() = 
        let expectedParamName = "name"
        let expectedMessage = "Name should not be null"
        raisesWith<ArgumentException> <@ MemberDetailReadModel.make (Helper.randomGuidString()) null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with empty name, raises expected exception``() = 
        let expectedParamName = "name"
        let expectedMessage = "Name should not be empty"
        raisesWith<ArgumentException> <@ MemberDetailReadModel.make (Helper.randomGuidString()) "" @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() = 
        let expectedId = Helper.randomGuidString()
        let expectedName = Helper.randomGuidString()
        let readModel = MemberDetailReadModel.make expectedId expectedName
        test <@ (box readModel) :? IMemberDetailReadModel @>
        let actualId = readModel.Id
        test <@ actualId = expectedId @>
        let actualName = readModel.Name
        test <@ actualName = expectedName @>
    
    [<Test>]
    let ``Calling id, returns expected id``() = 
        let expectedId = Helper.randomGuidString()
        let readModel = MemberDetailReadModel.make expectedId (Helper.randomGuidString())
        let actualId = readModel.Id
        test <@ actualId = expectedId @>
    
    [<Test>]
    let ``Calling name, returns expected name``() = 
        let expectedName = Helper.randomGuidString()
        let readModel = MemberDetailReadModel.make (Helper.randomGuidString()) expectedName
        let actualName = readModel.Name
        test <@ actualName = expectedName @>

[<TestFixture>]
module MemberDetailQueryTests = 
    [<Test>]
    let ``Calling make, with null id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be null"
        raisesWith<ArgumentException> <@ MemberDetailQuery.make null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with empty id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be empty"
        raisesWith<ArgumentException> <@ MemberDetailQuery.make "" @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with invalid guid formatted id, raises expected exception``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should be in the proper GUID format"
        raisesWith<ArgumentException> <@ MemberDetailQuery.make (Helper.randomNonGuidString()) @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() = 
        let expectedId = Helper.randomGuidString()
        let query = MemberDetailQuery.make expectedId
        test <@ (box query) :? IMemberDetailQuery @>
        let actualId = query.Id
        test <@ actualId = expectedId @>
    
    [<Test>]
    let ``Calling id, returns expected id``() = 
        let expectedId = Helper.randomGuidString()
        let query = MemberDetailQuery.make expectedId
        let actualId = query.Id
        test <@ actualId = expectedId @>

[<TestFixture>]
module MemberDetailQueryHandlerTests = 
    [<Test>]
    let ``Calling make, with null context, raises expected exception``() = 
        let expectedParamName = "context"
        let expectedMessage = "Data context should not be null"
        raisesWith<ArgumentException> <@ MemberDetailQueryHandler.make null @> 
            (fun e -> 
            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
    
    [<Test>]
    let ``Calling make, with valid parameters, returns expected result``() = 
        let handler = Mock<IDataContext>().Create() |> MemberDetailQueryHandler.make
        test <@ (box handler) :? IMemberDetailQueryHandler @>

//    [<Test>]
//    let ``Calling handle, with null query, raises expected exception``() = 
//        let expectedParamName = "query"
//        let expectedMessage = "Query should not be null"
//        let handler = Mock<IDataContext>().Create() |> MemberDetailQueryHandler.make
//        raisesWith<ArgumentException> <@ handler.Handle(null) @> 
//            (fun e -> 
//            <@ e.ParamName = expectedParamName && (Helper.fixArgumentExceptionMessage e.Message) = expectedMessage @>)
//    
[<TestFixture>]
module MemberDetailExtensionTests = 
    [<Test>]
    let ``Calling to message display read model, with success result, returns expected result``() = 
        let expectedHeading = "Member Detail Query"
        let expectedSeverity = MessageDisplaySeverity.informational
        let expectedMessages = [ "Member detail query was successful" ]
        
        let readModel = 
            MemberDetailReadModel.make (Helper.randomGuidString()) (Helper.randomGuidString())
            |> Result.success
            |> MemberDetailExtensions.toMessageDisplayReadModel
        
        let actualHeading = readModel.Heading
        test <@ actualHeading = expectedHeading @>
        let actualSeverity = readModel.Severity
        test <@ actualSeverity = expectedSeverity @>
        let actualMessages = List.ofSeq readModel.Messages
        test <@ actualMessages = expectedMessages @>
