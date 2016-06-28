namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MessageDisplayReadModelTests =

    [<Test>]
    let ``Calling make informational, with successful parameters, returns expected read model`` () =
        let heading = randomString 32
        let subHeading = randomString 32
        let statusCode = Random().Next(1000)
        let message = randomString 64
        let readModel = MessageDisplayReadModel.makeInformational heading subHeading statusCode message
        testMessageDisplayReadModel readModel heading subHeading statusCode
            MessageDisplayReadModel.informationalSeverity message

    [<Test>]
    let ``Calling make warning, with successful parameters, returns expected read model`` () =
        let heading = randomString 32
        let subHeading = randomString 32
        let statusCode = Random().Next(1000)
        let message = randomString 64
        let readModel = MessageDisplayReadModel.makeWarning heading subHeading statusCode message
        testMessageDisplayReadModel readModel heading subHeading statusCode
            MessageDisplayReadModel.warningSeverity message

    [<Test>]
    let ``Calling make error, with successful parameters, returns expected read model`` () =
        let heading = randomString 32
        let subHeading = randomString 32
        let statusCode = Random().Next(1000)
        let message = randomString 64
        let readModel = MessageDisplayReadModel.makeError heading subHeading statusCode message
        testMessageDisplayReadModel readModel heading subHeading statusCode
            MessageDisplayReadModel.errorSeverity message

    [<Test>]
    let ``Calling informational severity, returns expected state`` () =
        let severity = MessageDisplayReadModel.informationalSeverity
        severity.IsInformational =! true
        severity.IsWarning =! false
        severity.IsError =! false

    [<Test>]
    let ``Calling warning severity, returns expected state`` () =
        let severity = MessageDisplayReadModel.warningSeverity
        severity.IsInformational =! false
        severity.IsWarning =! true
        severity.IsError =! false

    [<Test>]
    let ``Calling error severity, returns expected state`` () =
        let severity = MessageDisplayReadModel.errorSeverity
        severity.IsInformational =! false
        severity.IsWarning =! false
        severity.IsError =! true
