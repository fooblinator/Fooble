namespace Fooble.Core

open MediatR
open System.Runtime.CompilerServices

type IMemberListItemReadModel = 
    abstract Id : string
    abstract Name : string

type IMemberListReadModel = 
    abstract Members : IMemberListItemReadModel seq

type IMemberListQueryFailureStatus = 
    abstract IsNotFound : bool

type IMemberListQuery = 
    inherit IRequest<IResult<IMemberListReadModel, IMemberListQueryFailureStatus>>

type IMemberListQueryHandler = IRequestHandler<IMemberListQuery, IResult<IMemberListReadModel, IMemberListQueryFailureStatus>>

[<RequireQualifiedAccess; Extension>]
module MemberList = 
    (* Query *)

    type private MemberListQueryImplementation() = 
        interface IMemberListQuery
    
    [<CompiledName("MakeQuery")>]
    let makeQuery() : IMemberListQuery = MemberListQueryImplementation() :> _
    
    (* Item Read Model *)

    type private MemberListItemReadModelImplementation = 
        { Id : string
          Name : string }
        interface IMemberListItemReadModel with
            
            member this.Id = 
                match this with
                | { Id = i; Name = _ } -> i
            
            member this.Name = 
                match this with
                | { Id = _; Name = n } -> n
    
    let internal makeItemReadModel id name : IMemberListItemReadModel = 
        Validation.ensureIsValid Member.validateId id
        Validation.ensureIsValid Member.validateName name
        { MemberListItemReadModelImplementation.Id = id
          Name = name } :> _
    
    (* Read Model *)

    type private MemberListReadModelImplementation = 
        { Members : IMemberListItemReadModel seq }
        interface IMemberListReadModel with
            member this.Members = 
                match this with
                | { Members = ms } -> ms
    
    let internal validateMembers (members : IMemberListItemReadModel seq) = 
        if Validation.isNullValue members then 
            Some(Validation.makeFailureInfo "members" (sprintf "%s should not be null" "Member list"))
        else if Seq.isEmpty members then 
            Some(Validation.makeFailureInfo "members" (sprintf "%s should not be empty" "Member list"))
        else if Validation.containsNullValue members then 
            Some(Validation.makeFailureInfo "members" (sprintf "%s should not be null" "Member list items"))
        else None
    
    let internal makeReadModel members : IMemberListReadModel = 
        Validation.ensureIsValid validateMembers members
        { MemberListReadModelImplementation.Members = members } :> _
    
    (* Query Failure Status *)

    type private MemberListQueryFailureStatusImplementation = 
        | NotFound
        interface IMemberListQueryFailureStatus with
            member this.IsNotFound = 
                match this with
                | NotFound -> true
    
    [<CompiledName("NotFoundQueryFailureStatus")>]
    let notFoundQueryFailureStatus : IMemberListQueryFailureStatus = NotFound :> _
    
    let (|NotFound|) (status : IMemberListQueryFailureStatus) = ()
    
    (* Query Handler *)

    type private MemberListQueryHandlerImplementation(context : IDataContext) = 
        interface IMemberListQueryHandler with
            member this.Handle(query) = 
                Validation.ensureNotNull query "query" "Query"
                match List.ofSeq context.Members with
                | [] -> Result.failure notFoundQueryFailureStatus
                | mds -> 
                    List.map (fun (md : MemberData) -> makeItemReadModel md.Id md.Name) mds
                    |> Seq.ofList
                    |> makeReadModel
                    |> Result.success
    
    let internal makeQueryHandler context : IMemberListQueryHandler = 
        Validation.ensureNotNull context "context" "Data context"
        MemberListQueryHandlerImplementation(context) :> _
    
    (* Extensions *)

    [<CompiledName("ToMessageDisplayReadModel"); Extension>]
    let toMessageDisplayReadModel (result : IResult<IMemberListReadModel, IMemberListQueryFailureStatus>) : IMessageDisplayReadModel = 
        let h = "Member List Query"
        let ss = MessageDisplay.informationalSeverity
        let sm = "Member list query was successful"
        let fs = MessageDisplay.errorSeverity
        let fm = "Member list query was not successful and returned not found"
        match result with
        | Result.Success _ -> MessageDisplay.makeReadModel h ss (Seq.singleton sm)
        | Result.Failure s -> 
            match s with
            | NotFound -> MessageDisplay.makeReadModel h fs (Seq.singleton fm)
