namespace Fooble.Core

open System.Diagnostics

[<RequireQualifiedAccess>]
module internal Validation =

    [<DefaultAugmentation(false)>]
    type private Result =
        | Valid'
        | Invalid of string * string

        member this.ParamName =
            match this with
            | Invalid (x, _) -> x
            | Valid' -> invalidOp "Result was not invalid"

        member this.Message =
            match this with
            | Invalid (_, x) -> x
            | Valid' -> invalidOp "Result was not invalid"

        member this.IsValid =
            match this with
            | Valid' -> true
            | Invalid _ -> false

        member this.IsInvalid =
            match this with
            | Invalid _ -> true
            | Valid' -> false

        interface IValidationResult with
            member this.ParamName = this.ParamName
            member this.Message = this.Message
            member this.IsValid = this.IsValid
            member this.IsInvalid = this.IsInvalid

    let internal validResult = Valid' :> IValidationResult

    let internal makeInvalidResult paramName message =
        Debug.Assert(notIsNull paramName, "Param name parameter was null")
        Debug.Assert(String.notIsEmpty paramName, "Param name parameter was empty string")
        Debug.Assert(notIsNull message, "Message parameter was null")
        Debug.Assert(String.notIsEmpty message, "Message parameter was empty string")
        Invalid(paramName, message) :> IValidationResult

    let internal (|IsValid|IsInvalid|) (result:IValidationResult) =
        if result.IsValid
            then Choice1Of2 ()
            else Choice2Of2 (result.ParamName, result.Message)

    let internal validate value paramName conditions =
        let chooser (fn, x) = if not <| fn value then Some x else None
        match Seq.tryPick chooser conditions with
        | None -> validResult
        | Some x -> makeInvalidResult paramName x

    let internal raiseIfInvalid result =
        match result with
        | IsValid -> ()
        | IsInvalid (x, y) -> invalidArg x y
