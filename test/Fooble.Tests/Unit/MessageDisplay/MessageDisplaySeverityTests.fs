namespace Fooble.Tests.Unit

open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module MessageDisplaySeverityTests =

    [<Test>]
    let ``Calling informational, returns severity`` () =
        let severity = MessageDisplay.Severity.informational

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling warning, returns severity`` () =
        let severity = MessageDisplay.Severity.warning

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling error, returns severity`` () =
        let severity = MessageDisplay.Severity.error

        test <@ box severity :? IMessageDisplaySeverity @>

    [<Test>]
    let ``Calling is informational, with informational severity, returns true`` () =
        let severity = MessageDisplay.Severity.informational

        test <@ severity.IsInformational @>

    [<Test>]
    let ``Calling is informational, with warning severity, returns false`` () =
        let severity = MessageDisplay.Severity.warning

        test <@ not <| severity.IsInformational @>

    [<Test>]
    let ``Calling is informational, with error severity, returns false`` () =
        let severity = MessageDisplay.Severity.error

        test <@ not <| severity.IsInformational @>

    [<Test>]
    let ``Calling is warning, with informational severity, returns false`` () =
        let severity = MessageDisplay.Severity.informational

        test <@ not <| severity.IsWarning @>

    [<Test>]
    let ``Calling is warning, with warning severity, returns true`` () =
        let severity = MessageDisplay.Severity.warning

        test <@ severity.IsWarning @>

    [<Test>]
    let ``Calling is warning, with error severity, returns false`` () =
        let severity = MessageDisplay.Severity.error

        test <@ not <| severity.IsWarning @>

    [<Test>]
    let ``Calling is error, with informational severity, returns false`` () =
        let severity = MessageDisplay.Severity.informational

        test <@ not <| severity.IsError @>

    [<Test>]
    let ``Calling is error, with warning severity, returns false`` () =
        let severity = MessageDisplay.Severity.warning

        test <@ not <| severity.IsError @>

    [<Test>]
    let ``Calling is error, with error severity, returns true`` () =
        let severity = MessageDisplay.Severity.error

        test <@ severity.IsError @>
