namespace Fooble.Core

open System.Diagnostics
open System.Runtime.CompilerServices

[<AutoOpen; Extension>]
type CoreExtensions =

    (* Member Detail *)

    [<Extension>]
    static member ToMessageDisplayReadModel(result:IMemberDetailQueryResult) =
        Validation.raiseIfInvalid <| Validation.validate result "result" [ notIsNull << box, "Result parameter was null" ]
        let h = "Member Detail Query"
        let ss = MessageDisplay.informationalSeverity
        let sm = "Member detail query was successful"
        let fs = MessageDisplay.errorSeverity
        let fm = "Member detail query was not successful and returned not found"
        match result with
        | MemberDetail.IsSuccess _ -> MessageDisplay.makeReadModel h ss (Seq.singleton sm)
        | MemberDetail.IsNotFound -> MessageDisplay.makeReadModel h fs (Seq.singleton fm)

    (* Member List *)

    [<Extension>]
    static member ToMessageDisplayReadModel(result:IMemberListQueryResult) =
        Validation.raiseIfInvalid <| Validation.validate result "result" [ notIsNull << box, "Result parameter was null" ]
        let h = "Member List Query"
        let ss = MessageDisplay.informationalSeverity
        let sm = "Member list query was successful"
        let fs = MessageDisplay.errorSeverity
        let fm = "Member list query was not successful and returned not found"
        match result with
        | MemberList.IsSuccess _ -> MessageDisplay.makeReadModel h ss (Seq.singleton sm)
        | MemberList.IsNotFound -> MessageDisplay.makeReadModel h fs (Seq.singleton fm)
