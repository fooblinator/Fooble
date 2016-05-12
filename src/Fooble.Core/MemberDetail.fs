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

[<RequireQualifiedAccess; Extension>]
module MemberDetail = 
    (* Query *)

    type private MemberDetailQueryImplementation = 
        { Id : string }
        interface IMemberDetailQuery with
            member this.Id = 
                match this with
                | { Id = i } -> i
    
    [<CompiledName("MakeQuery")>]
    let makeQuery id : IMemberDetailQuery = 
        Validation.ensureIsValid Member.validateId id
        { MemberDetailQueryImplementation.Id = id } :> _
    
    (* Read Model *)

    type private MemberDetailReadModelImplementation = 
        { Id : string
          Name : string }
        interface IMemberDetailReadModel with
            
            member this.Id = 
                match this with
                | { Id = i; Name = _ } -> i
            
            member this.Name = 
                match this with
                | { Id = _; Name = n } -> n
    
    let internal makeReadModel id name : IMemberDetailReadModel = 
        Validation.ensureIsValid Member.validateId id
        Validation.ensureIsValid Member.validateName name
        { MemberDetailReadModelImplementation.Id = id
          Name = name } :> _
    
    (* Query Failure Status *)

    type private MemberDetailQueryFailureStatusImplementation = 
        | NotFound
        interface IMemberDetailQueryFailureStatus with
            member this.IsNotFound = 
                match this with
                | NotFound -> true
    
    [<CompiledName("NotFoundQueryFailureStatus")>]
    let notFoundQueryFailureStatus : IMemberDetailQueryFailureStatus = NotFound :> _
    
    let (|NotFound|) (status : IMemberDetailQueryFailureStatus) = ()
    
    (* Query Handler *)

    type private MemberDetailQueryHandlerImplementation(context : IDataContext) = 
        interface IMemberDetailQueryHandler with
            member this.Handle(query) = 
                Validation.ensureNotNull query "query" "Query"
                box (context.Members.Find(query.Id))
                |> Option.ofObj
                |> Option.map unbox<MemberData>
                |> Option.map (fun md -> makeReadModel md.Id md.Name)
                |> Result.ofOption notFoundQueryFailureStatus
    
    let internal makeQueryHandler context : IMemberDetailQueryHandler = 
        Validation.ensureNotNull context "context" "Data context"
        MemberDetailQueryHandlerImplementation(context) :> _
    
    (* Extensions *)

    [<CompiledName("ToMessageDisplayReadModel"); Extension>]
    let toMessageDisplayReadModel (result : IResult<IMemberDetailReadModel, IMemberDetailQueryFailureStatus>) : IMessageDisplayReadModel = 
        let h = "Member Detail Query"
        let ss = MessageDisplay.informationalSeverity
        let sm = "Member detail query was successful"
        let fs = MessageDisplay.errorSeverity
        let fm = "Member detail query was not successful and returned not found"
        match result with
        | Result.Success _ -> MessageDisplay.makeReadModel h ss (Seq.singleton sm)
        | Result.Failure s -> 
            match s with
            | NotFound -> MessageDisplay.makeReadModel h fs (Seq.singleton fm)
