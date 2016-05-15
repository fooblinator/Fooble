namespace Fooble.Core.Persistence

open Fooble.Core
open System.Data.Entity

type internal MemberData() =

    member val Id = "" with get, set
    member val Name = "" with get, set

    interface IMemberData with

        member this.Id = this.Id
        member this.Name = this.Name

type internal DataContext() =
    inherit DbContext("name=FoobleContext")

    member this.Members =
        base.Set<MemberData>() :> IDbSet<MemberData>

    interface IDataContext with

        member this.Members =
            this.Members :?> IDbSet<IMemberData>
