namespace Fooble.Core

type IResult<'TSuccess, 'TFailure> = 
    abstract Value : 'TSuccess
    abstract Status : 'TFailure
    abstract IsSuccess : bool
    abstract IsFailure : bool

[<RequireQualifiedAccess>]
module Result = 
    let (|Success|Failure|) (result : IResult<'TSuccess, 'TFailure>) = 
        if result.IsSuccess then Choice1Of2(result.Value)
        else Choice2Of2(result.Status)
    
    type private Implementation<'TSuccess, 'TFailure> = 
        | Success of 'TSuccess
        | Failure of 'TFailure
        interface IResult<'TSuccess, 'TFailure> with
            
            member this.Value = 
                match this with
                | Success v -> v
                | Failure _ -> invalidOp "Result was not a success"
            
            member this.Status = 
                match this with
                | Failure s -> s
                | Success _ -> invalidOp "Result was not a failure"
            
            member this.IsFailure = 
                match this with
                | Failure _ -> true
                | _ -> false
            
            member this.IsSuccess = 
                match this with
                | Success _ -> true
                | _ -> false
    
    [<CompiledName("ValidateValue")>]
    let validateValue value = 
        if Validation.isNullValue value then 
            Some(ValidationFailureInfo.make "value" (sprintf "%s should not be null" "Value"))
        else None
    
    [<CompiledName("ValidateStatus")>]
    let validateStatus status = 
        if Validation.isNullValue status then 
            Some(ValidationFailureInfo.make "status" (sprintf "%s should not be null" "Status"))
        else None
    
    [<CompiledName("Success")>]
    let success (value : 'T) = 
        Validation.ensureValid validateValue value
        Success value :> IResult<'T, _>
    
    [<CompiledName("Failure")>]
    let failure (status : 'T) = 
        Validation.ensureValid validateStatus status
        Failure status :> IResult<_, 'T>
    
    let internal ofOption statusIfNone valueOption = 
        match valueOption with
        | Some v -> success v
        | None -> failure statusIfNone
