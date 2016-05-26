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
        [ (Guid.isNotEmpty), "Id is required" ]
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
        [ (String.isNotNullOrEmpty), "Id is required"
          (String.isGuid), "Id is not in the correct format (GUID)" ]
        |> Validation.validate id "id"

    /// <summary>
    /// Validates the supplied member username.
    /// </summary>
    /// <param name="username">The username of the member.</param>
    /// <returns>Returns a validation result.</returns>
    [<CompiledName("ValidateUsername")>]
    let validateUsername username =
        [ (String.isNotNullOrEmpty), "Username is required"
          (String.isNotShorter 3), "Username is shorter than 3 characters"
          (String.isNotLonger 32), "Username is longer than 32 characters" ]
        |> Validation.validate username "username"

    /// <summary>
    /// Validates the supplied member name.
    /// </summary>
    /// <param name="name">The name of the member.</param>
    /// <returns>Returns a validation result.</returns>
    [<CompiledName("ValidateName")>]
    let validateName name =
        [ (String.isNotNullOrEmpty), "Name is required" ]
        |> Validation.validate name "name"
