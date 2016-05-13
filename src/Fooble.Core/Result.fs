namespace Fooble.Core

type IResult<'TSuccess, 'TFailure> = 
    abstract Value : 'TSuccess
    abstract Status : 'TFailure
    abstract IsSuccess : bool
    abstract IsFailure : bool

[<RequireQualifiedAccess>]
module internal Result = 
    (* Active Patterns *)

    let internal (|IsSuccess|IsFailure|) (result : IResult<'TSuccess, 'TFailure>) = 
        if result.IsSuccess then Choice1Of2(result.Value)
        else Choice2Of2(result.Status)
    
    (* Result *)

    type private ResultImplementation<'TSuccess, 'TFailure> = 
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
    
    let internal success<'TSuccess, 'TFailure> (value : 'TSuccess) = 
        Validation.enforce (Validation.validateIsNotNullValue value "value" "Value")
        Success value :> IResult<'TSuccess, 'TFailure>
    
    let internal failure<'TSuccess, 'TFailure> (status : 'TFailure) = 
        Validation.enforce (Validation.validateIsNotNullValue status "status" "Status")
        Failure status :> IResult<'TSuccess, 'TFailure>
    
    (* Misc *)
    let internal ofOption (statusIfNone : 'TFailure) (valueOption : 'TSuccess option) = 
        match valueOption with
        | Some v -> success v
        | None -> failure statusIfNone
