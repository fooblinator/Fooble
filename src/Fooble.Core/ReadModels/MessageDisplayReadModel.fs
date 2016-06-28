namespace Fooble.Presentation

open Fooble.Common
open System

[<RequireQualifiedAccess>]
module internal MessageDisplayReadModel =

    [<DefaultAugmentation(false)>]
    type private MessageDisplaySeverityImpl =
        | Informational
        | Warning
        | Error

        interface IMessageDisplaySeverity with

            member this.IsInformational
                with get() =
                    match this with
                    | Informational -> true
                    | _ -> false

            member this.IsWarning
                with get() =
                    match this with
                    | Warning -> true
                    | _ -> false

            member this.IsError
                with get() =
                    match this with
                    | Error -> true
                    | _ -> false

    let informationalSeverity = Informational :> IMessageDisplaySeverity
    let warningSeverity = Warning :> IMessageDisplaySeverity
    let errorSeverity = Error :> IMessageDisplaySeverity

    [<DefaultAugmentation(false)>]
    [<NoComparison>]
    type private MessageDisplayReadModelImpl =
        | ReadModel of heading:string * subHeading:string * statusCode:int * severity:IMessageDisplaySeverity *
              message:string

        interface IMessageDisplayReadModel with

            member this.Heading
                with get() =
                    match this with
                    | ReadModel(heading = x) -> x

            member this.SubHeading
                with get() =
                    match this with
                    | ReadModel(subHeading = x) -> x

            member this.StatusCode
                with get() =
                    match this with
                    | ReadModel(statusCode = x) -> x

            member this.Severity
                with get() =
                    match this with
                    | ReadModel(severity = x) -> x

            member this.Message
                with get() =
                    match this with
                    | ReadModel(message = x) -> x

    let makeInformational heading subHeading statusCode message =
#if DEBUG
        assertOn heading "heading" [ (String.IsNullOrEmpty >> not), "Heading is required" ]
        assertWith (validateRequired subHeading "subHeading" "Sub-heading")
        assertOn statusCode "statusCode" [ ((<=) 0), "Status code parameter is less than zero" ]
        assertOn message "message" [ (String.IsNullOrEmpty >> not), "Message is required" ]
#endif
        ReadModel(heading, subHeading, statusCode, informationalSeverity, message) :> IMessageDisplayReadModel

    let makeWarning heading subHeading statusCode message =
#if DEBUG
        assertOn heading "heading" [ (String.IsNullOrEmpty >> not), "Heading is required" ]
        assertWith (validateRequired subHeading "subHeading" "Sub-heading")
        assertOn statusCode "statusCode" [ ((<=) 0), "Status code parameter is less than zero" ]
        assertOn message "message" [ (String.IsNullOrEmpty >> not), "Message is required" ]
#endif
        ReadModel(heading, subHeading, statusCode, warningSeverity, message) :> IMessageDisplayReadModel

    let makeError heading subHeading statusCode message =
#if DEBUG
        assertOn heading "heading" [ (String.IsNullOrEmpty >> not), "Heading is required" ]
        assertWith (validateRequired subHeading "subHeading" "Sub-heading")
        assertOn statusCode "statusCode" [ ((<=) 0), "Status code parameter is less than zero" ]
        assertOn message "message" [ (String.IsNullOrEmpty >> not), "Message is required" ]
#endif
        ReadModel(heading, subHeading, statusCode, errorSeverity, message) :> IMessageDisplayReadModel
