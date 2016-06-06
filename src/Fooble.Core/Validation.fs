namespace Fooble.Core

open Fooble.Common

[<RequireQualifiedAccess>]
module ValidationResult =

    [<DefaultAugmentation(false)>]
    type private ValidationResultImpl =
        | Valid
        | Invalid of paramName:string * message:string

        interface IValidationResult with

            member this.ParamName
                with get() =
                    match this with
                    | Invalid(paramName = x) -> x
                    | _ -> invalidOp "Result was not invalid"

            member this.Message
                with get() =
                    match this with
                    | Invalid(message = x) -> x
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
        Invalid(paramName, message) :> IValidationResult
