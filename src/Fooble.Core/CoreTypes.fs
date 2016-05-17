namespace Fooble.Core

open MediatR
open System.Data.Entity

[<AllowNullLiteral>]
type IResult<'TSuccess,'TFailure> =
    abstract Value:'TSuccess
    abstract Status:'TFailure
    abstract IsSuccess:bool
    abstract IsFailure:bool

[<AllowNullLiteral>]
type IMemberData =
    abstract Id:string
    abstract Name:string

[<AllowNullLiteral>]
type internal IDataContext =
    abstract Members:IDbSet<IMemberData>

[<AllowNullLiteral>]
type IMemberDetailReadModel =
    abstract Id:string
    abstract Name:string

[<AllowNullLiteral>]
type IMemberDetailQueryFailureStatus =
    abstract IsNotFound:bool

[<AllowNullLiteral>]
type IMemberDetailQuery =
    inherit IRequest<IResult<IMemberDetailReadModel,IMemberDetailQueryFailureStatus>>
    abstract Id:string

type IMemberDetailQueryHandler = IRequestHandler<IMemberDetailQuery,IResult<IMemberDetailReadModel,IMemberDetailQueryFailureStatus>>

[<AllowNullLiteral>]
type IMemberListItemReadModel =
    abstract Id:string
    abstract Name:string

[<AllowNullLiteral>]
type IMemberListReadModel =
    abstract Members:seq<IMemberListItemReadModel>

[<AllowNullLiteral>]
type IMemberListQueryFailureStatus =
    abstract IsNotFound:bool

[<AllowNullLiteral>]
type IMemberListQuery =
    inherit IRequest<IResult<IMemberListReadModel,IMemberListQueryFailureStatus>>

type IMemberListQueryHandler = IRequestHandler<IMemberListQuery,IResult<IMemberListReadModel,IMemberListQueryFailureStatus>>

[<AllowNullLiteral>]
type IMessageDisplaySeverity =
    abstract IsInformational:bool
    abstract IsWarning:bool
    abstract IsError:bool

[<AllowNullLiteral>]
type IMessageDisplayReadModel =
    abstract Heading:string
    abstract Severity:IMessageDisplaySeverity
    abstract Messages:seq<string>

[<AllowNullLiteral>]
type IValidationResult =
    abstract ParamName:string
    abstract Message:string
    abstract IsValid:bool
    abstract IsInvalid:bool
