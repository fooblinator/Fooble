namespace Fooble.Core

open System

type IValidationFailureInfo = 
    abstract ParamName : string
    abstract Message : string

[<RequireQualifiedAccess>]
module internal Validation = 
    type private ValidationFailureInfoImplementation = 
        { ParamName : string
          Message : string }
        interface IValidationFailureInfo with
            
            member this.ParamName = 
                match this with
                | { ParamName = x; Message = _ } -> x
            
            member this.Message = 
                match this with
                | { ParamName = _; Message = x } -> x
    
    let ensureIsValid (validator : 'T -> IValidationFailureInfo option) (value : 'T) = 
        match validator value with
        | Some x -> invalidArg x.ParamName x.Message
        | None -> ()
    
    let isNullValue value = isNull (box value)
    let isEmptyString value = value = ""
    let isNotGuidString value = not (fst (Guid.TryParse(value)))
    let containsNullValue values = Option.isSome (Seq.tryFind (box >> isNull) values)
    let containsEmptyString values = Option.isSome (Seq.tryFind (fun x -> x = "") values)
    
    let makeFailureInfo paramName message : IValidationFailureInfo = 
        if isNullValue paramName then invalidArg "paramName" (sprintf "%s should not be null" "Param name")
        else if isEmptyString paramName then invalidArg "paramName" (sprintf "%s should not be empty" "Param name")
        else if isNullValue message then invalidArg "message" (sprintf "%s should not be null" "Message")
        else if isEmptyString message then invalidArg "message" (sprintf "%s should not be empty" "Message")
        else 
            { ParamName = paramName
              Message = message } :> _
