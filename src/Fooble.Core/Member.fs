namespace Fooble.Core

/// <summary>
/// Provides common functionality used in the other member-related modules.
/// </summary>
[<RequireQualifiedAccess>]
module Member =

    (* Validation *)

    /// <summary>
    /// Validates the supplied member id.
    /// </summary>
    /// <param name="id">The id that represents the member.</param>
    /// <returns>Returns a validation result.</returns>
    /// <remarks>
    /// The member id is of type <see cref="System.Guid"/>, however this helper exists to validate Guid-formatted
    /// strings before parsing into a Guid. This exists purely for consistency with the other validation helpers, and
    /// as an alternative to <see cref="System.Guid.TryParse"/>.
    /// </remarks>
    [<CompiledName("ValidateId")>]
    let validateId id =
        [ (notIsNull), "Id parameter was null"
          (String.notIsEmpty), "Id parameter was an empty string"
          (String.isGuid), "Id parameter was not in GUID format" ]
        |> Validation.validate id "id"

    /// <summary>
    /// Validates the supplied member name.
    /// </summary>
    /// <param name="id">The name of the member.</param>
    /// <returns>Returns a validation result.</returns>
    [<CompiledName("ValidateName")>]
    let validateName name =
        [ (notIsNull), "Name parameter was null"
          (String.notIsEmpty), "Name parameter was an empty string" ]
        |> Validation.validate name "name"
