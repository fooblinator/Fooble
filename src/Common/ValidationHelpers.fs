namespace Fooble.Common

open Fooble.Core
open System.Diagnostics

[<DebuggerStepThrough>]
[<AutoOpen>]
module internal ValidationHelpers =

    let validate value paramName conditions =
        assert (String.isNotNullOrEmpty paramName)
        assert (List.isNotEmpty conditions)

        let chooser (f, x) = if f value then None else Some x

        match Seq.tryPick chooser conditions with
        | None -> ValidationResult.valid
        | Some x -> ValidationResult.makeInvalid paramName x

    let enforce (result:IValidationResult) =
        match result with
        | x when x.IsInvalid -> invalidArg x.ParamName x.Message
        | _ -> ()
