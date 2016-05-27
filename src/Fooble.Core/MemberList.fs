namespace Fooble.Core

open Fooble.Common
open Fooble.Presentation
open System.Runtime.CompilerServices

/// Provides functionality used in the querying and presentation of member lists.
[<RequireQualifiedAccess>]
[<Extension>]
module MemberList =

    /// <summary>
    /// Constructs a member list query.
    /// </summary>
    /// <returns>Returns a member list query.</returns>
    [<CompiledName("MakeQuery")>]
    let makeQuery () = MemberListQuery.make ()

    /// <summary>
    /// Constructs a message display read model from a member list query result.
    /// </summary>
    /// <param name="result">The member list query result to extend.</param>
    /// <returns>Returns a message display read model.</returns>
    /// <remarks>This method should only be called on unsuccessful results. For displaying a "success" result, use
    /// <see cref="MessageDisplay.MakeReadModel"/> directly.</remarks>
    [<Extension>]
    [<CompiledName("ToMessageDisplayReadModel")>]
    let toMessageDisplayReadModel result =

        [ (isNotNull << box), "Result parameter was null" ]
        |> ValidationResult.get result "result"
        |> ValidationResult.enforce

        match result with
        | MemberListQuery.IsSuccess _ -> invalidOp "Result was not unsuccessful"
        | MemberListQuery.IsNotFound ->
            MessageDisplay.makeReadModel "Member" "List" 200 MessageDisplay.informationalSeverity
                "No members have yet been added."
