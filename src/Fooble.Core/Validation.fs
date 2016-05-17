namespace Fooble.Core

open System
open System.Diagnostics

[<RequireQualifiedAccess>]
module internal ValidationResult =

    (* Implementation *)

    type private Implementation =
        | Valid
        | Invalid of string * string

        interface IValidationResult with

            member this.ParamName =
                match this with
                | Invalid (x, _) -> x
                | _ -> invalidOp "Result was not invalid"

            member this.Message =
                match this with
                | Invalid (_, x) -> x
                | _ -> invalidOp "Result was not invalid"

            member this.IsValid =
                match this with
                | Valid -> true
                | _ -> false

            member this.IsInvalid =
                match this with
                | Invalid _ -> true
                | _ -> false

    (* Construction *)

    let internal valid = Valid :> IValidationResult

    let internal invalid paramName message =
        Debug.Assert(notIsNull paramName, "(ValidationResult.invalid) paramName argument was null")
        Debug.Assert(String.notIsEmpty paramName, "(ValidationResult.invalid) paramName argument was empty string")
        Debug.Assert(notIsNull message, "(ValidationResult.invalid) message argument was null")
        Debug.Assert(String.notIsEmpty message, "(ValidationResult.invalid) message argument was empty string")
        Invalid (paramName, message) :> IValidationResult

[<AutoOpen>]
module internal ValidationLibrary =

    (* Active Patterns *)

    let internal (|IsValid|IsInvalid|) (result:IValidationResult) =
        if result.IsValid
            then Choice1Of2 ()
            else Choice2Of2 (result.ParamName, result.Message)

    (* Misc *)

    let internal validate value (paramName:string) conditions =
        let chooser (fn, x) = if not <| fn value then Some x else None
        match Seq.tryPick chooser conditions with
        | None -> ValidationResult.valid
        | Some x -> ValidationResult.invalid paramName x

    let internal raiseIfInvalid (result:IValidationResult) =
        match result with
        | IsValid -> ()
        | IsInvalid (x, y) -> invalidArg x y
