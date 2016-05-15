[<AutoOpen>]
module internal Fooble.Tests.TestHelpers

open Moq
open Moq.FSharp.Extensions
open System
open System.Data.Entity
open System.Linq

let internal fixArgumentExceptionMessage (message:string) =
    (message.IndexOf "Parameter name: " |> message.Remove).Trim()

let internal makeDbSet (data:seq<'T>) =
    let queryable = data.AsQueryable()

    let setMock = Mock<IDbSet<'T>>()
    
    setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.Provider).Returns(queryable.Provider).End
    setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.Expression).Returns(queryable.Expression).End
    setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.ElementType).Returns(queryable.ElementType).End
    setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.GetEnumerator()).Returns(queryable.GetEnumerator()).End

    setMock.Object

let internal randomGuidString () =
    sprintf "%A" (Guid.NewGuid())

let internal randomNonGuidString () =
    (randomGuidString ()).ToCharArray()
    |> Array.filter (fun c -> c <> '-')
    |> Array.take 16
    |> String
