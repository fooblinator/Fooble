namespace Fooble.Core

open System.Runtime.CompilerServices

[<AutoOpen; Extension>]
type CoreExtensions =

    (* Extensions *)

    [<Extension>]
    static member ToMessageDisplayReadModel(result:IResult<IMemberDetailReadModel,IMemberDetailQueryFailureStatus>) =

        let contracts =
            [ postCondition (isNullValue >> not) "(MemberDetail) toMessageDisplayReadModel returned null value" ]

        let body x =
            enforce (validateIsNotNullValue x "result" "Result")
            let h = "Member Detail Query"
            let ss = MessageDisplay.informationalSeverity
            let sm = "Member detail query was successful"
            let fs = MessageDisplay.errorSeverity
            let fm = "Member detail query was not successful and returned not found"
            match x with
            | IsSuccess _ -> MessageDisplay.makeReadModel h ss (Seq.singleton sm)
            | IsFailure s ->
                match s with
                | MemberDetail.IsNotFound -> MessageDisplay.makeReadModel h fs (Seq.singleton fm)

        ensure contracts body result

    [<Extension>]
    static member ToMessageDisplayReadModel(result:IResult<IMemberListReadModel,IMemberListQueryFailureStatus>) =

        let contracts =
            [ postCondition (isNullValue >> not) "(MemberList) toMessageDisplayReadModel returned null value" ]

        let body x =
            enforce (validateIsNotNullValue x "result" "Result")
            let h = "Member List Query"
            let ss = MessageDisplay.informationalSeverity
            let sm = "Member list query was successful"
            let fs = MessageDisplay.errorSeverity
            let fm = "Member list query was not successful and returned not found"
            match x with
            | IsSuccess _ -> MessageDisplay.makeReadModel h ss (Seq.singleton sm)
            | IsFailure s ->
                match s with
                | MemberList.IsNotFound -> MessageDisplay.makeReadModel h fs (Seq.singleton fm)

        ensure contracts body result
