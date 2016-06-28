namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Fooble.Presentation
open Fooble.Web.Controllers
open MediatR
open Moq
open Swensen.Unquote
open System

[<AutoOpen>]
module internal UnitTestHelpers =

    let makeMemberController mediator idGeneratorResult =
        let mediator =
            match mediator with
            | Some(x) -> x
            | _ -> Mock.Of<IMediator>()
        let idGenerator = makeIdGenerator idGeneratorResult
        new MemberController(mediator, idGenerator,
            InitialMemberChangePasswordViewModelFactory(MemberChangePasswordViewModel.makeInitial),
            InitialMemberDeactivateViewModelFactory(MemberDeactivateViewModel.makeInitial),
            MemberDetailQueryFactory(MemberDetailQuery.make),
            MemberEmailQueryFactory(MemberEmailQuery.make),
            MemberExistsQueryFactory(MemberExistsQuery.make),
            MemberListQueryFactory(MemberListQuery.make),
            MemberOtherQueryFactory(MemberOtherQuery.make),
            InitialMemberRegisterViewModelFactory(MemberRegisterViewModel.makeInitial),
            MemberUsernameQueryFactory(MemberUsernameQuery.make))

    let makeMemberData id username passwordData email nickname avatarData registeredOn passwordChangedOn
        deactivatedOn =

        let memberData = wrapMemberData (MemberData())
        memberData.Id <-
            match id with
            | Some(x) -> x
            | None -> Guid.NewGuid()
        memberData.Username <-
            match username with
            | Some(x) -> x
            | None -> randomString 32
        memberData.PasswordData <-
            match passwordData with
            | Some(x) -> x
            | None -> randomPassword 32 |> fun x -> Crypto.hash x 100
        memberData.Email <-
            match email with
            | Some(x) -> x
            | None -> randomEmail 32
        memberData.Nickname <-
            match nickname with
            | Some(x) -> x
            | None -> randomString 64
        memberData.AvatarData <-
            match avatarData with
            | Some(x) -> x
            | None -> randomString 32
        memberData.RegisteredOn <-
            match registeredOn with
            | Some(x) -> x
            | None -> DateTime(2001, 1, 1)
        memberData.PasswordChangedOn <-
            match passwordChangedOn with
            | Some(x) -> x
            | None -> DateTime(2001, 1, 1)
        memberData.DeactivatedOn <- deactivatedOn
        memberData

    let randomPasswordWithInvalidChars len =
        randomPassword (len - 2)
        |> fun x -> sprintf "%c%s%c" Char.MinValue x Char.MaxValue

    let randomPasswordWithoutDigitChars = randomPasswordWith false true true true
    let randomPasswordWithoutLowercaseChars = randomPasswordWith true false true true
    let randomPasswordWithoutSpecialChars = randomPasswordWith true true true false
    let randomPasswordWithoutUppercaseChars = randomPasswordWith true true false true

    let testInvalidOperationException message expression =
        assert (not (isNull message))
        raisesWith<InvalidOperationException> expression (fun x -> <@ x.Message = message @>)

    let testMemberChangeEmailCommand (actual:IMemberChangeEmailCommand) id currentPassword email =
        actual.Id =! id
        actual.CurrentPassword =! currentPassword
        actual.Email =! email

    let testMemberChangeOtherCommand (actual:IMemberChangeOtherCommand) id nickname avatarData =
        actual.Id =! id
        actual.Nickname =! nickname
        actual.AvatarData =! avatarData

    let testMemberChangePasswordCommand (actual:IMemberChangePasswordCommand) id currentPassword password =
        actual.Id =! id
        actual.CurrentPassword =! currentPassword
        actual.Password =! password

    let testMemberChangeUsernameCommand (actual:IMemberChangeUsernameCommand) id currentPassword username =
        actual.Id =! id
        actual.CurrentPassword =! currentPassword
        actual.Username =! username

    let testMemberDeactivateCommand (actual:IMemberDeactivateCommand) id currentPassword =
        actual.Id =! id
        actual.CurrentPassword =! currentPassword

    let testMemberRegisterCommand (actual:IMemberRegisterCommand) id username password email nickname avatarData =
        actual.Id =! id
        actual.Username =! username
        actual.Password =! password
        actual.Email =! email
        actual.Nickname =! nickname
        actual.AvatarData =! avatarData
