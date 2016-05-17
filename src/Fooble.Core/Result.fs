namespace Fooble.Core

open System.Diagnostics

[<RequireQualifiedAccess>]
module internal Result =

    (* Implementation *)

    type private Implementation<'TSuccess,'TFailure when 'TSuccess : null and 'TFailure : null> =
        | Success of 'TSuccess
        | Failure of 'TFailure

        interface IResult<'TSuccess,'TFailure> with

            member this.Value =
                match this with
                | Success x -> x
                | _ -> invalidOp "Result was not a success"

            member this.Status =
                match this with
                | Failure x -> x
                | _ -> invalidOp "Result was not a failure"

            member this.IsSuccess =
                match this with
                | Success _ -> true
                | _ -> false

            member this.IsFailure =
                match this with
                | Failure _ -> true
                | _ -> false

    (* Construction *)

    let internal success value =
        Debug.Assert(notIsNull value, "(Result.success) value argument was null")
        Success value :> IResult<'TSuccess,'TFailure>

    let internal failure status =
        Debug.Assert(notIsNull status, "(Result.failure) status argument was null")
        Failure status :> IResult<'TSuccess,'TFailure>

[<AutoOpen>]
module internal ResultLibrary =

    (* Active Patterns *)

    let internal (|IsSuccess|IsFailure|) (result:IResult<'TSuccess,'TFailure>) =
        if result.IsSuccess
            then Choice1Of2 result.Value
            else Choice2Of2 result.Status

    (* Misc *)

    let internal failureIfNone statusIfNone valueOption =
        match valueOption with
        | Some v -> Result.success v
        | None -> Result.failure statusIfNone
