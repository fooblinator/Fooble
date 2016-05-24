namespace Fooble.Core

open System

[<AutoOpen>]
module internal Helpers =

    (* Extensions *)

    [<RequireQualifiedAccess>]
    module internal Guid =
        let internal empty = System.Guid.Empty
        let internal isEmpty x = x = empty
        let internal notIsEmpty x = not <| isEmpty x

    [<RequireQualifiedAccess>]
    module internal String =
        let internal empty = System.String.Empty
        let internal isEmpty x = x = empty
        let internal notIsEmpty x = not <| isEmpty x
        let internal isGuid x = fst <| Guid.TryParse(x)

    [<RequireQualifiedAccess>]
    module internal Seq =
        let internal notIsEmpty x = not <| Seq.isEmpty x

    (* Misc *)

    let internal notIsNull x = not <| isNull x