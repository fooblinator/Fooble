﻿namespace Fooble.Persistence

open System

(* Member Data *)

/// Represents the data record for members.
type IMemberData =

    /// The id that will potentially represent the member.
    abstract Id:Guid with get, set

    /// The username of the member.
    abstract Username:string with get, set

    /// The password data for the member.
    abstract PasswordData:string with get, set

    /// The email of the member.
    abstract Email:string with get, set

    /// The nickname of the member.
    abstract Nickname:string with get, set

    /// The avatar data for the member.
    abstract AvatarData:string with get, set

    /// The date the member data was registered.
    abstract RegisteredOn:DateTime with get, set

    /// The date the password was last changed.
    abstract PasswordChangedOn:DateTime with get, set

    /// The date the account was deactivated (if deactivated).
    abstract DeactivatedOn:DateTime option with get, set

/// Represents the function to invoke to construct a new IMemberData instance.
type MemberDataFactory =
    delegate of id:Guid * username:string * passwordData:string * email:string * nickname:string * avatarData:string *
        registeredOn:DateTime * passwordChanged:DateTime * deactivatedOn:DateTime option -> IMemberData



(* Fooble Context *)

/// Represents the data context for the Fooble application.
type IFoobleContext =
    inherit IDisposable

    /// <summary>
    /// Retrieves the member data matching the specified id from the data store.
    /// </summary>
    /// <param name="id">The id of the member to retrieve.</param>
    /// <param name="includeDeactivated">Whether to also include deactivated member accounts (or not).</param>
    /// <remarks>Returns none if no matching member id was found.</remarks>
    abstract GetMember : id:Guid * includeDeactivated:bool -> IMemberData option

    /// <summary>
    /// Retrieves all of the member data from the data store.
    /// </summary>
    /// <param name="includeDeactivated">Whether to also include deactivated member accounts (or not).</param>
    abstract GetMembers : includeDeactivated:bool -> IMemberData list

    /// <summary>
    /// Determines whether or not the specified member id exists in the data store.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <param name="includeDeactivated">Whether to also include deactivated member accounts (or not).</param>
    abstract ExistsMemberId : id:Guid * includeDeactivated:bool -> bool

    /// <summary>
    /// Determines whether or not the specified member username exists in the data store.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <param name="includeDeactivated">Whether to also include deactivated member accounts (or not).</param>
    abstract ExistsMemberUsername : username:string * includeDeactivated:bool -> bool

    /// <summary>
    /// Determines whether or not the specified member email exists in the data store.
    /// </summary>
    /// <param name="username">The email to search for.</param>
    /// <param name="includeDeactivated">Whether to also include deactivated member accounts (or not).</param>
    abstract ExistsMemberEmail : email:string * includeDeactivated:bool -> bool

    /// <summary>
    /// Adds the supplied member data to the data store.
    /// </summary>
    /// <param name="memberData">The member data to add.</param>
    abstract AddMember : memberData:IMemberData -> unit

    /// <summary>
    /// Deletes the supplied member data from the data store.
    /// </summary>
    /// <param name="memberData">The member data to delete.</param>
    abstract DeleteMember : memberData:IMemberData -> unit

    /// Saves all pending changes to the data store.
    abstract SaveChanges : unit -> unit
