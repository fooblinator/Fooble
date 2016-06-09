namespace Fooble.Presentation

open Fooble.Common
open Fooble.Core
open System.Runtime.CompilerServices

/// Provides presentation-related extension methods for member exists query result.
[<Extension>]
[<RequireQualifiedAccess>]
module MemberExistsQueryResult =

    (* Extensions *)

    /// <summary>
    /// Constructs a message display read model from a member exists query result.
    /// </summary>
    /// <param name="result">The member exists query result to extend.</param>
    /// <param name="subHeading">The message display sub-heading.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplayReadModel.Make"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel (result:IMemberExistsQueryResult) subHeading =
        ensureWith (validateRequired result "result" "Result")
        ensureWith (validateRequired subHeading "subHeading" "Sub-heading")
        match result with
        | x when x.IsNotFound ->
              MessageDisplayReadModel.make "Member" subHeading 404 MessageDisplayReadModel.warningSeverity
                  "No matching member could be found."
        | _ -> invalidOp "Result was not unsuccessful"
