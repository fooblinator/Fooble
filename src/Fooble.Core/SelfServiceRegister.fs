namespace Fooble.Core

open Fooble.Core.Persistence
open System
open System.Diagnostics

[<RequireQualifiedAccess>]
module SelfServiceRegister =

    (* Validation *)

    [<CompiledName("ValidateName")>]
    let validateName name =
        [ (notIsNull), "Name parameter was null"
          (String.notIsEmpty), "Name parameter was empty string" ]
        |> Validation.validate name "name"

    (* Command *)

    [<DefaultAugmentation(false)>]
    type private Command =
        | Command of Guid * string

        member this.Id =
            match this with
            | Command (x, _) -> x

        member this.Name =
            match this with
            | Command (_, x) -> x

        interface ISelfServiceRegisterCommand with
            member this.Id = this.Id
            member this.Name = this.Name

    [<CompiledName("MakeCommand")>]
    let makeCommand id name =
        Validation.raiseIfInvalid <| validateName name
        Command (id, name) :> ISelfServiceRegisterCommand

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

    (* Command Result *)

    [<DefaultAugmentation(false)>]
    type private CommandResult =
        | Success'
        | DuplicateId'

        member this.IsSuccess =
            match this with
            | Success' _ -> true
            | DuplicateId' -> false

        member this.IsDuplicateId =
            match this with
            | Success' _ -> false
            | DuplicateId' -> true

        interface ISelfServiceRegisterCommandResult with
            member this.IsSuccess = this.IsSuccess
            member this.IsDuplicateId = this.IsDuplicateId

    let internal successResult = Success' :> ISelfServiceRegisterCommandResult
    let internal duplicateIdResult = DuplicateId' :> ISelfServiceRegisterCommandResult

    (* Command Handler *)

    [<DefaultAugmentation(false)>]
    type private CommandHandler =
        | CommandHandler of IFoobleContext

        member this.Context =
            match this with
            | CommandHandler x -> x

        interface ISelfServiceRegisterCommandHandler with

            member this.Handle(command) =
                Debug.Assert(notIsNull <| box command, "Command parameter was null")
                Seq.tryFind (fun (x:MemberData) -> x.Id = command.Id) this.Context.MemberData
                |> function
                    | Some _ -> duplicateIdResult
                    | None ->
                        MemberData(Id = command.Id, Name = command.Name)
                        |> this.Context.MemberData.AddObject
                        // TODO: research what could go wrong here and handle appropriately
                        ignore <| this.Context.SaveChanges()
                        successResult

    let internal makeCommandHandler context =
        Debug.Assert(notIsNull context, "Context parameter was null")
        CommandHandler context :> ISelfServiceRegisterCommandHandler

    (* Active Patterns *)

    let internal (|IsSuccess|IsDuplicateId|) (result:ISelfServiceRegisterCommandResult) =
        if result.IsSuccess
            then Choice1Of2 ()
            else Choice2Of2 ()
