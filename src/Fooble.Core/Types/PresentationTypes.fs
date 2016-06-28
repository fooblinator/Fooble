namespace Fooble.Presentation

open System
open System.ComponentModel
open System.ComponentModel.DataAnnotations

(* Member Detail Read Model *)

/// Contains a single member's detailed information, for the purpose of presentation.
type IMemberDetailReadModel =

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The username of the member.
    abstract Username:string with get

    /// The email of the member.
    [<DataType(DataType.EmailAddress)>]
    abstract Email:string with get

    /// The nickname of the member.
    abstract Nickname:string with get

    /// The avatar data for the member.
    abstract AvatarData:string with get

    /// The date the member data was registered.
    [<DataType(DataType.Date)>]
    abstract Registered:DateTime with get

    /// The date the password was last changed.
    [<DataType(DataType.Date)>]
    [<DisplayName("Password Changed")>]
    abstract PasswordChanged:DateTime with get



(* Member List Read Model *)

/// Contains a single member's information, for the purpose of presentation as part of a list of members.
type IMemberListItemReadModel =

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The nickname of the member.
    abstract Nickname:string with get

    /// The avatar data for the member.
    abstract AvatarData:string with get

/// Contains a list of members' information, for the purpose of presentation.
type IMemberListReadModel =

    /// The list of members' information.
    abstract Members:seq<IMemberListItemReadModel> with get

    /// The total number of members.
    abstract MemberCount:int with get



(* Message Display Read Model *)

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
    [<DisplayName("Sub-Heading")>]
    abstract SubHeading:string with get

    /// The status code of the message display.
    [<DisplayName("Status Code")>]
    abstract StatusCode:int with get

    /// The severity of the message display.
    abstract Severity:IMessageDisplaySeverity with get

    /// The message to be displayed.
    abstract Message:string with get



(* Member Change Email View Model *)

/// Contains a member's information to be submitted for a change email request.
type IMemberChangeEmailViewModel =

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The current password of the member.
    [<DataType(DataType.Password)>]
    [<DisplayName("Current Password")>]
    abstract CurrentPassword:string with get

    /// The email of the member.
    [<DataType(DataType.EmailAddress)>]
    [<DisplayName("Email")>]
    abstract Email:string with get



(* Member Change Other View Model *)

/// Contains a member's information to be submitted for a change other request.
type IMemberChangeOtherViewModel =

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The nickname of the member.
    [<DisplayName("Nickname")>]
    abstract Nickname:string with get

    /// The avatar data for the member.
    [<DisplayName("Avatar Data")>]
    abstract AvatarData:string with get



(* Member Change Password View Model *)

/// Contains a member's information to be submitted for a change password request.
type IMemberChangePasswordViewModel =

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The current password of the member.
    [<DataType(DataType.Password)>]
    [<DisplayName("Current Password")>]
    abstract CurrentPassword:string with get

    /// The password of the member.
    [<DataType(DataType.Password)>]
    [<DisplayName("Password")>]
    abstract Password:string with get

    /// The confirm password of the member.
    [<DataType(DataType.Password)>]
    [<DisplayName("Confirm Password")>]
    abstract ConfirmPassword:string with get



(* Member Change Username View Model *)

/// Contains a member's information to be submitted for a change username request.
type IMemberChangeUsernameViewModel =

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The current password of the member.
    [<DataType(DataType.Password)>]
    [<DisplayName("Current Password")>]
    abstract CurrentPassword:string with get

    /// The username of the member.
    [<DisplayName("Username")>]
    abstract Username:string with get



(* Member Deactivate View Model *)

/// Contains a member's information to be submitted for a deactivation request.
type IMemberDeactivateViewModel =

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The current password of the member.
    [<DataType(DataType.Password)>]
    [<DisplayName("Current Password")>]
    abstract CurrentPassword:string with get



(* Member Register View Model *)

/// Contains a potential member's information to be submitted for a registration request.
type IMemberRegisterViewModel =

    /// The username of the member.
    abstract Username:string with get

    /// The password of the member.
    [<DataType(DataType.Password)>]
    abstract Password:string with get

    /// The confirm password of the member.
    [<DataType(DataType.Password)>]
    [<DisplayName("Confirm Password")>]
    abstract ConfirmPassword:string with get

    /// The email of the member.
    [<DataType(DataType.EmailAddress)>]
    abstract Email:string with get

    /// The nickname of the member.
    abstract Nickname:string with get

    /// The avatar data for the member.
    [<DisplayName("Avatar Data")>]
    abstract AvatarData:string with get



(* Delegates *)

/// <summary>
/// Function to construct an initial member change password view model.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
type InitialMemberChangePasswordViewModelFactory =
    delegate of id:Guid -> IMemberChangePasswordViewModel

/// <summary>
/// Function to construct an initial member deactivate view model.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
type InitialMemberDeactivateViewModelFactory =
    delegate of id:Guid -> IMemberDeactivateViewModel

/// <summary>
/// Function to construct an initial member register view model.
/// </summary>
type InitialMemberRegisterViewModelFactory =
    delegate of unit -> IMemberRegisterViewModel

/// <summary>
/// Function to construct a member change email view model.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
/// <param name="currentPassword">The current password of the member.</param>
/// <param name="email">The email of the member.</param>
type MemberChangeEmailViewModelFactory =
    delegate of id:Guid * currentPassword:string * email:string -> IMemberChangeEmailViewModel

/// <summary>
/// Function to construct a member change other view model.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
/// <param name="nickname">The nickname of the member.</param>
/// <param name="avatarData">The avatar data of the member.</param>
type MemberChangeOtherViewModelFactory =
    delegate of id:Guid * nickname:string * avatarData:string -> IMemberChangeOtherViewModel

/// <summary>
/// Function to construct a member change password view model.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
/// <param name="currentPassword">The current password of the member.</param>
/// <param name="password">The new password of the member.</param>
/// <param name="confirmPassword">The confirm password of the member.</param>
type MemberChangePasswordViewModelFactory =
    delegate of id:Guid * currentPassword:string * password:string * confirmPassword:string ->
        IMemberChangePasswordViewModel

/// <summary>
/// Function to construct a member change username view model.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
/// <param name="currentPassword">The current password of the member.</param>
/// <param name="username">The username of the member.</param>
type MemberChangeUsernameViewModelFactory =
    delegate of id:Guid * currentPassword:string * username:string -> IMemberChangeUsernameViewModel

/// <summary>
/// Function to construct a member deactivate view model.
/// </summary>
/// <param name="id">The id of the member to retrieve.</param>
/// <param name="currentPassword">The current password of the member.</param>
type MemberDeactivateViewModelFactory =
    delegate of id:Guid * currentPassword:string -> IMemberDeactivateViewModel

/// <summary>
/// Function to construct a member register view model.
/// </summary>
/// <param name="username">The username of the potential member.</param>
/// <param name="password">The password of the potential member.</param>
/// <param name="confirmPassword">The confirm password of the potential member.</param>
/// <param name="email">The email of the potential member.</param>
/// <param name="nickname">The nickname of the potential member.</param>
/// <param name="avatarData">The avatarData of the potential member.</param>
type MemberRegisterViewModelFactory =
    delegate of username:string * password:string * currentPassword:string * email:string * nickname:string *
        avatarData:string -> IMemberRegisterViewModel
