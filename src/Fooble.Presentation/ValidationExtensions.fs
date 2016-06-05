namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related extension methods for validation.
[<RequireQualifiedAccess>]
[<Extension>]
module ValidationExtensions =

    /// <summary>
    /// Adds a model error to the model state if the validation result is not valid.
    /// </summary>
    /// <param name="result">The validation result to extend.</param>
    /// <param name="modelState">The model state dictionary to add model errors to.</param>
    [<Extension>]
    [<CompiledName("AddModelErrorIfNotValid")>]
    let addModelErrorIfNotValid (result:IValidationResult) (modelState:ModelStateDictionary) =

        [ (box >> isNotNull), "Result is required" ]
        |> validate result "result" |> enforce

        [ (isNotNull), "Model state is required" ]
        |> validate modelState "modelState" |> enforce

        match result with
        | x when x.IsInvalid -> modelState.AddModelError(x.ParamName, x.Message)
        | _ -> ()

    /// <summary>
    /// Constructs a message display read model from a validation result.
    /// </summary>
    /// <param name="result">The validation result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on "invalid" results. For displaying a "valid" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IValidationResult) =

        [ (box >> isNotNull), "Result parameter was null" ]
        |> validate result "result" |> enforce

        match result with
        | x when x.IsInvalid ->
              MessageDisplayReadModel.make "Validation" String.empty 400 MessageDisplayReadModel.errorSeverity
                  (sprintf "Validation was not successful and returned: \"%s\"" x.Message)
        | _ ->  invalidOp "Result was not invalid"
