namespace Fooble.Core

type IMessageDisplaySeverity = 
    abstract IsInformational : bool
    abstract IsWarning : bool
    abstract IsError : bool

type IMessageDisplayReadModel = 
    abstract Heading : string
    abstract Severity : IMessageDisplaySeverity
    abstract Messages : string seq

[<RequireQualifiedAccess>]
module MessageDisplaySeverity = 
    let (|Informational|Warning|Error|) (severity : IMessageDisplaySeverity) = 
        if severity.IsInformational then Choice1Of3()
        else if severity.IsWarning then Choice2Of3()
        else Choice3Of3()
    
    type private Implementation = 
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
    
    [<CompiledName("Informational")>]
    let informational : IMessageDisplaySeverity = Informational :> _
    
    [<CompiledName("Warning")>]
    let warning : IMessageDisplaySeverity = Warning :> _
    
    [<CompiledName("Error")>]
    let error : IMessageDisplaySeverity = Error :> _

[<RequireQualifiedAccess>]
module MessageDisplayReadModel = 
    type private Implementation = 
        { Heading : string
          Severity : IMessageDisplaySeverity
          Messages : string seq }
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
    
    [<CompiledName("ValidateHeading")>]
    let validateHeading heading = 
        seq { 
            yield Helper.checkNotNull heading "Heading"
            yield Helper.checkNotEmptyString heading "Heading"
        }
        |> Seq.concat
    
    [<CompiledName("ValidateMessages")>]
    let validateMessages messages = 
        seq { 
            yield Helper.checkNotNull messages "Message list"
            yield Helper.checkNotEmptySequence messages "Message list"
            yield Helper.checkSequenceOfNotNull messages "Message list item"
            yield Helper.checkSequenceOfNotEmptyString messages "Message list item"
        }
        |> Seq.concat
    
    [<CompiledName("Make")>]
    let make heading severity messages : IMessageDisplayReadModel = 
        Helper.ensureValid validateHeading heading "heading"
        Helper.ensureValid validateMessages messages "messages"
        match severity with
        | MessageDisplaySeverity.Informational -> 
            { Heading = heading
              Severity = MessageDisplaySeverity.informational
              Messages = messages } :> _
        | MessageDisplaySeverity.Warning -> 
            { Heading = heading
              Severity = MessageDisplaySeverity.warning
              Messages = messages } :> _
        | MessageDisplaySeverity.Error -> 
            { Heading = heading
              Severity = MessageDisplaySeverity.error
              Messages = messages } :> _
