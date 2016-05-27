namespace Fooble.Core

open System.Runtime.CompilerServices
open System.Web.Mvc

[<RequireQualifiedAccess>]
[<Extension>]
module Validation =

    (* Active Patterns *)

    let internal (|IsValid|IsInvalid|) (result:IValidationResult) =
        if result.IsValid
            then Choice1Of2 ()
            else Choice2Of2 (result.ParamName, result.Message)



    (* Result *)

    [<RequireQualifiedAccess>]
    module internal Result =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | Valid
            | Invalid of string * string

            interface IValidationResult with

                member this.ParamName
                    with get() =
                        match this with
                        | Invalid (x, _) -> x
                        | _ -> invalidOp "Result was not invalid"

                member this.Message
                    with get() =
                        match this with
                        | Invalid (_, x) -> x
                        | _ -> invalidOp "Result was not invalid"

                member this.IsValid
                    with get() =
                        match this with
                        | Valid -> true
                        | _ -> false

                member this.IsInvalid
                    with get() =
                        match this with
                        | Invalid _ -> true
                        | _ -> false

        let internal valid = Valid :> IValidationResult

        let internal makeInvalid paramName message =
            assert (isNotNull paramName)
            assert (String.isNotEmpty paramName)
            assert (isNotNull message)
            assert (String.isNotEmpty message)
            Invalid(paramName, message) :> IValidationResult



    (* Helpers *)

    let internal validate value paramName conditions =
        let chooser (fn, x) = if not <| fn value then Some x else None
        match Seq.tryPick chooser conditions with
        | None -> Result.valid
        | Some x -> Result.makeInvalid paramName x

    let internal raiseIfInvalid result =
        match result with
        | IsValid -> ()
        | IsInvalid (x, y) -> invalidArg x y



    (* Extensions *)

    [<Extension>]
    [<CompiledName("AddModelErrorIfNotValid")>]
    let addModelErrorIfNotValid result (modelState:ModelStateDictionary) =

        [ (isNotNull << box), "Result is required" ]
        |> validate result "result"
        |> raiseIfInvalid

        [ (isNotNull), "Model state is required" ]
        |> validate modelState "modelState"
        |> raiseIfInvalid

        match result with
        | IsInvalid (x, y) -> modelState.AddModelError(x, y)
        | IsValid _ -> ()
