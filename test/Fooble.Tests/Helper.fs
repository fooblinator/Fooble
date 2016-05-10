namespace Fooble.Tests

open System

[<RequireQualifiedAccess>]
module internal Helper = 
    let fixArgumentExceptionMessage (message : string) = (message.IndexOf "Parameter name: " |> message.Remove).Trim()
    let randomGuidString() = sprintf "%A" (Guid.NewGuid())
