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

[<RequireQualifiedAccess>]
module internal MemberListItemReadModel = 
    type private Implementation = 
        { Id : string
          Name : string }
        interface IMemberListItemReadModel with
            
            member this.Id = 
                match this with
                | { Id = i; Name = _ } -> i
            
            member this.Name = 
                match this with
                | { Id = _; Name = n } -> n
    
    let make id name : IMemberListItemReadModel = 
        Validation.ensureIsValid Member.validateId id
        Validation.ensureIsValid Member.validateName name
        { Id = id
          Name = name } :> _

[<RequireQualifiedAccess>]
module internal MemberListReadModel = 
    type private Implementation = 
        { Members : IMemberListItemReadModel seq }
        interface IMemberListReadModel with
            member this.Members = 
                match this with
                | { Members = ms } -> ms
    
    let validateMembers (members : IMemberListItemReadModel seq) = 
        if Validation.isNullValue members then 
            Some(Validation.makeFailureInfo "members" (sprintf "%s should not be null" "Member list"))
        else if Seq.isEmpty members then 
            Some(Validation.makeFailureInfo "members" (sprintf "%s should not be empty" "Member list"))
        else if Validation.containsNullValue members then 
            Some(Validation.makeFailureInfo "members" (sprintf "%s should not be null" "Member list items"))
        else None
    
    let make members : IMemberListReadModel = 
        Validation.ensureIsValid validateMembers members
        { Members = members } :> _

[<RequireQualifiedAccess>]
module MemberListQueryFailureStatus = 
    let (|NotFound|) (status : IMemberListQueryFailureStatus) = ()
    
    type private Implementation = 
        | NotFound
        interface IMemberListQueryFailureStatus with
            member this.IsNotFound = 
                match this with
                | NotFound -> true
    
    [<CompiledName("NotFound")>]
    let notFound : IMemberListQueryFailureStatus = NotFound :> _

[<RequireQualifiedAccess>]
module MemberListQuery = 
    type private Implementation() = 
        interface IMemberListQuery
    
    [<CompiledName("Make")>]
    let make() : IMemberListQuery = Implementation() :> _

[<RequireQualifiedAccess>]
module internal MemberListQueryHandler = 
    let validateQuery query = 
        if Validation.isNullValue query then 
            Some(Validation.makeFailureInfo "query" (sprintf "%s should not be null" "Query"))
        else None
    
    type private Implementation(context : IDataContext) = 
        interface IMemberListQueryHandler with
            member this.Handle(query) = 
                Validation.ensureIsValid validateQuery query
                match List.ofSeq context.Members with
                | [] -> Result.failure MemberListQueryFailureStatus.notFound
                | mds -> 
                    List.map (fun md -> MemberListItemReadModel.make md.Id md.Name) mds
                    |> Seq.ofList
                    |> MemberListReadModel.make
                    |> Result.success
    
    let validateContext context = 
        if Validation.isNullValue context then 
            Some(Validation.makeFailureInfo "context" (sprintf "%s should not be null" "Data context"))
        else None
    
    let make context : IMemberListQueryHandler = 
        Validation.ensureIsValid validateContext context
        Implementation(context) :> _

[<Extension>]
module MemberListExtensions = 
    [<CompiledName("ToMessageDisplayReadModel"); Extension>]
    let toMessageDisplayReadModel (result : IResult<IMemberListReadModel, IMemberListQueryFailureStatus>) = 
        let h = "Member List Query"
        let ss = MessageDisplaySeverity.informational
        let sm = "Member list query was successful"
        let fs = MessageDisplaySeverity.error
        let fm = "Member list query was not successful and returned not found"
        match result with
        | Result.Success _ -> MessageDisplayReadModel.make h ss (Seq.singleton sm)
        | Result.Failure s -> 
            match s with
            | MemberListQueryFailureStatus.NotFound -> MessageDisplayReadModel.make h fs (Seq.singleton fm)
