namespace Fooble.Presentation

open System
open System.ComponentModel
open System.ComponentModel.DataAnnotations


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

    /// The date the member data was registered.
    [<DataType(DataType.Date)>]
    abstract Registered:DateTime with get

    /// The date the password was last changed.
    [<DataType(DataType.Date)>]
    [<DisplayName("Password Changed")>]
    abstract PasswordChanged:DateTime with get


/// Contains a single member's information, for the purpose of presentation as part of a list of members.
type IMemberListItemReadModel =

    /// The id that represents the member.
    abstract Id:Guid with get

    /// The nickname of the member.
    abstract Nickname:string with get


/// Contains a list of members' information, for the purpose of presentation.
type IMemberListReadModel =
    
    /// The list of members' information.
    abstract Members:seq<IMemberListItemReadModel>


/// Contains a member's password change information to be submitted.
type IMemberChangePasswordViewModel =

    /// The current password of the member.
    [<DataType(DataType.Password)>]
    [<DisplayName("Current Password")>]
    abstract CurrentPassword:string with get

    /// The new password of the member.
    [<DataType(DataType.Password)>]
    [<DisplayName("New Password")>]
    abstract NewPassword:string with get

    /// The confirm password of the member.
    [<DataType(DataType.Password)>]
    [<DisplayName("Confirm Password")>]
    abstract ConfirmPassword:string with get


/// Contains a potential member's detailed information to be submitted.
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


/// Represents the function to invoke to construct a new IMemberDetailReadModel instance.
type MemberDetailReadModelFactory =
    delegate of id:Guid * username:string * email:string * nickname:string * registered:DateTime *
        passwordChanged:DateTime -> IMemberDetailReadModel


/// Represents the function to invoke to construct a new IMemberListItemReadModel instance.
type MemberListItemReadModelFactory =
    delegate of id:Guid * nickname:string -> IMemberListItemReadModel


/// Represents the function to invoke to construct a new IMemberListReadModel instance.
type MemberListReadModelFactory =
    delegate of members:seq<IMemberListItemReadModel> -> IMemberListReadModel
