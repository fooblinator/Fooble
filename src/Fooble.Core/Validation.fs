namespace Fooble.Core

open System

[<AllowNullLiteral>]
type IValidationInfo =
    abstract ParamName:string
    abstract Message:string

[<RequireQualifiedAccess>]
module internal Validation =

    (* Failure Info *)

    type private ValidationInfoImplementation =
        { ParamName:string; Message:string }

        interface IValidationInfo with

            member this.ParamName =

                let contracts =
                    [ postCondition (isNullValue >> not) "IValidationInfo.ParamName property returned null value"
                      postCondition (isEmptyString >> not) "IValidationInfo.ParamName property returned empty string" ]

                let body x =
                    match x with
                    | { ValidationInfoImplementation.ParamName = pn; Message = _ } -> pn

                ensure contracts body this

            member this.Message =

                let contracts =
                    [ postCondition (isNullValue >> not) "IValidationInfo.Message property returned null value"
                      postCondition (isEmptyString >> not) "IValidationInfo.Message property returned empty string" ]

                let body x =
                    match x with
                    | { ValidationInfoImplementation.ParamName = _; Message = m } -> m

                ensure contracts body this

    let internal makeInfo paramName message =

        let contracts =
            [ preCondition (fst >> isNullValue >> not) "Validation.makeInfo paramName argument was null value"
              preCondition (fst >> isEmptyString >> not) "Validation.makeInfo paramName argument was empty string"
              preCondition (snd >> isNullValue >> not) "Validation.makeInfo message argument was null value"
              preCondition (snd >> isEmptyString >> not) "Validation.makeInfo message argument was empty string"
              postCondition (isNullValue >> not) "Validation.makeInfo returned null value" ]

        let body (x, y) =
            { ValidationInfoImplementation.ParamName = x; Message = y } :> IValidationInfo

        ensure contracts body (paramName, message)

    (* Validators *)

    let internal validateIsNotNullValue (value:'T) paramName prefix =
        match isNullValue value with
        | true -> Some(makeInfo paramName (sprintf "%s was null value" prefix))
        | _ -> None

    let internal validateIsNotEmptyString value paramName prefix =
        match isEmptyString value with
        | true -> Some(makeInfo paramName (sprintf "%s was empty string" prefix))
        | _ -> None

    let internal validateIsGuidString value paramName prefix =
        match not (isGuidString value) with
        | true -> Some(makeInfo paramName (sprintf "%s was string with invalid GUID format" prefix))
        | _ -> None

    let internal validateIsNotEmptyValue (value:seq<'T>) paramName prefix =
        match isEmptyValue value with
        | true -> Some(makeInfo paramName (sprintf "%s was empty value" prefix))
        | _ -> None

    let internal validateContainsNotNullValues (values:seq<'T>) paramName prefix =
        match containsNullValues values with
        | true -> Some(makeInfo paramName (sprintf "%s contained null values" prefix))
        | _ -> None

    let internal validateContainsNotEmptyStrings values paramName prefix =
        match containsEmptyStrings values with
        | true -> Some(makeInfo paramName (sprintf "%s contained empty strings" prefix))
        | _ -> None

    (* Misc *)

    let internal enforce (validationInfo:IValidationInfo option) =
        match validationInfo with
        | Some i -> invalidArg i.ParamName i.Message
        | None -> ()
