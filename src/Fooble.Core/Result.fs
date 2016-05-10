namespace Fooble.Core

type IResult<'TSuccess, 'TFailure> = 
    abstract Value : 'TSuccess
    abstract Status : 'TFailure
    abstract IsSuccess : bool
    abstract IsFailure : bool

[<RequireQualifiedAccess>]
module Result = 
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
    let validateValue<'TSuccess> (value : 'TSuccess) = Helper.checkNotNull value "Value"
    
    [<CompiledName("ValidateStatus")>]
    let validateStatus<'TFailure> (status : 'TFailure) = Helper.checkNotNull status "Status"
    
    [<CompiledName("Success")>]
    let success<'TSuccess, 'TFailure> (value : 'TSuccess) : IResult<_, 'TFailure> = 
        Helper.ensureValid validateValue value "value"
        Success value :> _
    
    [<CompiledName("Failure")>]
    let failure<'TSuccess, 'TFailure> (status : 'TFailure) : IResult<'TSuccess, _> = 
        Helper.ensureValid validateStatus status "status"
        Failure status :> _
