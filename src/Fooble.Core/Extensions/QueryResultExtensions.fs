namespace Fooble.Core

open Fooble.Common
open Fooble.Presentation
open System.Runtime.CompilerServices

/// Provides presentation-related extension methods for query results.
[<Extension>]
type QueryResultExtensions =

    (* Extensions *)

    /// <summary>
    /// Constructs a message display read model from an existing member detail query result.
    /// </summary>
    /// <param name="result">The member detail query result to extend.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberDetailQueryResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.makeWarning "Member" "Detail" 404
                "No matching member could be found."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a message display read model from an existing member email query result.
    /// </summary>
    /// <param name="result">The member email query result to extend.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberEmailQueryResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.makeWarning "Member" "Change Email" 404
                "No matching member could be found."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a message display read model from an existing member exists query result.
    /// </summary>
    /// <param name="result">The member exists query result to extend.</param>
    /// <param name="subHeading">The sub-heading to use.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberExistsQueryResult, subHeading) =
        ensureWith (validateRequired result "result" "Result")
        ensureWith (validateRequired subHeading "subHeading" "Sub-heading")
        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.makeWarning "Member" subHeading 404
                "No matching member could be found."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a message display read model from an existing member list query result.
    /// </summary>
    /// <param name="result">The member list query result to extend.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberListQueryResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.makeInformational "Member" "List" 200
                "No members have yet been added."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a message display read model from an existing member other query result.
    /// </summary>
    /// <param name="result">The member other query result to extend.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberOtherQueryResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.makeWarning "Member" "Change Other" 404
                "No matching member could be found."
        | _ -> invalidOp "Result was not unsuccessful"

    /// <summary>
    /// Constructs a message display read model from an existing member username query result.
    /// </summary>
    /// <param name="result">The member username query result to extend.</param>
    [<Extension>]
    static member MapMessageDisplayReadModel(result:IMemberUsernameQueryResult) =
        ensureWith (validateRequired result "result" "Result")
        match result with
        | x when x.IsNotFound ->
            MessageDisplayReadModel.makeWarning "Member" "Change Username" 404
                "No matching member could be found."
        | _ -> invalidOp "Result was not unsuccessful"
