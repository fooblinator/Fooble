namespace Fooble.Core

open Fooble.Common
open Fooble.Presentation
open System.Runtime.CompilerServices

/// Provides functionality used in the querying and presentation of member details.
[<RequireQualifiedAccess>]
[<Extension>]
module MemberDetail =

    /// <summary>
    /// Constructs a member detail query.
    /// </summary>
    /// <param name="id">The member id to search for.</param>
    /// <returns>Returns a member detail query.</returns>
    [<CompiledName("MakeQuery")>]
    let makeQuery id =
        ValidationResult.enforce (Member.validateId id)
        MemberDetailQuery.make id

    /// <summary>
    /// Constructs a message display read model from a member detail query result.
    /// </summary>
    /// <param name="result">The member detail query result to extend.</param>
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
        | MemberDetailQuery.IsSuccess _ -> invalidOp "Result was not unsuccessful"
        | MemberDetailQuery.IsNotFound ->
            MessageDisplay.makeReadModel "Member" "Detail" 404 MessageDisplay.warningSeverity
                "No matching member could be found."
