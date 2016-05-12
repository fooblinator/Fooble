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
    
    let (|Informational|Warning|Error|) (severity : IMessageDisplaySeverity) = 
        if severity.IsInformational then Choice1Of3()
        else if severity.IsWarning then Choice2Of3()
        else Choice3Of3()
    
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
    
    [<CompiledName("InformationalSeverity")>]
    let informationalSeverity : IMessageDisplaySeverity = Informational :> _
    
    [<CompiledName("WarningSeverity")>]
    let warningSeverity : IMessageDisplaySeverity = Warning :> _
    
    [<CompiledName("ErrorSeverity")>]
    let errorSeverity : IMessageDisplaySeverity = Error :> _
    
    [<CompiledName("MakeReadModel")>]
    let makeReadModel heading severity messages : IMessageDisplayReadModel = 
        Validation.ensureIsValid validateHeading heading
        Validation.ensureIsValid validateMessages messages
        match severity with
        | Informational -> 
            { Heading = heading
              Severity = informationalSeverity
              Messages = messages } :> _
        | Warning -> 
            { Heading = heading
              Severity = warningSeverity
              Messages = messages } :> _
        | Error -> 
            { Heading = heading
              Severity = errorSeverity
              Messages = messages } :> _
