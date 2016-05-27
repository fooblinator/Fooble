namespace Fooble.Common

open System

[<RequireQualifiedAccess>]
module Guid =

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
