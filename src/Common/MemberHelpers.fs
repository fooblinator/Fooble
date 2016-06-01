namespace Fooble.Common

open Fooble.Persistence

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

    let assertMemberNickname nickname =
        assert (String.isNotNullOrEmpty nickname)
        assert (String.isNotLonger 64 nickname)

    let assertMemberData (memberData:IMemberData) =
        assertMemberId memberData.Id
        assertMemberUsername memberData.Username
        assertMemberEmail memberData.Email
        assertMemberNickname memberData.Nickname
