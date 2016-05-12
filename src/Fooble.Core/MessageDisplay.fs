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
        if Validation.isNullValue heading then 
            Some(Validation.makeFailureInfo "heading" (sprintf "%s should not be null" "Heading"))
        else if Validation.isEmptyString heading then 
            Some(Validation.makeFailureInfo "heading" (sprintf "%s should not be empty" "Heading"))
        else None
    
    [<CompiledName("ValidateMessages")>]
    let validateMessages messages = 
        if Validation.isNullValue messages then 
            Some(Validation.makeFailureInfo "messages" (sprintf "%s should not be null" "Message list"))
        else if Seq.isEmpty messages then 
            Some(Validation.makeFailureInfo "messages" (sprintf "%s should not be empty" "Message list"))
        else if Validation.containsNullValue messages then 
            Some(Validation.makeFailureInfo "messages" (sprintf "%s should not be null" "Message list items"))
        else if Validation.containsEmptyString messages then 
            Some(Validation.makeFailureInfo "messages" (sprintf "%s should not be empty" "Message list items"))
        else None
    
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
        Validation.ensureIsValid validateHeading heading
        Validation.ensureIsValid validateMessages messages
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
