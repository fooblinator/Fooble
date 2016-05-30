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
module ValidationResult =

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

    /// <summary>
    /// Represents a valid validation result.
    /// </summary>
    /// <returns>Returns a valid validation result.</returns>
    [<CompiledName("Valid")>]
    let valid = Valid :> IValidationResult

    /// <summary>
    /// Constructs an invalid validation result.
    /// </summary>
    /// <param name="paramName">The parameter name of the invalid value.</param>
    /// <param name="message">The validation message.</param>
    /// <returns>Returns an invalid validation result.</returns>
    [<CompiledName("MakeInvalid")>]
    let makeInvalid paramName message =
        if String.isNullOrEmpty paramName then invalidArg "paramName" "Param name is required"
        if String.isNullOrEmpty message then invalidArg "message" "Message is required"
        Invalid (paramName, message) :> IValidationResult

[<AutoOpen>]
module internal ValidationHelpers =

    let internal validate value paramName conditions =
        assert (String.isNotNullOrEmpty paramName)
        assert (List.isNotEmpty conditions)

        let chooser (f, x) = if f value then None else Some x

        match Seq.tryPick chooser conditions with
        | None -> ValidationResult.valid
        | Some x -> ValidationResult.makeInvalid paramName x

    let internal enforce (result:IValidationResult) =
        match result with
        | x when x.IsInvalid -> invalidArg x.ParamName x.Message
        | _ -> ()
