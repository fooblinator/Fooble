namespace Fooble.Core.Persistence

open FSharp.Data.TypeProviders
open System
open System.Data.Objects

(* Persistence *)

type internal EntityConnection = SqlEntityConnection<ConnectionStringName = "FoobleContext">
type internal FoobleContext = EntityConnection.ServiceTypes.EntityContainer
type internal MemberData = EntityConnection.ServiceTypes.MemberData

[<AllowNullLiteral>]
type internal IFoobleContext =
    inherit IDisposable
    abstract MemberData : IObjectSet<MemberData>
    abstract SaveChanges : unit -> int

[<AutoOpen>]
module internal Helpers =

    (* Helpers *)

    let internal makeFoobleContext connectionString =
        let context =
            match connectionString with
            | Some x -> EntityConnection.GetDataContext(x).DataContext :?> FoobleContext
            | None -> EntityConnection.GetDataContext().DataContext :?> FoobleContext

        { new IFoobleContext with
              member this.Dispose() = context.Dispose()
              member this.MemberData = context.MemberData :> IObjectSet<MemberData>
              member this.SaveChanges() = context.SaveChanges() }
