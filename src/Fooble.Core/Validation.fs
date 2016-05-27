namespace Fooble.Core

open Fooble.Common
open Fooble.Presentation
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides functionality used in the validation of data.
[<RequireQualifiedAccess>]
[<Extension>]
module Validation =

    /// <summary>
    /// Adds a model error to the model state if the validation result is not valid.
    /// </summary>
    /// <param name="result">The validation result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    [<CompiledName("AddModelErrorIfNotValid")>]
    let addModelErrorIfNotValid result (modelState:ModelStateDictionary) =

        [ (isNotNull << box), "Result is required" ]
        |> ValidationResult.get result "result"
        |> ValidationResult.enforce

        [ (isNotNull), "Model state is required" ]
        |> ValidationResult.get modelState "modelState"
        |> ValidationResult.enforce

        match result with
        | ValidationResult.IsInvalid (x, y) -> modelState.AddModelError(x, y)
        | ValidationResult.IsValid _ -> ()

    /// <summary>
    /// Constructs a message display read model from a validation result.
    /// </summary>
    /// <param name="result">The validation result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on "invalid" results. For displaying a "valid" result, use
    /// <see cref="MessageDisplay.MakeReadModel"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel result =

        [ (isNotNull << box), "Result parameter was null" ]
        |> ValidationResult.get result "result"
        |> ValidationResult.enforce

        match result with
        | ValidationResult.IsValid ->  invalidOp "Result was not invalid"
        | ValidationResult.IsInvalid (_, x) ->
            MessageDisplay.makeReadModel "Validation" String.empty 400 MessageDisplay.errorSeverity
                (sprintf "Validation was not successful and returned: \"%s\"" x)
