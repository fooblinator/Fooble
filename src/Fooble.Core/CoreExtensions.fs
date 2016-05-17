namespace Fooble.Core

open System.Runtime.CompilerServices

[<AutoOpen; Extension>]
type CoreExtensions =

    (* Extensions *)

    [<Extension>]
    static member ToMessageDisplayReadModel(result:IResult<IMemberDetailReadModel,IMemberDetailQueryFailureStatus>) =
        raiseIfInvalid <| validate result "result" [ notIsNull, "Result was null" ]
        let h = "Member Detail Query"
        let ss = MessageDisplay.Severity.informational
        let sm = "Member detail query was successful"
        let fs = MessageDisplay.Severity.error
        let fm = "Member detail query was not successful and returned not found"
        match result with
        | IsSuccess _ -> MessageDisplay.ReadModel.make h ss (Seq.singleton sm)
        | IsFailure s ->
            match s with
            | MemberDetail.IsNotFound -> MessageDisplay.ReadModel.make h fs (Seq.singleton fm)

    [<Extension>]
    static member ToMessageDisplayReadModel(result:IResult<IMemberListReadModel,IMemberListQueryFailureStatus>) =
        raiseIfInvalid <| validate result "result" [ notIsNull, "Result was null" ]
        let h = "Member List Query"
        let ss = MessageDisplay.Severity.informational
        let sm = "Member list query was successful"
        let fs =MessageDisplay.Severity.error
        let fm = "Member list query was not successful and returned not found"
        match result with
        | IsSuccess _ -> MessageDisplay.ReadModel.make h ss (Seq.singleton sm)
        | IsFailure s ->
            match s with
            | MemberList.IsNotFound -> MessageDisplay.ReadModel.make h fs (Seq.singleton fm)
