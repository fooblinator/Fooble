namespace Fooble.Common

open Fooble.Persistence
open System.Diagnostics

[<DebuggerStepThrough>]
[<AutoOpen>]
module internal MemberHelpers =

    let assertMemberId id =
        assert (Guid.isNotEmpty id)

    let assertMemberUsername username =
        assert (String.isNotNullOrEmpty username)
        assert (String.isNotShorter 3 username)
        assert (String.isNotLonger 32 username)
        assert (String.isMatch "^[a-z0-9]+$" username)

    let assertMemberEmail email =
        assert (String.isNotNullOrEmpty email)
        assert (String.isNotLonger 254 email)
        assert (String.isEmail email)

    let assertMemberPassword password =
        assert (String.isNotNullOrEmpty password)
        assert (String.isNotShorter 8 password)
        assert (String.isNotLonger 32 password)
        assert (Password.hasDigits password)
        assert (Password.hasLowerAlphas password)
        assert (Password.hasUpperAlphas password)
        assert (Password.hasSpecialChars password)
        assert (Password.isMatch password)

    let assertMemberNickname nickname =
        assert (String.isNotNullOrEmpty nickname)
        assert (String.isNotLonger 64 nickname)

    let assertMemberData (memberData:IMemberData) =
        assertMemberId memberData.Id
        assertMemberUsername memberData.Username
        assertMemberPassword memberData.Password
        assertMemberEmail memberData.Email
        assertMemberNickname memberData.Nickname
