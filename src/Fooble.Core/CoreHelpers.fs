[<AutoOpen>]
module internal Fooble.Core.CoreHelpers

open System
open System.Diagnostics

(* Contracts *)

let internal ensure contracts fn  =
    List.fold (|>) fn contracts

let internal preCondition condition message fn x  =
    Debug.Assert(condition x, message)
    fn x

let internal postCondition condition message fn x =
    let result = fn x
    Debug.Assert(condition result, message)
    result

(* Checks *)

let internal isNullValue value =
    isNull value

let internal isEmptyString value =
    match value with
    | "" -> true
    | _ -> false

let internal isGuidString value =
    (Guid.TryParse >> fst) value

let internal isEmptyValue value =
    Seq.isEmpty value

let internal containsNullValues values =
    (Seq.tryFind isNull >> Option.isSome) values

let internal containsEmptyStrings values =
    (Seq.tryFind isEmptyString >> Option.isSome) values

(* Misc *)

let internal fst3 (x, _, _) = x
let internal snd3 (_, x, _) = x
let internal thd3 (_, _, x) = x
