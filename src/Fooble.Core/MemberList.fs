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
    (* Active Patterns *)

    let internal (|IsNotFound|) (status : IMemberListQueryFailureStatus) = ()
    
    (* Validators *)

    let internal validateMembers (members : seq<IMemberListItemReadModel>) = 
        if Validation.isNullValue members then 
            Some(Validation.makeFailureInfo "members" (sprintf "%s should not be null" "Member list"))
        else if Seq.isEmpty members then 
            Some(Validation.makeFailureInfo "members" (sprintf "%s should not be empty" "Member list"))
        else if Validation.containsNullValue members then 
            Some(Validation.makeFailureInfo "members" (sprintf "%s should not be null" "Member list items"))
        else None
    
    (* Query *)

    type private MemberListQueryImplementation() = 
        interface IMemberListQuery
    
    [<CompiledName("MakeQuery")>]
    let makeQuery() = MemberListQueryImplementation() :> IMemberListQuery
    
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
    
    let internal makeItemReadModel id name = 
        Validation.ensureIsValid Member.validateId id
        Validation.ensureIsValid Member.validateName name
        { MemberListItemReadModelImplementation.Id = id
          Name = name } :> IMemberListItemReadModel
    
    (* Read Model *)

    type private MemberListReadModelImplementation = 
        { Members : IMemberListItemReadModel seq }
        interface IMemberListReadModel with
            member this.Members = 
                match this with
                | { Members = ms } -> ms
    
    let internal makeReadModel members = 
        Validation.ensureIsValid validateMembers members
        { MemberListReadModelImplementation.Members = members } :> IMemberListReadModel
    
    (* Query Failure Status *)

    type private MemberListQueryFailureStatusImplementation = 
        | NotFound
        interface IMemberListQueryFailureStatus with
            member this.IsNotFound = 
                match this with
                | NotFound -> true
    
    [<CompiledName("NotFoundQueryFailureStatus")>]
    let notFoundQueryFailureStatus = NotFound :> IMemberListQueryFailureStatus
    
    (* Query Handler *)

    type private MemberListQueryHandlerImplementation(context : IDataContext) = 
        interface IMemberListQueryHandler with
            member this.Handle(query : IMemberListQuery) = 
                Validation.ensureNotNull query "query" "Query"
                match List.ofSeq context.Members with
                | [] -> Result.failure notFoundQueryFailureStatus
                | mds -> 
                    List.map (fun (md : MemberData) -> makeItemReadModel md.Id md.Name) mds
                    |> Seq.ofList
                    |> makeReadModel
                    |> Result.success
    
    let internal makeQueryHandler (context : IDataContext) = 
        Validation.ensureNotNull context "context" "Data context"
        MemberListQueryHandlerImplementation(context) :> IMemberListQueryHandler
    
    (* Extensions *)

    [<CompiledName("ToMessageDisplayReadModel"); Extension>]
    let toMessageDisplayReadModel (result : IResult<IMemberListReadModel, IMemberListQueryFailureStatus>) = 
        let h = "Member List Query"
        let ss = MessageDisplay.informationalSeverity
        let sm = "Member list query was successful"
        let fs = MessageDisplay.errorSeverity
        let fm = "Member list query was not successful and returned not found"
        match result with
        | Result.IsSuccess _ -> MessageDisplay.makeReadModel h ss (Seq.singleton sm)
        | Result.IsFailure s -> 
            match s with
            | IsNotFound -> MessageDisplay.makeReadModel h fs (Seq.singleton fm)
