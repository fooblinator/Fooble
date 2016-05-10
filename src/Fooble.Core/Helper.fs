namespace Fooble.Core

open System

[<RequireQualifiedAccess>]
module internal Helper = 
    let checkNotNull<'T> (value : 'T) errorPrefix = 
        match box value with
        | null -> Seq.singleton (sprintf "%s should not be null" errorPrefix)
        | _ -> Seq.empty
    
    let checkNotEmptyString value errorPrefix = 
        match value with
        | "" -> Seq.singleton (sprintf "%s should not be empty" errorPrefix)
        | _ -> Seq.empty
    
    let checkNotEmptySequence<'T> (values : 'T seq) errorPrefix = 
        match box values with
        | null -> Seq.empty
        | _ -> 
            match Seq.isEmpty values with
            | true -> Seq.singleton (sprintf "%s should not be empty" errorPrefix)
            | _ -> Seq.empty
    
    let checkSequenceOfNotNull<'T> (values : 'T seq) errorPrefix = 
        match box values with
        | null -> Seq.empty
        | _ -> Seq.collect (fun v -> checkNotNull v errorPrefix) values
    
    let checkSequenceOfNotEmptyString values errorPrefix = 
        match box values with
        | null -> Seq.empty
        | _ -> Seq.collect (fun v -> checkNotEmptyString v errorPrefix) values
    
    let ensureValid<'T> validator (value : 'T) paramName = 
        let errors = validator value |> Seq.map (fun m -> (paramName, m))
        match Seq.tryHead errors with
        | Some(an, m) -> invalidArg an m
        | _ -> ()
