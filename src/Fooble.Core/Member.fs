namespace Fooble.Core

open System

[<RequireQualifiedAccess>]
module Member = 
    (* Validators *)

    [<CompiledName("ValidateId")>]
    let validateId id = 
        if Validation.isNullValue id then Some(Validation.makeFailureInfo "id" (sprintf "%s should not be null" "Id"))
        else if Validation.isEmptyString id then 
            Some(Validation.makeFailureInfo "id" (sprintf "%s should not be empty" "Id"))
        else if Validation.isNotGuidString id then 
            Some(Validation.makeFailureInfo "id" (sprintf "%s should be in the proper GUID format" "Id"))
        else None
    
    [<CompiledName("ValidateName")>]
    let validateName name = 
        if Validation.isNullValue name then 
            Some(Validation.makeFailureInfo "name" (sprintf "%s should not be null" "Name"))
        else if Validation.isEmptyString name then 
            Some(Validation.makeFailureInfo "name" (sprintf "%s should not be empty" "Name"))
        else None
