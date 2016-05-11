namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MemberTests = 
    [<Test>]
    let ``Calling validate id, with null id, returns expected message``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be null"
        let result = Member.validateId null
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate id, with empty id, returns expected message``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should not be empty"
        let result = Member.validateId ""
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate id, with invalid guid formatted id, returns expected message``() = 
        let expectedParamName = "id"
        let expectedMessage = "Id should be in the proper GUID format"
        let result = Member.validateId (Helper.randomNonGuidString())
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate name, with null name, returns expected message``() = 
        let expectedParamName = "name"
        let expectedMessage = "Name should not be null"
        let result = Member.validateName null
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
    
    [<Test>]
    let ``Calling validate name, with empty name, returns expected message``() = 
        let expectedParamName = "name"
        let expectedMessage = "Name should not be empty"
        let result = Member.validateName ""
        test <@ result.IsSome @>
        let actualParamName = result.Value.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Value.Message
        test <@ actualMessage = expectedMessage @>
