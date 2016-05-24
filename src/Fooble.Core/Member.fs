namespace Fooble.Core

/// Provides common functionality used in the other member-related modules.
[<RequireQualifiedAccess>]
module Member =

    (* Validation *)

    /// <summary>
    /// Validates the supplied member id.
    /// </summary>
    /// <param name="id">The id that represents the member.</param>
    /// <returns>Returns a validation result.</returns>
    [<CompiledName("ValidateId")>]
    let validateId id =
        [ (Guid.isNotEmpty), "Id parameter was an empty GUID" ]
        |> Validation.validate id "id"

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
    let validateIdString id =
        [ (isNotNull), "Id parameter was null"
          (String.isNotEmpty), "Id parameter was an empty string"
          (String.isGuid), "Id parameter was not in GUID format" ]
        |> Validation.validate id "id"

    /// <summary>
    /// Validates the supplied member username.
    /// </summary>
    /// <param name="username">The username of the member.</param>
    /// <returns>Returns a validation result.</returns>
    [<CompiledName("ValidateUsername")>]
    let validateUsername username =
        [ (isNotNull), "Username parameter was null"
          (String.isNotEmpty), "Username parameter was an empty string"
          (String.isNotShorter 3), "Username parameter was shorter than 3 characters"
          (String.isNotLonger 32), "Username parameter was longer than 32 characters" ]
        |> Validation.validate username "username"

    /// <summary>
    /// Validates the supplied member name.
    /// </summary>
    /// <param name="name">The name of the member.</param>
    /// <returns>Returns a validation result.</returns>
    [<CompiledName("ValidateName")>]
    let validateName name =
        [ (isNotNull), "Name parameter was null"
          (String.isNotEmpty), "Name parameter was an empty string" ]
        |> Validation.validate name "name"
