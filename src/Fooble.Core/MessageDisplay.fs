namespace Fooble.Core

type IMessageDisplaySeverity = 
    abstract IsInformational : bool
    abstract IsWarning : bool
    abstract IsError : bool

type IMessageDisplayReadModel = 
    abstract Heading : string
    abstract Severity : IMessageDisplaySeverity
    abstract Messages : seq<string>

[<RequireQualifiedAccess>]
module MessageDisplay = 
    (* Active Patterns *)

    let internal (|IsInformational|IsWarning|IsError|) (severity : IMessageDisplaySeverity) = 
        if severity.IsInformational then Choice1Of3()
        else if severity.IsWarning then Choice2Of3()
        else Choice3Of3()
    
    (* Validators *)

    [<CompiledName("ValidateHeading")>]
    let validateHeading heading = 
        [ Validation.validateIsNotNullValue; Validation.validateIsNotEmptyString ] 
        |> List.tryPick (fun fn -> fn heading "heading" "Heading")
    
    [<CompiledName("ValidateMessages")>]
    let validateMessages messages = 
        seq { 
            yield [ Validation.validateIsNotNullValue; Validation.validateIsNotEmptyValue ] 
                  |> List.tryPick (fun fn -> fn messages "messages" "Message list")
            yield [ Validation.validateContainsNotNullValues; Validation.validateContainsNotEmptyStrings ] 
                  |> List.tryPick (fun fn -> fn messages "messages" "Message list items")
        }
        |> Seq.tryPick Operators.id

    (* Severity *)

    type private MessageDisplaySeverityImplementation = 
        | Informational
        | Warning
        | Error
        interface IMessageDisplaySeverity with
            
            member this.IsInformational = 
                match this with
                | Informational -> true
                | _ -> false
            
            member this.IsWarning = 
                match this with
                | Warning -> true
                | _ -> false
            
            member this.IsError = 
                match this with
                | Error -> true
                | _ -> false
    
    [<CompiledName("InformationalSeverity")>]
    let informationalSeverity = Informational :> IMessageDisplaySeverity
    
    [<CompiledName("WarningSeverity")>]
    let warningSeverity = Warning :> IMessageDisplaySeverity
    
    [<CompiledName("ErrorSeverity")>]
    let errorSeverity = Error :> IMessageDisplaySeverity
    
    (* Read Model *)

    type private MessageDisplayReadModelImplementation = 
        { Heading : string
          Severity : IMessageDisplaySeverity
          Messages : seq<string> }
        interface IMessageDisplayReadModel with
            
            member this.Heading = 
                match this with
                | { Heading = h; Severity = _; Messages = _ } -> h
            
            member this.Severity = 
                match this with
                | { Heading = _; Severity = s; Messages = _ } -> s
            
            member this.Messages = 
                match this with
                | { Heading = _; Severity = _; Messages = ms } -> ms
    
    [<CompiledName("MakeReadModel")>]
    let makeReadModel heading severity messages = 
        Validation.ensure (validateHeading heading)
        Validation.ensure (validateMessages messages)
        match severity with
        | IsInformational -> 
            { MessageDisplayReadModelImplementation.Heading = heading
              Severity = informationalSeverity
              Messages = messages } :> IMessageDisplayReadModel
        | IsWarning -> 
            { MessageDisplayReadModelImplementation.Heading = heading
              Severity = warningSeverity
              Messages = messages } :> IMessageDisplayReadModel
        | IsError -> 
            { MessageDisplayReadModelImplementation.Heading = heading
              Severity = errorSeverity
              Messages = messages } :> IMessageDisplayReadModel
