[<AutoOpen>]
module internal Fooble.Core.Result

(* Active Patterns *)

let internal (|IsSuccess|IsFailure|) (result:IResult<'TSuccess,'TFailure>) =
    if result.IsSuccess
        then Choice1Of2 result.Value
        else Choice2Of2 result.Status

(* Result *)

type private ResultImplementation<'TSuccess,'TFailure when 'TSuccess : null and 'TFailure : null> =
    | Success of 'TSuccess
    | Failure of 'TFailure

    interface IResult<'TSuccess,'TFailure> with

        member this.Value =
            match this with
            | ResultImplementation.Success v -> v
            | _ -> invalidOp "Result was not a success"

        member this.Status =
            match this with
            | ResultImplementation.Failure s -> s
            | _ -> invalidOp "Result was not a failure"

        member this.IsFailure =
            match this with
            | ResultImplementation.Failure _ -> true
            | _ -> false

        member this.IsSuccess =
            match this with
            | ResultImplementation.Success _ -> true
            | _ -> false

let internal successResult value =

    let contracts =
        [ preCondition (isNullValue >> not) "(Result) success value argument was null value"
          postCondition (isNullValue >> not) "(Result) success returned null value" ]

    let body x =
        ResultImplementation.Success value :> IResult<'TSuccess,'TFailure>

    ensure contracts body value

let internal failureResult status =

    let contracts =
        [ preCondition (isNullValue >> not) "(Result) failure status argument was null value"
          postCondition (isNullValue >> not) "(Result) failure returned null value" ]

    let body x =
        ResultImplementation.Failure status :> IResult<'TSuccess,'TFailure>

    ensure contracts body status

(* Misc *)

let internal failureIfNone statusIfNone valueOption =
    match valueOption with
    | Some v -> successResult v
    | None -> failureResult statusIfNone
