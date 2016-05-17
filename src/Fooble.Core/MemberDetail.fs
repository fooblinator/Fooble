namespace Fooble.Core

open Fooble.Core.Persistence
open MediatR
open System.Diagnostics
open System.Runtime.CompilerServices

[<RequireQualifiedAccess>]
module MemberDetail =

    (* Active Patterns *)

    let internal (|IsNotFound|) (status:IMemberDetailQueryFailureStatus) = ()

    (* Validators *)

    [<CompiledName("ValidateId")>]
    let validateId id =
        [ (notIsNull), "Id was null"
          (String.notIsEmpty), "Id was empty string"
          (String.isGuid), "Id was not GUID string" ]
        |> validate id "id"

    [<RequireQualifiedAccess>]
    module Query =

        (* Implementation *)

        type private Implementation =
            { id:string }

            interface IMemberDetailQuery with

                member this.Id = this.id

        (* Construction *)

        [<CompiledName("Make")>]
        let make id =
            raiseIfInvalid <| validateId id
            { id = id } :> IMemberDetailQuery

    [<RequireQualifiedAccess>]
    module internal ReadModel =

        (* Implementation *)

        type private Implementation =
            { id:string; name:string }

            interface IMemberDetailReadModel with

                member this.Id = this.id
                member this.Name = this.name

        (* Construction *)

        let internal make id name =
            Debug.Assert(notIsNull id, "(MemberDetail.ReadModel.make) id argument was null value")
            Debug.Assert(String.notIsEmpty id, "(MemberDetail.ReadModel.make) id argument was empty string")
            Debug.Assert(String.isGuid id, "(MemberDetail.ReadModel.make) id argument was string with invalid GUID format")
            Debug.Assert(notIsNull name, "(MemberDetail.ReadModel.make) name argument was null value")
            Debug.Assert(String.notIsEmpty name, "(MemberDetail.ReadModel.make) name argument was empty string")
            { id = id; name = name } :> IMemberDetailReadModel

    [<RequireQualifiedAccess>]
    module QueryFailureStatus =

        (* Implementation *)

        type private Implementation =
            | NotFound

            interface IMemberDetailQueryFailureStatus with

                member this.IsNotFound =
                    match this with
                    | NotFound -> true

        (* Construction *)

        [<CompiledName("NotFound")>]
        let notFound = NotFound :> IMemberDetailQueryFailureStatus

    [<RequireQualifiedAccess>]
    module internal QueryHandler =

        (* Implementation *)

        type private Implementation(context:IDataContext) =

            interface IMemberDetailQueryHandler with

                member this.Handle(query) =
                    Debug.Assert(notIsNull query, "(IMemberDetailQueryHandler.Handle) query argument was null value")
                    context.Members.Find(query.Id)
                    |> Option.ofObj
                    |> Option.map unbox<MemberData>
                    |> Option.map (fun md -> ReadModel.make md.Id md.Name)
                    |> failureIfNone QueryFailureStatus.notFound

        (* Construction *)

        let internal make context =
            Debug.Assert(notIsNull context, "(MemberDetail.QueryHandler.make) context argument was null value")
            Implementation(context) :> IMemberDetailQueryHandler
