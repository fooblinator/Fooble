namespace Fooble.Core

open System

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
        let internal isNotEmpty x = not <| isEmpty x
        let internal isGuid x = fst (Guid.TryParse(x))
        let internal isLonger max x = String.length x > max
        let internal isNotLonger max x = not <| isLonger max x
        let internal isShorter min x = String.length x < min
        let internal isNotShorter min x = not <| isShorter min x

    [<RequireQualifiedAccess>]
    module internal Seq =
        let internal isNotEmpty x = not <| Seq.isEmpty x

    (* Misc *)

    let internal isNotNull x = not <| isNull x
