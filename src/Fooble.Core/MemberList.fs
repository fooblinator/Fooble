namespace Fooble.Core

open MediatR
open System.Diagnostics
open System.Runtime.CompilerServices

[<RequireQualifiedAccess>]
module MemberList =

    (* Active Patterns *)

    let internal (|IsNotFound|) (status:IMemberListQueryFailureStatus) = ()

    [<RequireQualifiedAccess>]
    module Query =

        (* Implementation *)

        type private Implementation() =
            interface IMemberListQuery

        (* Construction *)

        [<CompiledName("Make")>]
        let make() =
            Implementation() :> IMemberListQuery

    [<RequireQualifiedAccess>]
    module internal ItemReadModel =

        (* Implementation *)

        type private Implementation =
            { id:string; name:string }

            interface IMemberListItemReadModel with

                member this.Id = this.id
                member this.Name = this.name

        (* Construction *)

        let internal make id name =
            Debug.Assert(notIsNull id, "(MemberList.ItemReadModel.make) id argument was null value")
            Debug.Assert(String.notIsEmpty id, "(MemberList.ItemReadModel.make) id argument was empty string")
            Debug.Assert(String.isGuid id, "(MemberList.ItemReadModel.make) id argument was string with invalid GUID format")
            Debug.Assert(notIsNull name, "(MemberList.ItemReadModel.make) name argument was null value")
            Debug.Assert(String.notIsEmpty name, "(MemberList.ItemReadModel.make) name argument was empty string")
            { id = id; name = name } :> IMemberListItemReadModel

    [<RequireQualifiedAccess>]
    module internal ReadModel =

        (* Implementation *)

        type private Implementation =
            { members:seq<IMemberListItemReadModel> }

            interface IMemberListReadModel with

                member this.Members = this.members

        (* Construction *)

        let internal make members =
            Debug.Assert(notIsNull members, "(MemberList.ReadModel.make) members argument was null value")
            Debug.Assert(Seq.notIsEmpty members, "(MemberList.ReadModel.make) members argument was empty value")
            Debug.Assert(Seq.notContainsNulls members, "(MemberList.ReadModel.make) members argument contained null values")
            { members = members } :> IMemberListReadModel

    [<RequireQualifiedAccess>]
    module QueryFailureStatus =

        (* Implementation *)

        type private Implementation =
            | NotFound

            interface IMemberListQueryFailureStatus with

                member this.IsNotFound =
                    match this with
                    | NotFound -> true

        (* Construction *)

        [<CompiledName("NotFound")>]
        let notFound = NotFound :> IMemberListQueryFailureStatus

    [<RequireQualifiedAccess>]
    module internal QueryHandler =

        (* Implementation *)

        type private Implementation(context:IDataContext) =

            interface IMemberListQueryHandler with

                member this.Handle(query) =
                    Debug.Assert(notIsNull query, "(IMemberListQueryHandler.Handle) query argument was null value")
                    match List.ofSeq context.Members with
                    | [] -> Result.failure QueryFailureStatus.notFound
                    | mds ->
                        List.map (fun (md:IMemberData) -> ItemReadModel.make md.Id md.Name) mds
                        |> Seq.ofList
                        |> ReadModel.make
                        |> Result.success

        (* Construction *)

        let internal make context =
            Debug.Assert(notIsNull context, "(MemberList.QueryHandler.make) context argument was null value")
            Implementation(context) :> IMemberListQueryHandler
