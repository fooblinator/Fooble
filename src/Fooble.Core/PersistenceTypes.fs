namespace Fooble.Core.Persistence

open Fooble.Core
open System.Data.Entity

(* Member Data *)

[<AllowNullLiteral>]
type internal IMemberData =
    abstract Id:string
    abstract Name:string

type internal MemberData internal () =
    member val internal Id = "" with get, set
    member val internal Name = "" with get, set

    interface IMemberData with
        member this.Id = this.Id
        member this.Name = this.Name

(* Data Context *)

[<AllowNullLiteral>]
type internal IDataContext =
    abstract Members:IDbSet<IMemberData>

type internal DataContext internal () =
    inherit DbContext("name=FoobleContext")
    member private this.Members = base.Set<MemberData>() :> IDbSet<MemberData>

    interface IDataContext with
        member this.Members = this.Members :?> IDbSet<IMemberData>
