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
          (String.isNotLonger 32), "Username is longer than 32 characters"
          (String.isMatch "^[a-z0-9]+$"), "Username is not in the correct format (lowercase alphanumeric)" ]
        |> Validation.validate username "username"

    /// <summary>
    /// Validates the supplied member email.
    /// </summary>
    /// <param name="email">The email of the member.</param>
    /// <returns>Returns a validation result.</returns>
    [<CompiledName("ValidateEmail")>]
    let validateEmail email =
        [ (String.isNotNullOrEmpty), "Email is required"
          (String.isNotLonger 254), "Email is longer than 254 characters"
          (String.isEmail), "Email is not in the correct format" ]
        |> Validation.validate email "email"

    /// <summary>
    /// Validates the supplied member nickname.
    /// </summary>
    /// <param name="nickname">The nickname of the member.</param>
    /// <returns>Returns a validation result.</returns>
    [<CompiledName("ValidateNickname")>]
    let validateNickname nickname =
        [ (String.isNotNullOrEmpty), "Nickname is required"
          (String.isNotLonger 64), "Nickname is longer than 64 characters" ]
        |> Validation.validate nickname "nickname"
