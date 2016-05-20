namespace Fooble.Core

open MediatR
open System

(* Member Detail *)

type IMemberDetailReadModel =
    abstract Id:Guid
    abstract Name:string

type IMemberDetailQueryResult =
    abstract ReadModel:IMemberDetailReadModel
    abstract IsSuccess:bool
    abstract IsNotFound:bool

type IMemberDetailQuery =
    inherit IRequest<IMemberDetailQueryResult>
    abstract Id:Guid

type IMemberDetailQueryHandler = IRequestHandler<IMemberDetailQuery, IMemberDetailQueryResult>

(* Member List *)

type IMemberListItemReadModel =
    abstract Id:Guid
    abstract Name:string

type IMemberListReadModel =
    abstract Members:seq<IMemberListItemReadModel>

type IMemberListQueryResult =
    abstract ReadModel:IMemberListReadModel
    abstract IsSuccess:bool
    abstract IsNotFound:bool

type IMemberListQuery =
    inherit IRequest<IMemberListQueryResult>

type IMemberListQueryHandler = IRequestHandler<IMemberListQuery, IMemberListQueryResult>

(* Message Display *)

type IMessageDisplaySeverity =
    abstract IsInformational:bool
    abstract IsWarning:bool
    abstract IsError:bool

type IMessageDisplayReadModel =
    abstract Heading:string
    abstract Severity:IMessageDisplaySeverity
    abstract Messages:seq<string>

(* Validation *)

type IValidationResult =
    abstract ParamName:string
    abstract Message:string
    abstract IsValid:bool
    abstract IsInvalid:bool
