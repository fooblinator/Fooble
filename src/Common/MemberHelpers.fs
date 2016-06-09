namespace Fooble.Common

[<AutoOpen>]
module internal MemberHelpers =

    let validateMemberEmail email =
        [ (String.isNotNullOrEmpty), "Email is required"
          (String.isNotLonger 254), "Email is longer than 254 characters"
          (String.isEmail), "Email is not in the correct format" ]
        |> validateOn email "email"

    let validateMemberId id =
        [ (Guid.isNotEmpty), "Id is required" ]
        |> validateOn id "id"

    let validateMemberIdString id =
        [ (String.isNotNullOrEmpty), "Id is required"
          (String.isGuid), "Id is not in the correct format (GUID)" ]
        |> validateOn id "id"

    let validateMemberNickname nickname =
        [ (String.isNotNullOrEmpty), "Nickname is required"
          (String.isNotLonger 64), "Nickname is longer than 64 characters" ]
        |> validateOn nickname "nickname"

    let validateMemberPasswordData passwordData =
        [ (String.isNotNullOrEmpty), "Password data is required"
          (String.isNotShorter 64), "Password data is shorter than 64 characters"
          (String.isNotLonger 128), "Password data is longer than 128 characters" ]
        |> validateOn passwordData "passwordData"

    let validateMemberPasswordsWith password confirmPassword paramName messagePrefix =
        let initial =
            [ (String.isNotNullOrEmpty), sprintf "%s is required" messagePrefix
              (String.isNotShorter 8), sprintf "%s is shorter than 8 characters" messagePrefix
              (String.isNotLonger 32), sprintf "%s is longer than 32 characters" messagePrefix
              (Password.hasDigits), sprintf "%s does not contain any numbers" messagePrefix
              (Password.hasLowerAlphas), sprintf "%s does not contain any lower-case letters" messagePrefix
              (Password.hasUpperAlphas), sprintf "%s does not contain any upper-case letters" messagePrefix
              (Password.hasSpecialChars), sprintf "%s does not contain any special characters" messagePrefix
              (Password.isMatch), sprintf "%s contains invalid characters" messagePrefix ]
            |> validateOn password paramName
        match initial with
        | Some(x) -> Some(x)
        | _ ->
        match confirmPassword with
        | Some(x) -> validateOn x "confirmPassword" [ ((=) password), "Passwords do not match" ]
        | None -> None

    let validateMemberPasswords password confirmPassword =
        validateMemberPasswordsWith password confirmPassword "password" "Password"

    let validateMemberUsername username =
        [ (String.isNotNullOrEmpty), "Username is required"
          (String.isNotShorter 3), "Username is shorter than 3 characters"
          (String.isNotLonger 32), "Username is longer than 32 characters"
          (String.isMatch "^[a-z0-9]+$"), "Username is not in the correct format (lowercase alphanumeric)" ]
        |> validateOn username "username"
