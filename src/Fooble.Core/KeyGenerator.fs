namespace Fooble.Core

open Fooble.Common
open System

/// Used to generate unique keys.
type IKeyGenerator =
    /// <summary>
    /// Generates a unique key.
    /// </summary>
    /// <returns>Returns a newly generated unique key.</returns>
    abstract GenerateKey:unit -> Guid

[<RequireQualifiedAccess>]
module internal KeyGenerator =

    let internal make () =
        { new IKeyGenerator with
              member this.GenerateKey() = Guid.random () }