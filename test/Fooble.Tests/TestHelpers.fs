namespace Fooble.Tests

open FSharp.Configuration
open Moq
open Moq.FSharp.Extensions
open System
open System.Data.Objects
open System.Linq

type internal Settings = AppSettings<"app.config">

[<AutoOpen>]
module internal Helpers =

    (* Extensions *)

    [<RequireQualifiedAccess>]
    module internal Guid =
        let internal empty = Guid.Empty
        let internal isEmpty x = x = empty
        let internal isNotEmpty x = not <| isEmpty x
        let internal random () = Guid.NewGuid()
        let internal toString (x:Guid) = x.ToString()

    [<RequireQualifiedAccess>]
    module internal String =
        let internal empty = String.Empty
        let internal isEmpty x = x = empty
        let internal ofGuid x = Guid.toString x
        let internal toArray (x:string) = x.ToCharArray()

        let internal random len =
            assert (len > 0)
            Seq.init (len / 32 + 1) (fun _ -> Guid.random () |> ofGuid |> toArray)
            |> Array.concat
            |> Array.filter (fun x -> x <> '-')
            |> Array.take len
            |> String

    (* Misc *)

    let internal isNotNull x = not <| isNull x

    let internal fixInvalidArgMessage (message:string) =
        let i = message.IndexOf("Parameter name: ")
        message.Remove(i).Trim()

    let internal makeObjectSet (data:seq<'T>) =
        let queryable = data.AsQueryable()
        let setMock = Mock<IObjectSet<'T>>()
        setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.Provider).Returns(queryable.Provider).End
        setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.Expression).Returns(queryable.Expression).End
        setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.ElementType).Returns(queryable.ElementType).End
        setMock.As<IQueryable<'T>>().SetupFunc(fun m -> m.GetEnumerator()).Returns(queryable.GetEnumerator()).End
        setMock
