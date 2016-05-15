[<AutoOpen>]
module internal Fooble.Core.Validation

open System

(* Failure Info *)

type private ValidationInfoImplementation =
    { paramName:string; message:string }

    interface IValidationInfo with

        member this.ParamName =

            let contracts =
                [ postCondition (isNullValue >> not) "(IValidationInfo) ParamName property returned null value"
                  postCondition (isEmptyString >> not) "(IValidationInfo) ParamName property returned empty string" ]

            let body x =
                match x with
                | { ValidationInfoImplementation.paramName = pn; message = _ } -> pn

            ensure contracts body this

        member this.Message =

            let contracts =
                [ postCondition (isNullValue >> not) "(IValidationInfo) Message property returned null value"
                  postCondition (isEmptyString >> not) "(IValidationInfo) Message property returned empty string" ]

            let body x =
                match x with
                | { ValidationInfoImplementation.paramName = _; message = m } -> m

            ensure contracts body this

let internal makeValidationInfo paramName message =

    let contracts =
        [ preCondition (fst >> isNullValue >> not) "(Validation) makeValidationInfo paramName argument was null value"
          preCondition (fst >> isEmptyString >> not) "(Validation) makeValidationInfo paramName argument was empty string"
          preCondition (snd >> isNullValue >> not) "(Validation) makeValidationInfo message argument was null value"
          preCondition (snd >> isEmptyString >> not) "(Validation) makeValidationInfo message argument was empty string"
          postCondition (isNullValue >> not) "(Validation) makeValidationInfo returned null value" ]

    let body (x, y) =
        { ValidationInfoImplementation.paramName = x; message = y } :> IValidationInfo

    ensure contracts body (paramName, message)

(* Validators *)

let internal validateIsNotNullValue (value:'T) paramName prefix =
    match isNullValue value with
    | true -> Some(makeValidationInfo paramName (sprintf "%s was null value" prefix))
    | _ -> None

let internal validateIsNotEmptyString value paramName prefix =
    match isEmptyString value with
    | true -> Some(makeValidationInfo paramName (sprintf "%s was empty string" prefix))
    | _ -> None

let internal validateIsGuidString value paramName prefix =
    match not (isGuidString value) with
    | true -> Some(makeValidationInfo paramName (sprintf "%s was string with invalid GUID format" prefix))
    | _ -> None

let internal validateIsNotEmptyValue (value:seq<'T>) paramName prefix =
    match isEmptyValue value with
    | true -> Some(makeValidationInfo paramName (sprintf "%s was empty value" prefix))
    | _ -> None

let internal validateContainsNotNullValues (values:seq<'T>) paramName prefix =
    match containsNullValues values with
    | true -> Some(makeValidationInfo paramName (sprintf "%s contained null values" prefix))
    | _ -> None

let internal validateContainsNotEmptyStrings values paramName prefix =
    match containsEmptyStrings values with
    | true -> Some(makeValidationInfo paramName (sprintf "%s contained empty strings" prefix))
    | _ -> None

(* Misc *)

let internal enforce (validationInfo:IValidationInfo option) =
    match validationInfo with
    | Some i -> invalidArg i.ParamName i.Message
    | None -> ()
