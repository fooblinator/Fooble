namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module ValidationTests =

    [<Test>]
    let ``Calling make info, with valid parameters, returns expected result``() =
        let expectedParamName = randomGuidString()
        let expectedMessage = randomGuidString()
        let result = Validation.makeInfo expectedParamName expectedMessage
        test <@ (box result) :? IValidationInfo @>
        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling info param name, returns expected param name``() =
        let expectedParamName = randomGuidString()
        let info = Validation.makeInfo expectedParamName (randomGuidString())
        let actualParamName = info.ParamName
        test <@ actualParamName = expectedParamName @>

    [<Test>]
    let ``Calling info message, returns expected message``() =
        let expectedMessage = randomGuidString()
        let info = Validation.makeInfo (randomGuidString()) expectedMessage
        let actualMessage = info.Message
        test <@ actualMessage = expectedMessage @>
