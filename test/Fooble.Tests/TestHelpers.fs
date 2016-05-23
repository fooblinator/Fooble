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
    module internal String =

        let internal empty = System.String.Empty
        let internal isEmpty x = x = empty
        let internal notIsEmpty x = not <| isEmpty x
        let internal isGuid x = fst <| Guid.TryParse(x)
        let internal notIsGuid x = not <| isGuid x

    [<RequireQualifiedAccess>]
    module internal List =

        let internal containsEmptyStrings x = List.contains String.empty x
        let internal containsNulls x = List.contains null x
        let internal notContainsEmptyStrings x = not <| containsEmptyStrings x
        let internal notContainsNulls x = not <| containsNulls x
        let internal notIsEmpty x = not <| List.isEmpty x

    [<RequireQualifiedAccess>]
    module internal Seq =

        let internal containsEmptyStrings x = Seq.contains String.empty x
        let internal containsNulls x = Seq.contains null x
        let internal notContainsEmptyStrings x = not <| containsEmptyStrings x
        let internal notContainsNulls x = not <| containsNulls x
        let internal notIsEmpty x = not <| Seq.isEmpty x

    (* Misc *)

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

    let internal notIsNull x = not <| isNull x
    let internal randomGuid () = Guid.NewGuid()
    let internal randomString () = sprintf "%A" <| randomGuid ()

    let internal randomNonGuidString () =
        (randomString ()).ToCharArray()
        |> Array.filter (fun c -> c <> '-')
        |> Array.take 16
        |> String
