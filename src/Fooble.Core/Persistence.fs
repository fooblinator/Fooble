namespace Fooble.Core.Persistence

open Fooble.Core
open FSharp.Data.TypeProviders
open System
open System.Data.Objects
open System.Diagnostics

(* Persistence *)

type internal EntityConnection =
    SqlEntityConnection<ConnectionStringName = "FoobleContext", EntityContainer = "FoobleContext">

type internal MemberData = EntityConnection.ServiceTypes.MemberData
type internal FoobleContext = EntityConnection.ServiceTypes.FoobleContext

[<AllowNullLiteral>]
type internal IFoobleContext =
    inherit IDisposable
    abstract MemberData : IObjectSet<MemberData>
    abstract SaveChanges : unit -> int

[<AutoOpen>]
module internal Helpers =
    
    let internal wrapFoobleContext (context:FoobleContext) =
        Debug.Assert(notIsNull context, "Context parameter was null")
        { new IFoobleContext with

              member this.Dispose() =
                  context.Dispose()

              member this.MemberData =
                  context.MemberData :> IObjectSet<MemberData>

              member this.SaveChanges() =
                  context.SaveChanges() }
              
    let internal makeFoobleContext connectionString =
        match connectionString with
        | Some x -> EntityConnection.GetDataContext(x).DataContext :?> FoobleContext
        | None -> EntityConnection.GetDataContext().DataContext :?> FoobleContext
        |> wrapFoobleContext
