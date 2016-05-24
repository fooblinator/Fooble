namespace Fooble.Core

open Fooble.Core.Persistence
open MediatR
open System
open System.Diagnostics

/// Provides functionality used in the gathering and persisting of member details.
[<RequireQualifiedAccess>]
module SelfServiceRegister =

    (* Command *)

    /// Provides functionality used in the persisting of member details.
    [<RequireQualifiedAccess>]
    module Command =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | Command of Guid * string

            interface ISelfServiceRegisterCommand with

                member this.Id
                    with get() =
                        match this with
                        | Command (x, _) -> x

                member this.Name
                    with get() =
                        match this with
                        | Command (_, x) -> x

        /// <summary>
        /// Constructs a self-service register command.
        /// </summary>
        /// <param name="id">The id that will potentially represent the member.</param>
        /// <param name="name">The name of the potential member.</param>
        /// <returns>Returns a self-service register command.</returns>
        [<CompiledName("Make")>]
        let make id name =
            Validation.raiseIfInvalid <| Member.validateId id
            Validation.raiseIfInvalid <| Member.validateName name
            Command (id, name) :> ISelfServiceRegisterCommand



    (* View Model *)

    /// Provides functionality used in the gathering of member details.
    [<RequireQualifiedAccess>]
    module ViewModel =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | ViewModel of string

            interface ISelfServiceRegisterViewModel with

                member this.Name
                    with get() =
                        match this with
                        | ViewModel x -> x
    
        /// <summary>
        /// Represents an empty self-service register view model.
        /// </summary>
        /// <returns>Returns an empty self-service register view model.</returns>
        [<CompiledName("Empty")>]
        let empty = ViewModel String.empty :> ISelfServiceRegisterViewModel
    
        /// <summary>
        /// Constructs a self-service register view model.
        /// </summary>
        /// <param name="name">The name of the potential member.</param>
        /// <remarks>
        /// Does not validate parameters. This allows for re-construction of the view model with previously-submitted,
        /// and potentially invalid form data. Need to manually validate and handle submitted form data.
        /// </remarks>
        /// <returns>Returns a self-service register view model.</returns>
        [<CompiledName("Make")>]
        let make name = ViewModel name :> ISelfServiceRegisterViewModel



    (* Command Handler *)

    [<RequireQualifiedAccess>]
    module internal CommandHandler =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | CommandHandler of IFoobleContext

            member private this.Context
                with get() =
                    match this with
                    | CommandHandler x -> x

            interface IRequestHandler<ISelfServiceRegisterCommand, Unit> with

                member this.Handle(command) =
                    Debug.Assert(notIsNull <| box command, "Command parameter was null")
                    
                    MemberData(Id = command.Id, Name = command.Name)
                    |> this.Context.MemberData.AddObject

                    ignore <| this.Context.SaveChanges()

                    Unit.Value

        let internal make context =
            Debug.Assert(notIsNull context, "Context parameter was null")
            CommandHandler context :> IRequestHandler<ISelfServiceRegisterCommand, Unit>
