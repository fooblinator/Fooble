namespace Fooble.Core

[<RequireQualifiedAccess>]
module internal KeyGenerator =

    let internal make () =
        { new IKeyGenerator with
              member this.GenerateKey() = Guid.random () }