namespace Fooble.Core

open System

type IValidationFailureInfo = 
    abstract ParamName : string
    abstract Message : string

[<RequireQualifiedAccess>]
module internal ValidationFailureInfo = 
    type private Implementation = 
        { ParamName : string
          Message : string }
        interface IValidationFailureInfo with
            
            member this.ParamName = 
                match this with
                | { ParamName = x; Message = _ } -> x
            
            member this.Message = 
                match this with
                | { ParamName = _; Message = x } -> x
    
    let ensureParamName paramName = 
        match paramName with
        | x when isNull (box x) -> invalidArg "paramName" (sprintf "%s should not be null" "Param name")
        | x when Seq.isEmpty x -> invalidArg "paramName" (sprintf "%s should not be empty" "Param name")
        | _ -> ()
    
    let ensureMessage message = 
        match message with
        | x when isNull (box x) -> invalidArg "message" (sprintf "%s should not be null" "Message")
        | x when Seq.isEmpty x -> invalidArg "message" (sprintf "%s should not be empty" "Message")
        | _ -> ()
    
    let make paramName message : IValidationFailureInfo = 
        ensureParamName paramName
        ensureMessage message
        { ParamName = paramName
          Message = message } :> _

[<RequireQualifiedAccess>]
module internal Validation = 
    let ensureValid (validator : 'T -> IValidationFailureInfo option) (value : 'T) = 
        match validator value with
        | Some x -> invalidArg x.ParamName x.Message
        | None -> ()
    
    let isNullValue value = isNull (box value)
    let isEmptyString value = value = ""
    let isNotGuidString value = not (fst (Guid.TryParse(value)))
    let containsNullValue values = Option.isSome (Seq.tryFind (box >> isNull) values)
    let containsEmptyString values = Option.isSome (Seq.tryFind (fun x -> x = "") values)
