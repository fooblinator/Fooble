namespace Fooble.Persistence

open System

[<AutoOpen>]
module internal Helpers =

    let internal wrapMemberData (memberData:MemberData) =
        { new IMemberData with

              member this.Id
                  with get() = memberData.Id
                  and set(x) = memberData.Id <- x

              member this.Username
                  with get() = memberData.Username
                  and set(x) = memberData.Username <- x

              member this.Email
                  with get() = memberData.Email
                  and set(x) = memberData.Email <- x

              member this.Nickname
                  with get() = memberData.Nickname
                  and set(x) = memberData.Nickname <- x

          interface IExposeWrapped<MemberData> with

              member this.Inner
                  with get() = memberData }

    let internal unwrapMemberData (memberData:IMemberData) =
        assert (memberData :? IExposeWrapped<MemberData>)
        (memberData :?> IExposeWrapped<MemberData>).Inner

    let internal wrapFoobleContext (context:FoobleContext) =
        { new IFoobleContext with

            member this.GetMember(id:Guid) : IMemberData option =
                query { for x in context.MemberData do
                        where (x.Id = id)
                        select x
                        exactlyOneOrDefault }
                |> Option.ofObj
                |> Option.map wrapMemberData

            member this.GetMembers() : IMemberData list =
                query { for x in context.MemberData do
                        sortBy x.Nickname 
                        select x }
                |> Seq.map wrapMemberData
                |> List.ofSeq

            member this.ExistsMemberUsername(username:string) : bool =
                query { for x in context.MemberData do
                        select x.Username
                        contains username }

            member this.ExistsMemberEmail(email:string) : bool =
                query { for x in context.MemberData do
                        select x.Email
                        contains email }

            member this.AddMember(memberData:IMemberData) =
                unwrapMemberData memberData
                |> context.MemberData.AddObject

            member this.DeleteMember(memberData:IMemberData) =
                unwrapMemberData memberData
                |> context.MemberData.DeleteObject

            member this.SaveChanges() =
                ignore <| context.SaveChanges()

            member this.Dispose() = context.Dispose() }
