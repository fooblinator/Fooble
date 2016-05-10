namespace Fooble.Core

open System

[<RequireQualifiedAccess>]
module internal Helper = 
    let checkNotNull<'T> (value : 'T) errorPrefix = 
        match box value with
        | null -> Seq.singleton (sprintf "%s should not be null" errorPrefix)
        | _ -> Seq.empty
    
    let ensureValid<'T> validator (value : 'T) paramName = 
        let errors = validator value |> Seq.map (fun m -> (paramName, m))
        match Seq.tryHead errors with
        | Some(an, m) -> invalidArg an m
        | _ -> ()
