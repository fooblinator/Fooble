namespace Fooble.Core

open Fooble.Core.Persistence
open System
open System.Diagnostics

/// <summary>
/// Provides functionality used in the gathering and persisting of member details.
/// </summary>
[<RequireQualifiedAccess>]
module SelfServiceRegister =

    (* Active Patterns *)

    let internal (|IsSuccess|IsDuplicateId|) (result:ISelfServiceRegisterCommandResult) =
        if result.IsSuccess
            then Choice1Of2 ()
            else Choice2Of2 ()

    (* Command *)

    /// <summary>
    /// Provides functionality used in the persisting of member details.
    /// </summary>
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
            Validation.raiseIfInvalid <| Member.validateName name
            Command (id, name) :> ISelfServiceRegisterCommand

    (* Read Model *)

    /// <summary>
    /// Provides functionality used in the gathering of member details.
    /// </summary>
    [<RequireQualifiedAccess>]
    module ReadModel =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | ReadModel of string

            interface ISelfServiceRegisterReadModel with

                member this.Name
                    with get() =
                        match this with
                        | ReadModel x -> x
    
        /// <summary>
        /// Represents an empty self-service register read model.
        /// </summary>
        /// <returns>Returns an empty self-service register read model.</returns>
        [<CompiledName("Empty")>]
        let empty = ReadModel String.empty :> ISelfServiceRegisterReadModel
    
        /// <summary>
        /// Constructs a self-service register read model.
        /// </summary>
        /// <param name="name">The name of the potential member.</param>
        /// <remarks>
        /// Does not validate parameters. This allows for re-construction of the view model with previously-submitted,
        /// and potentially invalid form data. Need to manually validate and handle submitted form data.
        /// </remarks>
        /// <returns>Returns a self-service register read model.</returns>
        [<CompiledName("Make")>]
        let make name = ReadModel name :> ISelfServiceRegisterReadModel

    (* Command Result *)

    /// <summary>
    /// Provides functionality used in the persisting of member details.
    /// </summary>
    [<RequireQualifiedAccess>]
    module internal CommandResult =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | Success
            | DuplicateId

            interface ISelfServiceRegisterCommandResult with

                member this.IsSuccess
                    with get() =
                        match this with
                        | Success _ -> true
                        | DuplicateId -> false

                member this.IsDuplicateId
                    with get() =
                        match this with
                        | Success _ -> false
                        | DuplicateId -> true

        let internal success = Success :> ISelfServiceRegisterCommandResult
        let internal duplicateId = DuplicateId :> ISelfServiceRegisterCommandResult

    (* Command Handler *)

    /// <summary>
    /// Provides functionality used in the persisting of member details.
    /// </summary>
    [<RequireQualifiedAccess>]
    module internal CommandHandler =

        [<DefaultAugmentation(false)>]
        type private Implementation =
            | CommandHandler of IFoobleContext

            member private this.Context
                with get() =
                    match this with
                    | CommandHandler x -> x

            interface ISelfServiceRegisterCommandHandler with

                member this.Handle(command) =
                    Debug.Assert(notIsNull <| box command, "Command parameter was null")
                    Seq.tryFind (fun (x:MemberData) -> x.Id = command.Id) this.Context.MemberData
                    |> function
                        | Some _ -> CommandResult.duplicateId
                        | None ->
                            MemberData(Id = command.Id, Name = command.Name)
                            |> this.Context.MemberData.AddObject
                            // TODO: research what could go wrong here and handle appropriately
                            ignore <| this.Context.SaveChanges()
                            CommandResult.success

        let internal make context =
            Debug.Assert(notIsNull context, "Context parameter was null")
            CommandHandler context :> ISelfServiceRegisterCommandHandler
