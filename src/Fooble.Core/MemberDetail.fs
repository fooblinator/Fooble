namespace Fooble.Core

open MediatR
open System.Runtime.CompilerServices

type IMemberDetailReadModel = 
    abstract Id : string
    abstract Name : string

type IMemberDetailQueryFailureStatus = 
    abstract IsNotFound : bool

type IMemberDetailQuery = 
    inherit IRequest<IResult<IMemberDetailReadModel, IMemberDetailQueryFailureStatus>>
    abstract Id : string

type IMemberDetailQueryHandler = IRequestHandler<IMemberDetailQuery, IResult<IMemberDetailReadModel, IMemberDetailQueryFailureStatus>>

[<RequireQualifiedAccess>]
module internal MemberDetailReadModel = 
    type private Implementation = 
        { Id : string
          Name : string }
        interface IMemberDetailReadModel with
            
            member this.Id = 
                match this with
                | { Id = i; Name = _ } -> i
            
            member this.Name = 
                match this with
                | { Id = _; Name = n } -> n
    
    let make id name : IMemberDetailReadModel = 
        Validation.ensureIsValid Member.validateId id
        Validation.ensureIsValid Member.validateName name
        { Id = id
          Name = name } :> _

[<RequireQualifiedAccess>]
module MemberDetailQueryFailureStatus = 
    let (|NotFound|) (status : IMemberDetailQueryFailureStatus) = ()
    
    type private Implementation = 
        | NotFound
        interface IMemberDetailQueryFailureStatus with
            member this.IsNotFound = 
                match this with
                | NotFound -> true
    
    [<CompiledName("NotFound")>]
    let notFound : IMemberDetailQueryFailureStatus = NotFound :> _

[<RequireQualifiedAccess>]
module MemberDetailQuery = 
    type private Implementation = 
        { Id : string }
        interface IMemberDetailQuery with
            member this.Id = 
                match this with
                | { Id = i } -> i
    
    [<CompiledName("Make")>]
    let make id : IMemberDetailQuery = 
        Validation.ensureIsValid Member.validateId id
        { Id = id } :> _

[<RequireQualifiedAccess>]
module internal MemberDetailQueryHandler = 
    let validateQuery query = 
        if Validation.isNullValue query then 
            Some(Validation.makeFailureInfo "query" (sprintf "%s should not be null" "Query"))
        else None
    
    type private Implementation(context : IDataContext) = 
        interface IMemberDetailQueryHandler with
            member this.Handle(query) = 
                Validation.ensureIsValid validateQuery query
                box (context.Members.Find(query.Id))
                |> Option.ofObj
                |> Option.map unbox<MemberData>
                |> Option.map (fun md -> MemberDetailReadModel.make md.Id md.Name)
                |> Result.ofOption MemberDetailQueryFailureStatus.notFound
    
    let validateContext context = 
        if Validation.isNullValue context then 
            Some(Validation.makeFailureInfo "context" (sprintf "%s should not be null" "Data context"))
        else None
    
    let make context : IMemberDetailQueryHandler = 
        Validation.ensureIsValid validateContext context
        Implementation(context) :> _

[<Extension>]
module MemberDetailExtensions = 
    [<CompiledName("ToMessageDisplayReadModel"); Extension>]
    let toMessageDisplayReadModel (result : IResult<IMemberDetailReadModel, IMemberDetailQueryFailureStatus>) = 
        let h = "Member Detail Query"
        let ss = MessageDisplay.informationalSeverity
        let sm = "Member detail query was successful"
        let fs = MessageDisplay.errorSeverity
        let fm = "Member detail query was not successful and returned not found"
        match result with
        | Result.Success _ -> MessageDisplay.makeReadModel h ss (Seq.singleton sm)
        | Result.Failure s -> 
            match s with
            | MemberDetailQueryFailureStatus.NotFound -> MessageDisplay.makeReadModel h fs (Seq.singleton fm)
