namespace Fooble.Core

open Fooble.Presentation
open MediatR
open System

(* Member Detail Query *)

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



(* Member Email Query *)

/// <summary>
/// Represents the status of a member email query, and potential results, if successful.
/// </summary>
/// <remarks>The result is only one of "success" or "not found".</remarks>
type IMemberEmailQueryResult =

    /// The single member's email to be presented.
    abstract ViewModel:IMemberChangeEmailViewModel with get

    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

/// Contains a request for a single member's email, for the purpose of presentation.
type IMemberEmailQuery =
    inherit IRequest<IMemberEmailQueryResult>

    /// The member id to search for.
    abstract Id:Guid with get



(* Member Exists Query *)

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



(* Member List Query *)

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



(* Member Other Query *)

/// <summary>
/// Represents the status of a member other query, and potential results, if successful.
/// </summary>
/// <remarks>The result is only one of "success" or "not found".</remarks>
type IMemberOtherQueryResult =

    /// The single member's nickname and avatar data to be presented.
    abstract ViewModel:IMemberChangeOtherViewModel with get

    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

/// Contains a request for a single member's nickname and avatar data, for the purpose of presentation.
type IMemberOtherQuery =
    inherit IRequest<IMemberOtherQueryResult>

    /// The member id to search for.
    abstract Id:Guid with get



(* Member Username Query *)

/// <summary>
/// Represents the status of a member username query, and potential results, if successful.
/// </summary>
/// <remarks>The result is only one of "success" or "not found".</remarks>
type IMemberUsernameQueryResult =

    /// The single member's username to be presented.
    abstract ViewModel:IMemberChangeUsernameViewModel with get

    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

/// Contains a request for a single member's username, for the purpose of presentation.
type IMemberUsernameQuery =
    inherit IRequest<IMemberUsernameQueryResult>

    /// The member id to search for.
    abstract Id:Guid with get



(* Member Change Email Command *)

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

    /// The email of the member.
    abstract Email:string with get



(* Member Change Other Command *)

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

    /// The nickname of the member.
    abstract Nickname:string with get

    /// The avatar data of the member.
    abstract AvatarData:string with get



(* Member Change Password Command *)

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

    /// The password of the member.
    abstract Password:string with get



(* Member Change Username Command *)

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

    /// The username of the member.
    abstract Username:string with get



(* Member Deactivate Command *)

/// <summary>
/// Represents the status of a member deactivate command.
/// </summary>
/// <remarks>The result is only one of "success", "not found" or "incorrect password".</remarks>
type IMemberDeactivateCommandResult =
    /// Whether the result is "success" (or not).
    abstract IsSuccess:bool with get

    /// Whether the result is "not found" (or not).
    abstract IsNotFound:bool with get

    /// Whether the result is "incorrect password" (or not).
    abstract IsIncorrectPassword:bool with get

/// Represents the member deactivate command.
type IMemberDeactivateCommand =
    inherit IRequest<IMemberDeactivateCommandResult>

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The current password of the member.
    abstract CurrentPassword:string with get



(* Member Register Command *)

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

    /// The avatar data of the member.
    abstract AvatarData:string with get



(* Delegates *)

/// Function to generate unique keys for the domain.
type IdGenerator =
    delegate of unit -> Guid

/// <summary>
/// Function to construct a member detail query.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
type MemberDetailQueryFactory =
    delegate of id:Guid -> IMemberDetailQuery

/// <summary>
/// Function to construct a member email query.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
type MemberEmailQueryFactory =
    delegate of id:Guid -> IMemberEmailQuery

/// <summary>
/// Function to construct a member exists query.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
type MemberExistsQueryFactory =
    delegate of id:Guid -> IMemberExistsQuery

/// <summary>
/// Function to construct a member list query.
/// </summary>
type MemberListQueryFactory =
    delegate of unit -> IMemberListQuery

/// <summary>
/// Function to construct a member other query.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
type MemberOtherQueryFactory =
    delegate of id:Guid -> IMemberOtherQuery

/// <summary>
/// Function to construct a member username query.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
type MemberUsernameQueryFactory =
    delegate of id:Guid -> IMemberUsernameQuery
