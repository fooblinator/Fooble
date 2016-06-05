namespace Fooble.UnitTest

open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MessageDisplaySeverityTests =

    [<Test>]
    let ``Calling informational, returns severity`` () =
        let severity = MessageDisplayReadModel.informationalSeverity

        box severity :? IMessageDisplaySeverity =! true

    [<Test>]
    let ``Calling warning, returns severity`` () =
        let severity = MessageDisplayReadModel.warningSeverity

        box severity :? IMessageDisplaySeverity =! true

    [<Test>]
    let ``Calling error, returns severity`` () =
        let severity = MessageDisplayReadModel.errorSeverity

        box severity :? IMessageDisplaySeverity =! true

    [<Test>]
    let ``Calling is informational, with informational severity, returns true`` () =
        let severity = MessageDisplayReadModel.informationalSeverity

        severity.IsInformational =! true

    [<Test>]
    let ``Calling is informational, with warning severity, returns false`` () =
        let severity = MessageDisplayReadModel.warningSeverity

        severity.IsInformational =! false

    [<Test>]
    let ``Calling is informational, with error severity, returns false`` () =
        let severity = MessageDisplayReadModel.errorSeverity

        severity.IsInformational =! false

    [<Test>]
    let ``Calling is warning, with informational severity, returns false`` () =
        let severity = MessageDisplayReadModel.informationalSeverity

        severity.IsWarning =! false

    [<Test>]
    let ``Calling is warning, with warning severity, returns true`` () =
        let severity = MessageDisplayReadModel.warningSeverity

        severity.IsWarning =! true

    [<Test>]
    let ``Calling is warning, with error severity, returns false`` () =
        let severity = MessageDisplayReadModel.errorSeverity

        severity.IsWarning =! false

    [<Test>]
    let ``Calling is error, with informational severity, returns false`` () =
        let severity = MessageDisplayReadModel.informationalSeverity

        severity.IsError =! false

    [<Test>]
    let ``Calling is error, with warning severity, returns false`` () =
        let severity = MessageDisplayReadModel.warningSeverity

        severity.IsError =! false

    [<Test>]
    let ``Calling is error, with error severity, returns true`` () =
        let severity = MessageDisplayReadModel.errorSeverity

        severity.IsError =! true
