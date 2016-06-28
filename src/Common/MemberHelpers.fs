namespace Fooble.Common

open System
open System.Net.Mail
open System.Text.RegularExpressions

[<AutoOpen>]
module internal MemberHelpers =

    let specialPasswordCharsPattern =
        """ ~!@#$%\^&*_\-+=`|\\(){}[\]:;"'<>,.?/"""

    let private isEmailString x =
        assert (not (isNull x))
        try
            ignore (MailAddress(x))
            true
        with :? FormatException -> false

    let private isLongerString max (x:string) =
        assert (max >= 0)
        assert (not (isNull x))
        x.Length > max

    let private isMatchingString pattern x =
        assert (not (isNull pattern))
        assert (not (isNull x))
        Regex.IsMatch(x, pattern)

    let private isPasswordString x =
        assert (not (isNull x))
        Regex.IsMatch(x, sprintf "^[0-9a-zA-Z%s]+$" specialPasswordCharsPattern)

    let private isShorterString min (x:string) =
        assert (min >= 0)
        assert (not (isNull x))
        x.Length < min

    let private passwordContainsDigitChars x =
        assert (not (isNull x))
        Regex.IsMatch(x, "[0-9]")

    let private passwordContainsLowercaseChars x =
        assert (not (isNull x))
        Regex.IsMatch(x, "[a-z]")

    let private passwordContainsSpecialChars x =
        assert (not (isNull x))
        Regex.IsMatch(x, sprintf "[%s]" specialPasswordCharsPattern)

    let private passwordContainsUppercaseChars x =
        assert (not (isNull x))
        Regex.IsMatch(x, "[A-Z]")

    let validateMemberAvatarData avatarData =
        [ (String.IsNullOrEmpty >> not), "Avatar data is required"
          (isLongerString 32 >> not), "Avatar data is longer than 32 characters" ]
        |> validateOn avatarData "avatarData"

    let validateMemberEmail email =
        [ (String.IsNullOrEmpty >> not), "Email is required"
          (isLongerString 256 >> not), "Email is longer than 256 characters"
          (isEmailString), "Email is not in the correct format" ]
        |> validateOn email "email"

    let validateMemberId id =
        [ ((<>) Guid.Empty), "Id is required" ]
        |> validateOn id "id"

    let validateMemberIdString id =
        [ (String.IsNullOrEmpty >> not), "Id is required"
          (Guid.TryParse >> fst), "Id is not in the correct format (GUID)" ]
        |> validateOn id "id"

    let validateMemberNickname nickname =
        [ (String.IsNullOrEmpty >> not), "Nickname is required"
          (isLongerString 64 >> not), "Nickname is longer than 64 characters" ]
        |> validateOn nickname "nickname"

    let validateMemberPasswordData passwordData =
        [ (String.IsNullOrEmpty >> not), "Password data is required"
          (isShorterString 64 >> not), "Password data is shorter than 64 characters"
          (isLongerString 128 >> not), "Password data is longer than 128 characters" ]
        |> validateOn passwordData "passwordData"

    let validateMemberPasswords password confirmPassword =
        let initial =
            [ (String.IsNullOrEmpty >> not), "Password is required"
              (isShorterString 8 >> not), "Password is shorter than 8 characters"
              (isLongerString 32 >> not), "Password is longer than 32 characters"
              (passwordContainsDigitChars), "Password does not contain any numbers"
              (passwordContainsLowercaseChars), "Password does not contain any lower-case letters"
              (passwordContainsUppercaseChars), "Password does not contain any upper-case letters"
              (passwordContainsSpecialChars), "Password does not contain any special characters"
              (isPasswordString), "Password contains invalid characters" ]
            |> validateOn password "password"
        match initial with
        | Some(x) -> Some(x)
        | _ ->
        match confirmPassword with
        | Some(x) -> validateOn x "confirmPassword" [ ((=) password), "Passwords do not match" ]
        | None -> None

    let validateMemberUsername username =
        [ (String.IsNullOrEmpty >> not), "Username is required"
          (isShorterString 3 >> not), "Username is shorter than 3 characters"
          (isLongerString 32 >> not), "Username is longer than 32 characters"
          (isMatchingString "^[a-z0-9]+$"), "Username is not in the correct format (lowercase alphanumeric)" ]
        |> validateOn username "username"
