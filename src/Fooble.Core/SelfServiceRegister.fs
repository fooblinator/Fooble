namespace Fooble.Core

open System.Diagnostics

[<RequireQualifiedAccess>]
module SelfServiceRegister =

    (* Validation *)

    [<CompiledName("ValidateName")>]
    let validateName name =
        [ (notIsNull), "Name parameter was null"
          (String.notIsEmpty), "Name parameter was empty string" ]
        |> Validation.validate name "name"

    (* Read Model *)

    [<DefaultAugmentation(false)>]
    type private ReadModel =
        | ReadModel of string

        member this.Name =
            match this with
            | ReadModel x -> x

        interface ISelfServiceRegisterReadModel with
            member this.Name = this.Name
    
    [<CompiledName("MakeInitialReadModel")>]
    let makeInitialReadModel () =
        ReadModel String.empty :> ISelfServiceRegisterReadModel
    
    [<CompiledName("MakeReadModel")>]
    let makeReadModel name =
        Validation.raiseIfInvalid <| validateName name
        ReadModel name :> ISelfServiceRegisterReadModel
