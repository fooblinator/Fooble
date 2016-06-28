namespace Fooble.Persistence

open Fooble.Common
open FSharp.Data.TypeProviders
open System

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
                  and set(x) =
#if DEBUG
                      assertWith (validateMemberId x);
#endif
                      memberData.Id <- x

              member __.Username
                  with get() = memberData.Username
                  and set(x) =
#if DEBUG
                      assertWith (validateMemberUsername x)
#endif
                      memberData.Username <- x

              member __.PasswordData
                  with get() = memberData.PasswordData
                  and set(x) =
#if DEBUG
                      assertWith (validateMemberPasswordData x)
#endif
                      memberData.PasswordData <- x

              member __.Email
                  with get() = memberData.Email
                  and set(x) =
#if DEBUG
                      assertWith (validateMemberEmail x)
#endif
                      memberData.Email <- x

              member __.Nickname
                  with get() = memberData.Nickname
                  and set(x) =
#if DEBUG
                      assertWith (validateMemberNickname x)
#endif
                      memberData.Nickname <- x

              member __.AvatarData
                  with get() = memberData.AvatarData
                  and set(x) =
#if DEBUG
                      assertWith (validateMemberAvatarData x)
#endif
                      memberData.AvatarData <- x

              member __.RegisteredOn
                  with get() = memberData.RegisteredOn
                  and set(x) = memberData.RegisteredOn <- x

              member __.PasswordChangedOn
                  with get() = memberData.PasswordChangedOn
                  and set(x) = memberData.PasswordChangedOn <- x

              member __.DeactivatedOn
                  with get() = Option.ofNullable memberData.DeactivatedOn
                  and set(x) = memberData.DeactivatedOn <- Option.toNullable x

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

            member __.GetMember(id, includeDeactivated) =
#if DEBUG
                assertWith (validateMemberId id)
#endif
                if includeDeactivated
                    then query { for x in context.MemberData do
                                 where (x.Id = id)
                                 select x
                                 exactlyOneOrDefault }
                    else query { for x in context.MemberData do
                                 where (x.DeactivatedOn = Nullable())
                                 where (x.Id = id)
                                 select x
                                 exactlyOneOrDefault }
                |> Option.ofObj
                |> Option.map wrapMemberData

            member __.GetMembers(includeDeactivated) =
                if includeDeactivated
                    then query { for x in context.MemberData do
                                 sortBy x.Nickname
                                 select x }
                    else query { for x in context.MemberData do
                                 where (x.DeactivatedOn = Nullable())
                                 sortBy x.Nickname
                                 select x }
                |> Seq.map wrapMemberData
                |> List.ofSeq

            member __.ExistsMemberId(id, includeDeactivated) =
#if DEBUG
                assertWith (validateMemberId id)
#endif
                if includeDeactivated
                    then query { for x in context.MemberData do
                                 select x.Id
                                 contains id }
                    else query { for x in context.MemberData do
                                 where (x.DeactivatedOn = Nullable())
                                 select x.Id
                                 contains id }

            member __.ExistsMemberUsername(username, includeDeactivated) =
#if DEBUG
                assertWith (validateMemberUsername username)
#endif
                if includeDeactivated
                    then query { for x in context.MemberData do
                                 select x.Username
                                 contains username }
                    else query { for x in context.MemberData do
                                 where (x.DeactivatedOn = Nullable())
                                 select x.Username
                                 contains username }

            member __.ExistsMemberEmail(email, includeDeactivated) =
#if DEBUG
                assertWith (validateMemberEmail email)
#endif
                if includeDeactivated
                    then query { for x in context.MemberData do
                                 select x.Email
                                 contains email }
                    else query { for x in context.MemberData do
                                 where (x.DeactivatedOn = Nullable())
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
