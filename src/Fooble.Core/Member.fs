namespace Fooble.Core

open System

[<RequireQualifiedAccess>]
module Member = 
    (* Validators *)

    [<CompiledName("ValidateId")>]
    let validateId id = 
        [ Validation.validateIsNotNullValue; Validation.validateIsNotEmptyString; Validation.validateIsGuidString ] 
        |> List.tryPick (fun fn -> fn id "id" "Id")
    
    [<CompiledName("ValidateName")>]
    let validateName name = 
        [ Validation.validateIsNotNullValue; Validation.validateIsNotEmptyString ] 
        |> List.tryPick (fun fn -> fn name "name" "Name")
