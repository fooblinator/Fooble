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
        assert (isNotNull memberData)
        { new IMemberData with

              member this.Id
                  with get() = memberData.Id
                  and set(x) = memberData.Id <- x

              member this.Username
                  with get() = memberData.Username
                  and set(x) = memberData.Username <- x

              member this.PasswordData
                  with get() = memberData.PasswordData
                  and set(x) = memberData.PasswordData <- x

              member this.Email
                  with get() = memberData.Email
                  and set(x) = memberData.Email <- x

              member this.Nickname
                  with get() = memberData.Nickname
                  and set(x) = memberData.Nickname <- x

              member this.Registered
                  with get() = memberData.Registered
                  and set(x) = memberData.Registered <- x

              member this.PasswordChanged
                  with get() = memberData.PasswordChanged
                  and set(x) = memberData.PasswordChanged <- x

          interface IExposeWrapped<MemberData> with

              member this.Inner
                  with get() = memberData }

    let private unwrapMemberData (memberData:IMemberData) =
        assert (memberData :? IExposeWrapped<MemberData>)
        (memberData :?> IExposeWrapped<MemberData>).Inner

    let wrapFoobleContext (context:FoobleContext) =
        assert (isNotNull context)
        { new IFoobleContext with

            member this.GetMember(id) =
                assertMemberId id
                query { for x in context.MemberData do
                        where (x.Id = id)
                        select x
                        exactlyOneOrDefault }
                |> Option.ofObj
                |> Option.map wrapMemberData

            member this.GetMembers() =
                query { for x in context.MemberData do
                        sortBy x.Nickname
                        select x }
                |> Seq.map wrapMemberData
                |> List.ofSeq

            member this.ExistsMemberUsername(username) =
                assertMemberUsername username
                query { for x in context.MemberData do
                        select x.Username
                        contains username }

            member this.ExistsMemberEmail(email) =
                assertMemberEmail email
                query { for x in context.MemberData do
                        select x.Email
                        contains email }

            member this.AddMember(memberData) =
                assertMemberData memberData
                unwrapMemberData memberData
                |> context.MemberData.AddObject

            member this.DeleteMember(memberData) =
                assertMemberData memberData
                unwrapMemberData memberData
                |> context.MemberData.DeleteObject

            member this.SaveChanges() =
                ignore (context.SaveChanges())

            member this.Dispose() = context.Dispose() }
