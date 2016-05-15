namespace Fooble.Tests

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module ValidationInfoTests =

    [<Test>]
    let ``Calling make info, with valid parameters, returns expected result``() =
        let expectedParamName = randomGuidString()
        let expectedMessage = randomGuidString()

        let result = makeValidationInfo expectedParamName expectedMessage

        let actualParamName = result.ParamName
        test <@ actualParamName = expectedParamName @>
        let actualMessage = result.Message
        test <@ actualMessage = expectedMessage @>

    [<Test>]
    let ``Calling info param name, returns expected param name``() =
        let expectedParamName = randomGuidString()

        let info = makeValidationInfo expectedParamName (randomGuidString())

        let actualParamName = info.ParamName
        test <@ actualParamName = expectedParamName @>

    [<Test>]
    let ``Calling info message, returns expected message``() =
        let expectedMessage = randomGuidString()

        let info = makeValidationInfo (randomGuidString()) expectedMessage

        let actualMessage = info.Message
        test <@ actualMessage = expectedMessage @>
