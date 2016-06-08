namespace Fooble.Persistence

open Fooble.Common
open FSharp.Data.TypeProviders

type internal EntityConnection = SqlEntityConnection<ConnectionStringName = "FoobleContext">
type internal FoobleContext = EntityConnection.ServiceTypes.EntityContainer
type internal MemberData = EntityConnection.ServiceTypes.MemberData

type internal IExposeWrapped<'T> = abstract Inner:'T with get

[<AutoOpen>]
module internal PersistenceHelpers =

    let wrapMemberData (memberData:MemberData) =
#if DEBUG
        assertWith (validateRequired memberData "memberData" "Member data")
#endif

        { new IMemberData with

              member __.Id
                  with get() = memberData.Id
                  and set(x) = memberData.Id <- x

              member __.Username
                  with get() = memberData.Username
                  and set(x) = memberData.Username <- x

              member __.PasswordData
                  with get() = memberData.PasswordData
                  and set(x) = memberData.PasswordData <- x

              member __.Email
                  with get() = memberData.Email
                  and set(x) = memberData.Email <- x

              member __.Nickname
                  with get() = memberData.Nickname
                  and set(x) = memberData.Nickname <- x

              member __.Registered
                  with get() = memberData.Registered
                  and set(x) = memberData.Registered <- x

              member __.PasswordChanged
                  with get() = memberData.PasswordChanged
                  and set(x) = memberData.PasswordChanged <- x

          interface IExposeWrapped<MemberData> with

              member __.Inner
                  with get() = memberData }

    let private unwrapMemberData (memberData:IMemberData) =
#if DEBUG
        assertWith (validateRequired memberData "memberData" "Member data")
#endif
        (memberData :?> IExposeWrapped<MemberData>).Inner

    let wrapFoobleContext (context:FoobleContext) =
#if DEBUG
        assertWith (validateRequired context "context" "Context")
#endif

        { new IFoobleContext with

            member __.GetMember(id) =
#if DEBUG
                assertWith (validateMemberId id)
#endif
                query { for x in context.MemberData do
                        where (x.Id = id)
                        select x
                        exactlyOneOrDefault }
                |> Option.ofObj
                |> Option.map wrapMemberData

            member __.GetMembers() =
                query { for x in context.MemberData do
                        sortBy x.Nickname
                        select x }
                |> Seq.map wrapMemberData
                |> List.ofSeq

            member __.ExistsMemberId(id) =
#if DEBUG
                assertWith (validateMemberId id)
#endif
                query { for x in context.MemberData do
                        select x.Id
                        contains id }

            member __.ExistsMemberUsername(username) =
#if DEBUG
                assertWith (validateMemberUsername username)
#endif
                query { for x in context.MemberData do
                        select x.Username
                        contains username }

            member __.ExistsMemberEmail(email) =
#if DEBUG
                assertWith (validateMemberEmail email)
#endif
                query { for x in context.MemberData do
                        select x.Email
                        contains email }

            member __.AddMember(memberData) =
#if DEBUG
                assertWith (validateMemberId memberData.Id)
                assertWith (validateMemberUsername memberData.Username)
                assertWith (validateMemberPasswordData memberData.PasswordData)
                assertWith (validateMemberEmail memberData.Email)
                assertWith (validateMemberNickname memberData.Nickname)
#endif
                unwrapMemberData memberData
                |> context.MemberData.AddObject

            member __.DeleteMember(memberData) =
#if DEBUG
                assertWith (validateMemberId memberData.Id)
                assertWith (validateMemberUsername memberData.Username)
                assertWith (validateMemberPasswordData memberData.PasswordData)
                assertWith (validateMemberEmail memberData.Email)
                assertWith (validateMemberNickname memberData.Nickname)
#endif
                unwrapMemberData memberData
                |> context.MemberData.DeleteObject

            member __.SaveChanges() =
                ignore (context.SaveChanges())

            member __.Dispose() = context.Dispose() }
