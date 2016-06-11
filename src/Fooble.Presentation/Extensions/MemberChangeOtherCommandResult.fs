namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices
open System.Web.Mvc

/// Provides presentation-related extension methods for member changeOther.
[<Extension>]
[<RequireQualifiedAccess>]
module MemberChangeOtherCommandResult =

    (* Extensions *)

    /// <summary>
    /// Constructs a message display read model from a member change other command result.
    /// </summary>
    /// <param name="result">The member change other command result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IMemberChangeOtherCommandResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
              MessageDisplayReadModel.make "Member" "Change Other" 404 MessageDisplayReadModel.warningSeverity
                  "No matching member could be found."
        | _ -> invalidOp "Result was not unsuccessful"
