namespace Fooble.Core

open Fooble.Common

[<RequireQualifiedAccess>]
module internal KeyGenerator =

    let make () = { new IKeyGenerator with member this.GenerateKey() = Guid.random () }