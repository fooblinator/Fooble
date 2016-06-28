namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Presentation
open Fooble.Web.Controllers
open MediatR
open Moq
open NUnit.Framework

[<TestFixture>]
module MemberControllerTests =

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        ignore (
            new MemberController(
                Mock.Of<IMediator>(),
                Mock.Of<IdGenerator>(),
                Mock.Of<InitialMemberChangePasswordViewModelFactory>(),
                Mock.Of<InitialMemberDeactivateViewModelFactory>(),
                Mock.Of<MemberDetailQueryFactory>(),
                Mock.Of<MemberEmailQueryFactory>(),
                Mock.Of<MemberExistsQueryFactory>(),
                Mock.Of<MemberListQueryFactory>(),
                Mock.Of<MemberOtherQueryFactory>(),
                Mock.Of<InitialMemberRegisterViewModelFactory>(),
                Mock.Of<MemberUsernameQueryFactory>()))

    [<Test>]
    let ``Constructing, with null mediator, raises expected exception`` () =
        testArgumentException "mediator" "Mediator is required"
            <@ new MemberController(
                null,
                Mock.Of<IdGenerator>(),
                Mock.Of<InitialMemberChangePasswordViewModelFactory>(),
                Mock.Of<InitialMemberDeactivateViewModelFactory>(),
                Mock.Of<MemberDetailQueryFactory>(),
                Mock.Of<MemberEmailQueryFactory>(),
                Mock.Of<MemberExistsQueryFactory>(),
                Mock.Of<MemberListQueryFactory>(),
                Mock.Of<MemberOtherQueryFactory>(),
                Mock.Of<InitialMemberRegisterViewModelFactory>(),
                Mock.Of<MemberUsernameQueryFactory>()) @>

    [<Test>]
    let ``Constructing, with null id generator, raises expected exception`` () =
        testArgumentException "idGenerator" "Id generator is required"
            <@ new MemberController(
                Mock.Of<IMediator>(),
                null,
                Mock.Of<InitialMemberChangePasswordViewModelFactory>(),
                Mock.Of<InitialMemberDeactivateViewModelFactory>(),
                Mock.Of<MemberDetailQueryFactory>(),
                Mock.Of<MemberEmailQueryFactory>(),
                Mock.Of<MemberExistsQueryFactory>(),
                Mock.Of<MemberListQueryFactory>(),
                Mock.Of<MemberOtherQueryFactory>(),
                Mock.Of<InitialMemberRegisterViewModelFactory>(),
                Mock.Of<MemberUsernameQueryFactory>()) @>

    [<Test>]
    let ``Constructing, with null member change password view model factory, raises expected exception`` () =
        testArgumentException "memberChangePasswordViewModelFactory"
            "Member change password view model factory is required"
            <@ new MemberController(
                Mock.Of<IMediator>(),
                Mock.Of<IdGenerator>(),
                null,
                Mock.Of<InitialMemberDeactivateViewModelFactory>(),
                Mock.Of<MemberDetailQueryFactory>(),
                Mock.Of<MemberEmailQueryFactory>(),
                Mock.Of<MemberExistsQueryFactory>(),
                Mock.Of<MemberListQueryFactory>(),
                Mock.Of<MemberOtherQueryFactory>(),
                Mock.Of<InitialMemberRegisterViewModelFactory>(),
                Mock.Of<MemberUsernameQueryFactory>()) @>

    [<Test>]
    let ``Constructing, with null member deactivate view model factory, raises expected exception`` () =
        testArgumentException "memberDeactivateViewModelFactory" "Member deactivate view model factory is required"
            <@ new MemberController(
                Mock.Of<IMediator>(),
                Mock.Of<IdGenerator>(),
                Mock.Of<InitialMemberChangePasswordViewModelFactory>(),
                null,
                Mock.Of<MemberDetailQueryFactory>(),
                Mock.Of<MemberEmailQueryFactory>(),
                Mock.Of<MemberExistsQueryFactory>(),
                Mock.Of<MemberListQueryFactory>(),
                Mock.Of<MemberOtherQueryFactory>(),
                Mock.Of<InitialMemberRegisterViewModelFactory>(),
                Mock.Of<MemberUsernameQueryFactory>()) @>

    [<Test>]
    let ``Constructing, with null member detail query factory, raises expected exception`` () =
        testArgumentException "memberDetailQueryFactory" "Member detail query factory is required"
            <@ new MemberController(
                Mock.Of<IMediator>(),
                Mock.Of<IdGenerator>(),
                Mock.Of<InitialMemberChangePasswordViewModelFactory>(),
                Mock.Of<InitialMemberDeactivateViewModelFactory>(),
                null,
                Mock.Of<MemberEmailQueryFactory>(),
                Mock.Of<MemberExistsQueryFactory>(),
                Mock.Of<MemberListQueryFactory>(),
                Mock.Of<MemberOtherQueryFactory>(),
                Mock.Of<InitialMemberRegisterViewModelFactory>(),
                Mock.Of<MemberUsernameQueryFactory>()) @>

    [<Test>]
    let ``Constructing, with null member email query factory, raises expected exception`` () =
        testArgumentException "memberEmailQueryFactory" "Member email query factory is required"
            <@ new MemberController(
                Mock.Of<IMediator>(),
                Mock.Of<IdGenerator>(),
                Mock.Of<InitialMemberChangePasswordViewModelFactory>(),
                Mock.Of<InitialMemberDeactivateViewModelFactory>(),
                Mock.Of<MemberDetailQueryFactory>(),
                null,
                Mock.Of<MemberExistsQueryFactory>(),
                Mock.Of<MemberListQueryFactory>(),
                Mock.Of<MemberOtherQueryFactory>(),
                Mock.Of<InitialMemberRegisterViewModelFactory>(),
                Mock.Of<MemberUsernameQueryFactory>()) @>

    [<Test>]
    let ``Constructing, with null member exists query factory, raises expected exception`` () =
        testArgumentException "memberExistsQueryFactory" "Member exists query factory is required"
            <@ new MemberController(
                Mock.Of<IMediator>(),
                Mock.Of<IdGenerator>(),
                Mock.Of<InitialMemberChangePasswordViewModelFactory>(),
                Mock.Of<InitialMemberDeactivateViewModelFactory>(),
                Mock.Of<MemberDetailQueryFactory>(),
                Mock.Of<MemberEmailQueryFactory>(),
                null,
                Mock.Of<MemberListQueryFactory>(),
                Mock.Of<MemberOtherQueryFactory>(),
                Mock.Of<InitialMemberRegisterViewModelFactory>(),
                Mock.Of<MemberUsernameQueryFactory>()) @>

    [<Test>]
    let ``Constructing, with null member list query factory, raises expected exception`` () =
        testArgumentException "memberListQueryFactory" "Member list query factory is required"
            <@ new MemberController(
                Mock.Of<IMediator>(),
                Mock.Of<IdGenerator>(),
                Mock.Of<InitialMemberChangePasswordViewModelFactory>(),
                Mock.Of<InitialMemberDeactivateViewModelFactory>(),
                Mock.Of<MemberDetailQueryFactory>(),
                Mock.Of<MemberEmailQueryFactory>(),
                Mock.Of<MemberExistsQueryFactory>(),
                null,
                Mock.Of<MemberOtherQueryFactory>(),
                Mock.Of<InitialMemberRegisterViewModelFactory>(),
                Mock.Of<MemberUsernameQueryFactory>()) @>

    [<Test>]
    let ``Constructing, with null member other query factory, raises expected exception`` () =
        testArgumentException "memberOtherQueryFactory" "Member other query factory is required"
            <@ new MemberController(
                Mock.Of<IMediator>(),
                Mock.Of<IdGenerator>(),
                Mock.Of<InitialMemberChangePasswordViewModelFactory>(),
                Mock.Of<InitialMemberDeactivateViewModelFactory>(),
                Mock.Of<MemberDetailQueryFactory>(),
                Mock.Of<MemberEmailQueryFactory>(),
                Mock.Of<MemberExistsQueryFactory>(),
                Mock.Of<MemberListQueryFactory>(),
                null,
                Mock.Of<InitialMemberRegisterViewModelFactory>(),
                Mock.Of<MemberUsernameQueryFactory>()) @>

    [<Test>]
    let ``Constructing, with null member register view model factory, raises expected exception`` () =
        testArgumentException "memberRegisterViewModelFactory" "Member register view model factory is required"
            <@ new MemberController(
                Mock.Of<IMediator>(),
                Mock.Of<IdGenerator>(),
                Mock.Of<InitialMemberChangePasswordViewModelFactory>(),
                Mock.Of<InitialMemberDeactivateViewModelFactory>(),
                Mock.Of<MemberDetailQueryFactory>(),
                Mock.Of<MemberEmailQueryFactory>(),
                Mock.Of<MemberExistsQueryFactory>(),
                Mock.Of<MemberListQueryFactory>(),
                Mock.Of<MemberOtherQueryFactory>(),
                null,
                Mock.Of<MemberUsernameQueryFactory>()) @>

    [<Test>]
    let ``Constructing, with null member username query factory, raises expected exception`` () =
        testArgumentException "memberUsernameQueryFactory" "Member username query factory is required"
            <@ new MemberController(
                Mock.Of<IMediator>(),
                Mock.Of<IdGenerator>(),
                Mock.Of<InitialMemberChangePasswordViewModelFactory>(),
                Mock.Of<InitialMemberDeactivateViewModelFactory>(),
                Mock.Of<MemberDetailQueryFactory>(),
                Mock.Of<MemberEmailQueryFactory>(),
                Mock.Of<MemberExistsQueryFactory>(),
                Mock.Of<MemberListQueryFactory>(),
                Mock.Of<MemberOtherQueryFactory>(),
                Mock.Of<InitialMemberRegisterViewModelFactory>(),
                null) @>
