namespace Fooble.Core

open System

[<RequireQualifiedAccess>]
module Member = 
    [<CompiledName("ValidateId")>]
    let validateId id = 
        if Validation.isNullValue id then Some(ValidationFailureInfo.make "id" (sprintf "%s should not be null" "Id"))
        else if Validation.isEmptyString id then 
            Some(ValidationFailureInfo.make "id" (sprintf "%s should not be empty" "Id"))
        else if Validation.isNotGuidString id then 
            Some(ValidationFailureInfo.make "id" (sprintf "%s should be in the proper GUID format" "Id"))
        else None
    
    [<CompiledName("ValidateName")>]
    let validateName name = 
        if Validation.isNullValue name then 
            Some(ValidationFailureInfo.make "name" (sprintf "%s should not be null" "Name"))
        else if Validation.isEmptyString name then 
            Some(ValidationFailureInfo.make "name" (sprintf "%s should not be empty" "Name"))
        else None
