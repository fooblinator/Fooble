namespace Fooble.Common

[<RequireQualifiedAccess>]
module Seq =

    let isNotEmpty x = not <| Seq.isEmpty x

    let isNullOrEmpty x = isNull x || Seq.isEmpty x
    let isNotNullOrEmpty x = not <| isNullOrEmpty x
