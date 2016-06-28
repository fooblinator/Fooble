namespace Fooble.Common

open System
open System.Diagnostics

[<DebuggerStepThrough>]
[<AutoOpen>]
module internal CommonHelpers =

    (* Validate *)

    let validateOn value paramName conditions =
        assert (not (String.IsNullOrEmpty(paramName)))
        assert (not (List.isEmpty conditions))
        let result = Seq.tryPick (fun (f, x) -> if f value then None else Some(x)) conditions
        match result with
        | Some(x) -> Some(paramName, x)
        | None -> None

    let validateRequired value paramName messagePrefix =
        validateOn value paramName [ (box >> isNull >> not), sprintf "%s is required" messagePrefix ]

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

    (* Other *)

    let randomString len =
        assert (len > 0)
        Seq.init (len / 32 + 1) (fun _ -> Guid.NewGuid() |> string |> fun x -> x.ToCharArray())
        |> Array.concat
        |> Array.filter ((<>) '-')
        |> Array.take len
        |> String
