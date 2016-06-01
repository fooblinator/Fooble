namespace Fooble.Common

open System
open System.Net.Mail
open System.Text.RegularExpressions

[<AutoOpen>]
module internal CommonHelpers =

    let isNotNull x = not <| isNull x

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

[<RequireQualifiedAccess>]
module internal EmailAddress =

    let random () = sprintf "%s@%s.%s" (String.random 14) (String.random 14) (String.random 2)

[<RequireQualifiedAccess>]
module internal Seq =

    let isNotEmpty x = not <| Seq.isEmpty x

    let isNullOrEmpty x = isNull x || Seq.isEmpty x
    let isNotNullOrEmpty x = not <| isNullOrEmpty x

[<RequireQualifiedAccess>]
module internal List =

    let isNotEmpty x = not <| List.isEmpty x
