namespace Fooble.Core

open Fooble.Common

/// <summary>
/// Represents the status of parameter validation, and potential results, if invalid.
/// </summary>
/// <remarks>The result is only one of "valid" or "invalid".</remarks>
type IValidationResult =
    /// The name of the invalid parameter.
    abstract ParamName:string with get
    /// The message describing why the parameter is invalid.
    abstract Message:string with get
    /// Whether the result is "valid" (or not).
    abstract IsValid:bool with get
    /// Whether the result is "invalid" (or not).
    abstract IsInvalid:bool with get

[<RequireQualifiedAccess>]
module internal ValidationResult =

    let internal (|IsValid|IsInvalid|) (result:IValidationResult) =
        if result.IsValid then Choice1Of2 ()
        else Choice2Of2 (result.ParamName, result.Message) // IsInvalid

    [<DefaultAugmentation(false)>]
    type private ValidationResultImplementation =
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
        assert (String.isNotNullOrEmpty paramName)
        assert (String.isNotNullOrEmpty message)
        Invalid (paramName, message) :> IValidationResult

    let internal get value paramName conditions =
        assert (String.isNotNullOrEmpty paramName)
        let chooser (fn, x) = if not <| fn value then Some x else None
        match Seq.tryPick chooser conditions with
        | None -> valid
        | Some x -> makeInvalid paramName x

    let internal enforce result =
        match result with
        | IsValid -> ()
        | IsInvalid (x, y) -> invalidArg x y
