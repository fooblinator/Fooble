namespace Fooble.Core

open System

[<RequireQualifiedAccess>]
module internal KeyGenerator =

    let internal make () =
        { new IKeyGenerator with
              member this.GenerateKey() = Guid.NewGuid() }