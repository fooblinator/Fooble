namespace Fooble.Common

[<AutoOpen>]
module internal MemberHelpers =

    let validateMemberEmailWith email paraName messagePrefix =
        [ (String.isNotNullOrEmpty), sprintf "%s is required" messagePrefix
          (String.isNotLonger 254), sprintf "%s is longer than 254 characters" messagePrefix
          (String.isEmail), sprintf "%s is not in the correct format" messagePrefix ]
        |> validateOn email paraName

    let validateMemberEmail email =
        validateMemberEmailWith email "email" "Email"

    let validateMemberId id =
        [ (Guid.isNotEmpty), "Id is required" ]
        |> validateOn id "id"

    let validateMemberIdString id =
        [ (String.isNotNullOrEmpty), "Id is required"
          (String.isGuid), "Id is not in the correct format (GUID)" ]
        |> validateOn id "id"

    let validateMemberNicknameWith nickname paramName messagePrefix =
        [ (String.isNotNullOrEmpty), sprintf "%s is required" messagePrefix
          (String.isNotLonger 64), sprintf "%s is longer than 64 characters" messagePrefix ]
        |> validateOn nickname paramName

    let validateMemberNickname nickname =
        validateMemberNicknameWith nickname "nickname" "Nickname"

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

    let validateMemberUsernameWith username paraName messagePrefix =
        [ (String.isNotNullOrEmpty), sprintf "%s is required" messagePrefix
          (String.isNotShorter 3), sprintf "%s is shorter than 3 characters" messagePrefix
          (String.isNotLonger 32), sprintf "%s is longer than 32 characters" messagePrefix
          ((String.isMatch "^[a-z0-9]+$"),
              sprintf "%s is not in the correct format (lowercase alphanumeric)" messagePrefix) ]
        |> validateOn username paraName

    let validateMemberUsername username =
        validateMemberUsernameWith username "username" "Username"
