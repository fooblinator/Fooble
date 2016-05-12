namespace Fooble.Core

open System

type IValidationFailureInfo = 
    abstract ParamName : string
    abstract Message : string

[<RequireQualifiedAccess>]
module internal Validation = 
    (* Checks *)

    let internal isNullValue (value : 'T) = isNull (box value)
    let internal isEmptyString value = value = ""
    let internal isNotGuidString value = not (fst (Guid.TryParse(value)))
    let internal containsNullValue (values : seq<'T>) = Option.isSome (Seq.tryFind (box >> isNull) values)
    let internal containsEmptyString values = Option.isSome (Seq.tryFind (fun x -> x = "") values)
    
    (* Failure Info *)

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
    
    let internal makeFailureInfo paramName message = 
        if isNullValue paramName then invalidArg "paramName" (sprintf "%s should not be null" "Param name")
        else if isEmptyString paramName then invalidArg "paramName" (sprintf "%s should not be empty" "Param name")
        else if isNullValue message then invalidArg "message" (sprintf "%s should not be null" "Message")
        else if isEmptyString message then invalidArg "message" (sprintf "%s should not be empty" "Message")
        else 
            { ValidationFailureInfoImplementation.ParamName = paramName
              Message = message } :> IValidationFailureInfo
    
    (* Misc *)

    let internal ensureIsValid (validator : 'T -> IValidationFailureInfo option) (value : 'T) = 
        match validator value with
        | Some x -> invalidArg x.ParamName x.Message
        | None -> ()
    
    let internal ensureNotNull (value : 'T) paramName errorPrefix = 
        if isNullValue value then invalidArg paramName (sprintf "%s should not be null" errorPrefix)
