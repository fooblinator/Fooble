namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices

/// Provides presentation-related extension methods for member list.
[<Extension>]
[<RequireQualifiedAccess>]
module MemberListQueryResult =

    (* Extensions *)

    /// <summary>
    /// Constructs a message display read model from a member list query result.
    /// </summary>
    /// <param name="result">The member list query result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IMemberListQueryResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
              MessageDisplayReadModel.make "Member" "List" 200 MessageDisplayReadModel.informationalSeverity
                  "No members have yet been added."
        | _ -> invalidOp "Result was not unsuccessful"
