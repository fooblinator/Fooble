namespace Fooble.Core

open System
open System.Text.RegularExpressions

[<AutoOpen>]
module internal Helpers =

    (* Extensions *)

    [<RequireQualifiedAccess>]
    module internal Guid =
        let internal empty = Guid.Empty
        let internal isEmpty x = x = empty
        let internal isNotEmpty x = not <| isEmpty x
        let internal random () = Guid.NewGuid()

    [<RequireQualifiedAccess>]
    module internal String =

        let internal empty = String.Empty
        let internal isEmpty x = x = empty
        let internal isNullOrEmpty x = isNull x || isEmpty x
        let internal isNotNullOrEmpty x = not <| isNullOrEmpty x
        let internal isGuid x = fst (Guid.TryParse(x))
        let internal isLonger max x = String.length x > max
        let internal isNotLonger max x = not <| isLonger max x
        let internal isShorter min x = String.length x < min
        let internal isNotShorter min x = not <| isShorter min x
        let internal isMatch pattern x = Regex.IsMatch(x, pattern)

    [<RequireQualifiedAccess>]
    module internal Seq =
        let internal isNullOrEmpty x = isNull x || Seq.isEmpty x
        let internal isNotNullOrEmpty x = not <| isNullOrEmpty x

    (* Misc *)

    let internal isNotNull x = not <| isNull x
