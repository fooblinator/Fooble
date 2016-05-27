namespace Fooble.IntegrationTest

open FSharp.Configuration
open Moq
open Moq.FSharp.Extensions
open System.Data.Objects
open System.Linq

type internal Settings = AppSettings<"App.config">

[<AutoOpen>]
module internal Helpers =

    (* Misc *)

    let internal makeObjectSet (data:seq<'T>) =
        let queryable = data.AsQueryable()
        let setMock = Mock<IObjectSet<'T>>()
        setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.Provider).Returns(queryable.Provider).End
        setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.Expression).Returns(queryable.Expression).End
        setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.ElementType).Returns(queryable.ElementType).End
        setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.GetEnumerator()).Returns(queryable.GetEnumerator()).End
        setMock
