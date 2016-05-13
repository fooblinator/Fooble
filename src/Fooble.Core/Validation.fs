namespace Fooble.Core

open System

type IValidationInfo = 
    abstract ParamName : string
    abstract Message : string

[<RequireQualifiedAccess>]
module internal Validation = 
    (* Helpers *)

    let private isNullValue (value : 'T) = isNull (box value)
    let private isEmptyString value = value = ""
    let private isNotGuidString value = not (fst (Guid.TryParse(value)))
    let private isEmptyValue (value : seq<'T>) = Seq.isEmpty value
    let private containsNullValues (values : seq<'T>) = Option.isSome (Seq.tryFind (box >> isNull) values)
    let private containsEmptyStrings values = Option.isSome (Seq.tryFind (fun x -> x = "") values)
    let private shouldNotBeNullMessage prefix = sprintf "%s should not be null" prefix
    let private shouldNotBeEmptyMessage prefix = sprintf "%s should not be empty" prefix
    let private shouldBeGuidFormatMessage prefix = sprintf "%s should be in the proper GUID format" prefix
    
    (* Failure Info *)

    type private ValidationInfoImplementation = 
        { ParamName : string
          Message : string }
        interface IValidationInfo with
            
            member this.ParamName = 
                match this with
                | { ParamName = x; Message = _ } -> x
            
            member this.Message = 
                match this with
                | { ParamName = _; Message = x } -> x
    
    let internal makeInfo paramName message = 
        if isNullValue paramName then invalidArg "paramName" (shouldNotBeNullMessage "Param name")
        if isEmptyString paramName then invalidArg "paramName" (shouldNotBeEmptyMessage "Param name")
        if isNullValue message then invalidArg "message" (shouldNotBeNullMessage "Message")
        if isEmptyString message then invalidArg "message" (shouldNotBeEmptyMessage "Message")
        { ValidationInfoImplementation.ParamName = paramName
          Message = message } :> IValidationInfo
    
    (* Validators *)

    let internal validateIsNotNullValue (value : 'T) paramName prefix = 
        match isNullValue value with
        | true -> Some(makeInfo paramName (shouldNotBeNullMessage prefix))
        | _ -> None
    
    let internal validateIsNotEmptyString value paramName prefix = 
        match isEmptyString value with
        | true -> Some(makeInfo paramName (shouldNotBeEmptyMessage prefix))
        | _ -> None
    
    let internal validateIsGuidString value paramName prefix = 
        match isNotGuidString value with
        | true -> Some(makeInfo paramName (shouldBeGuidFormatMessage prefix))
        | _ -> None
    
    let internal validateIsNotEmptyValue (value : seq<'T>) paramName prefix = 
        match isEmptyValue value with
        | true -> Some(makeInfo paramName (shouldNotBeEmptyMessage prefix))
        | _ -> None
    
    let internal validateContainsNotNullValues (values : seq<'T>) paramName prefix = 
        match containsNullValues values with
        | true -> Some(makeInfo paramName (shouldNotBeNullMessage prefix))
        | _ -> None
    
    let internal validateContainsNotEmptyStrings values paramName prefix = 
        match containsEmptyStrings values with
        | true -> Some(makeInfo paramName (shouldNotBeEmptyMessage prefix))
        | _ -> None
    
    (* Misc *)

    let internal enforce (validationInfo : IValidationInfo option) = 
        match validationInfo with
        | Some i -> invalidArg i.ParamName i.Message
        | None -> ()
