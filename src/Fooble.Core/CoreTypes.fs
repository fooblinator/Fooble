namespace Fooble.Core

open MediatR
open System

(* Key Generator *)

/// <summary>
/// Used to generate unique keys.
/// </summary>
type IKeyGenerator =

    /// <summary>
    /// Generates a unique key.
    /// </summary>
    /// <returns>Returns a newly generated unique key.</returns>
    abstract GenerateKey:unit -> Guid

(* Member Detail *)

/// <summary>
/// Contains a single member's detailed information, for the purpose of presentation.
/// </summary>
type IMemberDetailReadModel =

    /// <summary>
    /// The id that represents the member.
    /// </summary>
    abstract Id:Guid with get

    /// <summary>
    /// The name of the member.
    /// </summary>
    abstract Name:string with get

/// <summary>
/// Represents the status of a member detail query, and potential results, if successful.
/// </summary>
/// <remarks>The result is only one of "success" or "not found".</remarks>
type IMemberDetailQueryResult =

    /// <summary>
    /// The single member's detail information to be presented.
    /// </summary>
    abstract ReadModel:IMemberDetailReadModel with get

    /// <summary>
    /// Whether the result is "success" (or not).
    /// </summary>
    abstract IsSuccess:bool with get

    /// <summary>
    /// Whether the result is "not found" (or not).
    /// </summary>
    abstract IsNotFound:bool with get

/// <summary>
/// Contains a request for a single member's detailed information, for the purpose of presentation.
/// </summary>
type IMemberDetailQuery =
    inherit IRequest<IMemberDetailQueryResult>

    /// <summary>
    /// The member id to search for.
    /// </summary>
    abstract Id:Guid with get

(* Member List *)

/// <summary>
/// Contains a single member's information, for the purpose of presentation as part of a list of members.
/// </summary>
type IMemberListItemReadModel =

    /// <summary>
    /// The id that represents the member.
    /// </summary>
    abstract Id:Guid with get

    /// <summary>
    /// The name of the member.
    /// </summary>
    abstract Name:string with get

/// <summary>
/// Contains a list of members' information, for the purpose of presentation.
/// </summary>
type IMemberListReadModel =
    abstract Members:seq<IMemberListItemReadModel>

/// <summary>
/// Represents the status of a member list query, and potential results, if successful.
/// </summary>
/// <remarks>The result is only one of "success" or "not found".</remarks>
type IMemberListQueryResult =

    /// <summary>
    /// The list of members' information to be presented.
    /// </summary>
    abstract ReadModel:IMemberListReadModel with get

    /// <summary>
    /// Whether the result is "success" (or not).
    /// </summary>
    abstract IsSuccess:bool with get

    /// <summary>
    /// Whether the result is "not found" (or not).
    /// </summary>
    abstract IsNotFound:bool with get

/// <summary>
/// Contains a request for a list of members' information, for the purpose of presentation.
/// </summary>
type IMemberListQuery =
    inherit IRequest<IMemberListQueryResult>

(* Message Display *)

/// <summary>
/// Represents the severity of a message display.
/// </summary>
/// <remarks>The severity is only one of "informational", "warning", or "error".</remarks>
type IMessageDisplaySeverity =

    /// <summary>
    /// Whether the severity is "informational" (or not).
    /// </summary>
    abstract IsInformational:bool with get

    /// <summary>
    /// Whether the severity is "warning" (or not).
    /// </summary>
    abstract IsWarning:bool with get

    /// <summary>
    /// Whether the severity is "error" (or not).
    /// </summary>
    abstract IsError:bool with get

/// <summary>
/// Contains messages, for the purpose of presentation.
/// </summary>
type IMessageDisplayReadModel =

    /// <summary>
    /// The heading of the message display.
    /// </summary>
    abstract Heading:string with get

    /// <summary>
    /// The sub-heading of the message display.
    /// </summary>
    abstract SubHeading:string with get

    /// <summary>
    /// The status code of the message display.
    /// </summary>
    abstract StatusCode:int with get

    /// <summary>
    /// The severity of the message display.
    /// </summary>
    abstract Severity:IMessageDisplaySeverity with get

    /// <summary>
    /// The message to be displayed.
    /// </summary>
    abstract Message:string with get

(* Self Service Register *)

/// <summary>
/// Represents the self-service registration command, and contains the potential member's detailed information.
/// </summary>
type ISelfServiceRegisterCommand =
    inherit IRequest<Unit>

    /// <summary>
    /// The id that will potentially represent the member.
    /// </summary>
    abstract Id:Guid with get

    /// <summary>
    /// The name of the member.
    /// </summary>
    abstract Name:string with get

/// <summary>
/// Contains a potential member's detailed information to be submitted for self-service registration.
/// </summary>
type ISelfServiceRegisterViewModel =

    /// <summary>
    /// The name of the member.
    /// </summary>
    abstract Name:string with get

(* Validation *)

/// <summary>
/// Represents the status of parameter validation, and potential results, if invalid.
/// </summary>
/// <remarks>The result is only one of "valid" or "invalid".</remarks>
type IValidationResult =

    /// <summary>
    /// The name of the invalid parameter.
    /// </summary>
    abstract ParamName:string with get

    /// <summary>
    /// The message describing why the parameter is invalid.
    /// </summary>
    abstract Message:string with get

    /// <summary>
    /// Whether the result is "valid" (or not).
    /// </summary>
    abstract IsValid:bool with get

    /// <summary>
    /// Whether the result is "invalid" (or not).
    /// </summary>
    abstract IsInvalid:bool with get
