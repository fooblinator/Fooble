namespace Fooble.Common

open System
open System.Diagnostics
open System.Net.Mail
open System.Text.RegularExpressions

[<DebuggerStepThrough>]
[<AutoOpen>]
module internal CommonHelpers =

    let isNotNull x = not <| isNull x

[<DebuggerStepThrough>]
[<RequireQualifiedAccess>]
module internal Guid =

    let empty = Guid.Empty

    let isEmpty x = x = empty
    let isNotEmpty x = not <| isEmpty x

    let parse x = Guid.Parse(x)
    let tryParse x =
        match Guid.TryParse(x) with
        | (true, x) -> Some x
        | _ -> None

    let random () = Guid.NewGuid()

    let toString (x:Guid) = x.ToString()

[<DebuggerStepThrough>]
[<RequireQualifiedAccess>]
module internal String =

    let empty = String.Empty

    let isEmpty x = x = empty
    let isNotEmpty x = not <| isEmpty x

    let isNullOrEmpty x = isNull x || isEmpty x
    let isNotNullOrEmpty x = not <| isNullOrEmpty x

    let isGuid x = Guid.tryParse x |> Option.isSome
    let isNotGuid x = not <| isGuid x

    let isLonger max x = String.length x > max
    let isNotLonger max x = not <| isLonger max x

    let isShorter min x = String.length x < min
    let isNotShorter min x = not <| isShorter min x

    let isMatch pattern x = Regex.IsMatch(x, pattern)
    let isNotMatch pattern x = not <| isMatch pattern x

    let isEmail x = try ignore <| MailAddress(x); true with :? FormatException -> false
    let isNotEmail x = not <| isEmail x

    let ofArray (x:char[]) = String(x)
    let ofGuid x = Guid.toString x

    let toArray (x:string) = x.ToCharArray()
    let toGuid x = Guid.parse x

    let random len =
        assert (len > 0)
        Seq.init (len / 32 + 1) (fun _ -> Guid.random () |> ofGuid |> toArray)
        |> Array.concat
        |> Array.filter (fun x -> x <> '-')
        |> Array.take len
        |> ofArray

[<DebuggerStepThrough>]
[<RequireQualifiedAccess>]
module internal Password =

    let specialCharsPattern = """ ~!@#$%\^&*_\-+=`|\\(){}[\]:;"'<>,.?/"""

    let hasDigits x = String.isMatch "[0-9]" x
    let hasLowerAlphas x = String.isMatch "[a-z]" x
    let hasUpperAlphas x = String.isMatch "[A-Z]" x
    let hasSpecialChars x = String.isMatch (sprintf "[%s]" specialCharsPattern) x

    let isMatch x = String.isMatch (sprintf "^[0-9a-zA-Z%s]+$" specialCharsPattern) x
    let isNotMatch x = not (isMatch x)

    let charset =
        Array.concat [ [| '0' .. '9' |]; [| 'a' .. 'z' |]; [| 'A' .. 'Z' |] ]
        |> Array.append (String.toArray specialCharsPattern)
        |> Set.ofArray

    let random =
        let chars = Set.toList charset
        let charsLen = chars.Length
        let random = Random()
        let rec generate len =
            let res = [| for _ in 0..len-1 -> chars.[random.Next(charsLen)] |] |> String
            match (hasDigits res, hasLowerAlphas res, hasUpperAlphas res, hasSpecialChars res) with
            | (true, true, true, true) -> res
            | _ -> generate len
        fun len -> assert (len > 0); generate len

[<DebuggerStepThrough>]
[<RequireQualifiedAccess>]
module internal EmailAddress =

    let random () = sprintf "%s@%s.%s" (String.random 14) (String.random 14) (String.random 2)

[<DebuggerStepThrough>]
[<RequireQualifiedAccess>]
module internal Seq =

    let isNotEmpty x = not <| Seq.isEmpty x

    let isNullOrEmpty x = isNull x || Seq.isEmpty x
    let isNotNullOrEmpty x = not <| isNullOrEmpty x

[<DebuggerStepThrough>]
[<RequireQualifiedAccess>]
module internal List =

    let isNotEmpty x = not <| List.isEmpty x
