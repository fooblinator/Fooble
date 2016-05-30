namespace Fooble.UnitTest

open Fooble.Presentation
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MessageDisplaySeverityTests =

    [<Test>]
    let ``Calling informational, returns severity`` () =
        let severity = MessageDisplayReadModel.informationalSeverity

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling warning, returns severity`` () =
        let severity = MessageDisplayReadModel.warningSeverity

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling error, returns severity`` () =
        let severity = MessageDisplayReadModel.errorSeverity

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling is informational, with informational severity, returns true`` () =
        let severity = MessageDisplayReadModel.informationalSeverity

        test <@ severity.IsInformational @>

    [<Test>]
    let ``Calling is informational, with warning severity, returns false`` () =
        let severity = MessageDisplayReadModel.warningSeverity

        test <@ not <| severity.IsInformational @>

    [<Test>]
    let ``Calling is informational, with error severity, returns false`` () =
        let severity = MessageDisplayReadModel.errorSeverity

        test <@ not <| severity.IsInformational @>

    [<Test>]
    let ``Calling is warning, with informational severity, returns false`` () =
        let severity = MessageDisplayReadModel.informationalSeverity

        test <@ not <| severity.IsWarning @>

    [<Test>]
    let ``Calling is warning, with warning severity, returns true`` () =
        let severity = MessageDisplayReadModel.warningSeverity

        test <@ severity.IsWarning @>

    [<Test>]
    let ``Calling is warning, with error severity, returns false`` () =
        let severity = MessageDisplayReadModel.errorSeverity

        test <@ not <| severity.IsWarning @>

    [<Test>]
    let ``Calling is error, with informational severity, returns false`` () =
        let severity = MessageDisplayReadModel.informationalSeverity

        test <@ not <| severity.IsError @>

    [<Test>]
    let ``Calling is error, with warning severity, returns false`` () =
        let severity = MessageDisplayReadModel.warningSeverity

        test <@ not <| severity.IsError @>

    [<Test>]
    let ``Calling is error, with error severity, returns true`` () =
        let severity = MessageDisplayReadModel.errorSeverity

        test <@ severity.IsError @>
