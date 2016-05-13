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
    (* Active Patterns *)

    let internal (|IsNotFound|) (status : IMemberDetailQueryFailureStatus) = ()
    
    (* Query *)

    type private MemberDetailQueryImplementation = 
        { Id : string }
        interface IMemberDetailQuery with
            member this.Id = 
                match this with
                | { Id = i } -> i
    
    [<CompiledName("MakeQuery")>]
    let makeQuery id = 
        Validation.enforce (Member.validateId id)
        { MemberDetailQueryImplementation.Id = id } :> IMemberDetailQuery
    
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
    
    let internal makeReadModel id name = 
        Validation.enforce (Member.validateId id)
        Validation.enforce (Member.validateName name)
        { MemberDetailReadModelImplementation.Id = id
          Name = name } :> IMemberDetailReadModel
    
    (* Query Failure Status *)

    type private MemberDetailQueryFailureStatusImplementation = 
        | NotFound
        interface IMemberDetailQueryFailureStatus with
            member this.IsNotFound = 
                match this with
                | NotFound -> true
    
    [<CompiledName("NotFoundQueryFailureStatus")>]
    let notFoundQueryFailureStatus = NotFound :> IMemberDetailQueryFailureStatus
    
    (* Query Handler *)

    type private MemberDetailQueryHandlerImplementation(context : IDataContext) = 
        interface IMemberDetailQueryHandler with
            member this.Handle(query : IMemberDetailQuery) = 
                Validation.enforce (Validation.validateIsNotNullValue query "query" "Query")
                box (context.Members.Find(query.Id))
                |> Option.ofObj
                |> Option.map unbox<MemberData>
                |> Option.map (fun md -> makeReadModel md.Id md.Name)
                |> Result.ofOption notFoundQueryFailureStatus
    
    let internal makeQueryHandler (context : IDataContext) = 
        Validation.enforce (Validation.validateIsNotNullValue context "context" "Data context")
        MemberDetailQueryHandlerImplementation(context) :> IMemberDetailQueryHandler
    
    (* Extensions *)

    [<CompiledName("ToMessageDisplayReadModel"); Extension>]
    let toMessageDisplayReadModel (result : IResult<IMemberDetailReadModel, IMemberDetailQueryFailureStatus>) = 
        let h = "Member Detail Query"
        let ss = MessageDisplay.informationalSeverity
        let sm = "Member detail query was successful"
        let fs = MessageDisplay.errorSeverity
        let fm = "Member detail query was not successful and returned not found"
        match result with
        | Result.IsSuccess _ -> MessageDisplay.makeReadModel h ss (Seq.singleton sm)
        | Result.IsFailure s -> 
            match s with
            | IsNotFound -> MessageDisplay.makeReadModel h fs (Seq.singleton fm)
