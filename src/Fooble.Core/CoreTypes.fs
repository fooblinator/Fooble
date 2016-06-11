namespace Fooble.Core

open Fooble.Presentation
open MediatR
open System

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
/// Represents the status of a member exists query.
/// </summary>
/// <remarks>The result is only one of "success" or "not found".</remarks>
type IMemberExistsQueryResult =

    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get


/// Contains a request to verify the existence of a specified member id.
type IMemberExistsQuery =
    inherit IRequest<IMemberExistsQueryResult>

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
/// Represents the status of a member change email command.
/// </summary>
/// <remarks>The result is only one of "success", "not found", "incorrect password" or "unavailable email".</remarks>
type IMemberChangeEmailCommandResult =
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

    /// Whether the result is "incorrect password" (or not).
    abstract IsIncorrectPassword:bool with get

    /// Whether the result is "unavailable email" (or not).
    abstract IsUnavailableEmail:bool with get


/// Represents the member change email command.
type IMemberChangeEmailCommand =
    inherit IRequest<IMemberChangeEmailCommandResult>

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The current password of the member.
    abstract CurrentPassword:string with get

    /// The new email of the member.
    abstract NewEmail:string with get


/// <summary>
/// Represents the status of a member change other command.
/// </summary>
/// <remarks>The result is only one of "success" or "not found".</remarks>
type IMemberChangeOtherCommandResult =
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get


/// Represents the member change other command.
type IMemberChangeOtherCommand =
    inherit IRequest<IMemberChangeOtherCommandResult>

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The new nickname of the member.
    abstract NewNickname:string with get


/// <summary>
/// Represents the status of a member change password command.
/// </summary>
/// <remarks>The result is only one of "success", "not found" or "incorrect password".</remarks>
type IMemberChangePasswordCommandResult =
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

    /// Whether the result is "incorrect password" (or not).
    abstract IsIncorrectPassword:bool with get


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
/// Represents the status of a member change username command.
/// </summary>
/// <remarks>The result is only one of "success", "not found", "incorrect password" or "unavailable
/// username".</remarks>
type IMemberChangeUsernameCommandResult =
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

    /// Whether the result is "incorrect password" (or not).
    abstract IsIncorrectPassword:bool with get

    /// Whether the result is "unavailable username" (or not).
    abstract IsUnavailableUsername:bool with get


/// Represents the member change username command.
type IMemberChangeUsernameCommand =
    inherit IRequest<IMemberChangeUsernameCommandResult>

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The current password of the member.
    abstract CurrentPassword:string with get

    /// The new username of the member.
    abstract NewUsername:string with get


/// <summary>
/// Represents the status of a member register command.
/// </summary>
/// <remarks>The result is only one of "success", "unavailable username" or "unavailable email".</remarks>
type IMemberRegisterCommandResult =
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "unavailable username" (or not).
    abstract IsUnavailableUsername:bool with get

    /// Whether the result is "unavailable email" (or not).
    abstract IsUnavailableEmail:bool with get


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
