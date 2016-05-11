namespace Fooble.Tests

open System

[<RequireQualifiedAccess>]
module internal Helper = 
    let fixArgumentExceptionMessage (message : string) = (message.IndexOf "Parameter name: " |> message.Remove).Trim()
    let randomGuidString() = sprintf "%A" (Guid.NewGuid())
    
    let randomNonGuidString() = 
        randomGuidString().ToCharArray()
        |> Array.filter (fun c -> c <> '-')
        |> Array.take 16
        |> String
