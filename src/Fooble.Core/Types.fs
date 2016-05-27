namespace Fooble.Core

open MediatR
open System

(* Key Generator *)

/// Used to generate unique keys.
type IKeyGenerator =
    /// <summary>
    /// Generates a unique key.
    /// </summary>
    /// <returns>Returns a newly generated unique key.</returns>
    abstract GenerateKey:unit -> Guid



(* Member Detail *)

/// Contains a single member's detailed information, for the purpose of presentation.
type IMemberDetailReadModel =
    /// The id that represents the member.
    abstract Id:Guid with get
    /// The username of the member.
    abstract Username:string with get
    /// The nickname of the member.
    abstract Nickname:string with get

/// <summary>
/// Represents the status of a member detail query, and potential results, if successful.
/// </summary>
/// <remarks>The result is only one of "success" or "not found".</remarks>
type IMemberDetailQueryResult =
    /// The single member's detail information to be presented.
    abstract ReadModel:IMemberDetailReadModel with get
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get
    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

/// Contains a request for a single member's detailed information, for the purpose of presentation.
type IMemberDetailQuery =
    inherit IRequest<IMemberDetailQueryResult>
    /// The member id to search for.
    abstract Id:Guid with get



(* Member List *)

/// Contains a single member's information, for the purpose of presentation as part of a list of members.
type IMemberListItemReadModel =
    /// The id that represents the member.
    abstract Id:Guid with get
    /// The nickname of the member.
    abstract Nickname:string with get

/// Contains a list of members' information, for the purpose of presentation.
type IMemberListReadModel =
    abstract Members:seq<IMemberListItemReadModel>

/// <summary>
/// Represents the status of a member list query, and potential results, if successful.
/// </summary>
/// <remarks>The result is only one of "success" or "not found".</remarks>
type IMemberListQueryResult =
    /// The list of members' information to be presented.
    abstract ReadModel:IMemberListReadModel with get
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get
    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

/// Contains a request for a list of members' information, for the purpose of presentation.
type IMemberListQuery =
    inherit IRequest<IMemberListQueryResult>



(* Message Display *)

/// <summary>
/// Represents the severity of a message display.
/// </summary>
/// <remarks>The severity is only one of "informational", "warning", or "error".</remarks>
type IMessageDisplaySeverity =
    /// Whether the severity is "informational" (or not).
    abstract IsInformational:bool with get
    /// Whether the severity is "warning" (or not).
    abstract IsWarning:bool with get
    /// Whether the severity is "error" (or not).
    abstract IsError:bool with get

/// Contains messages, for the purpose of presentation.
type IMessageDisplayReadModel =
    /// The heading of the message display.
    abstract Heading:string with get
    /// The sub-heading of the message display.
    abstract SubHeading:string with get
    /// The status code of the message display.
    abstract StatusCode:int with get
    /// The severity of the message display.
    abstract Severity:IMessageDisplaySeverity with get
    /// The message to be displayed.
    abstract Message:string with get



(* Self Service Register *)

/// <summary>
/// Represents the status of a self-service registration command.
/// </summary>
/// <remarks>The result is only one of "success" or "username unavailable".</remarks>
type ISelfServiceRegisterCommandResult =
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get
    /// Whether the result is "username unavailable" (or not).
    abstract IsUsernameUnavailable:bool with get

/// Represents the self-service registration command, and contains the potential member's detailed information.
type ISelfServiceRegisterCommand =
    inherit IRequest<ISelfServiceRegisterCommandResult>
    /// The id that will potentially represent the member.
    abstract Id:Guid with get
    /// The username of the member.
    abstract Username:string with get
    /// The nickname of the member.
    abstract Nickname:string with get

/// Contains a potential member's detailed information to be submitted for self-service registration.
type ISelfServiceRegisterViewModel =
    /// The nickname of the member.
    abstract Nickname:string with get
    /// The username of the member.
    abstract Username:string with get



(* Validation *)

/// <summary>
/// Represents the status of parameter validation, and potential results, if invalid.
/// </summary>
/// <remarks>The result is only one of "valid" or "invalid".</remarks>
type IValidationResult =
    /// The name of the invalid parameter.
    abstract ParamName:string with get
    /// The message describing why the parameter is invalid.
    abstract Message:string with get
    /// Whether the result is "valid" (or not).
    abstract IsValid:bool with get
    /// Whether the result is "invalid" (or not).
    abstract IsInvalid:bool with get
