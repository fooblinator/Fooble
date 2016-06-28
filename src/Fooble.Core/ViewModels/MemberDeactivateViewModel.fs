namespace Fooble.Presentation

open Fooble.Common
open System

[<RequireQualifiedAccess>]
module internal MemberDeactivateViewModel =

    [<DefaultAugmentation(false)>]
    type private MemberDeactivateViewModelImpl =
        | ViewModel of id:Guid * currentPassword:string

        interface IMemberDeactivateViewModel with

            member this.Id
                with get() =
                    match this with
                    | ViewModel(id = x) -> x

            member this.CurrentPassword
                with get() =
                    match this with
                    | ViewModel(currentPassword = x) -> x

    let makeInitial id =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, String.Empty) :> IMemberDeactivateViewModel

    let make id currentPassword =
#if DEBUG
        assertWith (validateMemberId id)
#endif
        ViewModel(id, currentPassword) :> IMemberDeactivateViewModel
