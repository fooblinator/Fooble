namespace Fooble.Common

open System.Diagnostics

[<AutoOpen>]
module internal ValidationHelpers =

    (* Validate *)

    let validateOn value paramName conditions =
        assert (String.isNotNullOrEmpty paramName)
        assert (List.isNotEmpty conditions)
        let result = Seq.tryPick (fun (f, x) -> if f value then None else Some(x)) conditions
        match result with
        | Some(x) -> Some(paramName, x)
        | None -> None

    let validateRequired value paramName messagePrefix =
        validateOn value paramName [ (box >> isNotNull), sprintf "%s is required" messagePrefix ]

    (* Ensure *)

    let ensureWith result =
        match result with
        | Some(x, y) -> invalidArg x y
        | None -> ()

    let ensureOn value paramName conditions =
        ensureWith (validateOn value paramName conditions)

#if DEBUG

    (* Assert *)

    let assertWith result =
        match result with
        | Some(x, y) -> Debug.Fail(sprintf "[%s] %s" x y)
        | None -> ()

    let assertOn value paramName conditions =
        assertWith (validateOn value paramName conditions)

#endif
