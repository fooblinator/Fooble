﻿namespace Fooble.Core

open Fooble.Presentation
open MediatR
open System

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


/// <summary>
/// Represents the status of a member change password command.
/// </summary>
/// <remarks>The result is only one of "success", "not found" or "invalid".</remarks>
type IMemberChangePasswordCommandResult =
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

    /// Whether the result is "invalid" (or not).
    abstract IsInvalid:bool with get


/// Represents the member change password command.
type IMemberChangePasswordCommand =
    inherit IRequest<IMemberChangePasswordCommandResult>

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The current password of the member.
    abstract CurrentPassword:string with get

    /// The new password of the member.
    abstract NewPassword:string with get


/// <summary>
/// Represents the status of a member register command.
/// </summary>
/// <remarks>The result is only one of "success", "username unavailable" or "email unavailable".</remarks>
type IMemberRegisterCommandResult =
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "username unavailable" (or not).
    abstract IsUsernameUnavailable:bool with get

    /// Whether the result is "email unavailable" (or not).
    abstract IsEmailUnavailable:bool with get


/// Represents the member register command.
type IMemberRegisterCommand =
    inherit IRequest<IMemberRegisterCommandResult>

    /// The id that will potentially represent the member.
    abstract Id:Guid with get

    /// The username of the member.
    abstract Username:string with get

    /// The password of the member.
    abstract Password:string with get

    /// The email of the member.
    abstract Email:string with get

    /// The nickname of the member.
    abstract Nickname:string with get


/// Represents the function to invoke to generate a unique key.
type KeyGenerator =
    delegate of unit -> Guid
