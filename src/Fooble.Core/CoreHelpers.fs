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
        let internal notIsGuid x = not <| isGuid x

    [<RequireQualifiedAccess>]
    module internal List =

        let internal containsEmptyStrings x = List.contains String.empty x
        let internal containsNulls x = List.contains null x
        let internal notContainsEmptyStrings x = not <| containsEmptyStrings x
        let internal notContainsNulls x = not <| containsNulls x
        let internal notIsEmpty x = not <| List.isEmpty x

    [<RequireQualifiedAccess>]
    module internal Seq =

        let internal containsEmptyStrings x = Seq.contains String.empty x
        let internal containsNulls x = Seq.contains null x
        let internal notContainsEmptyStrings x = not <| containsEmptyStrings x
        let internal notContainsNulls x = not <| containsNulls x
        let internal notIsEmpty x = not <| Seq.isEmpty x

    (* Misc *)

    let internal notIsNull x = not <| isNull x