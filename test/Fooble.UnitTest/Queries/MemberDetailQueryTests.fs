namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open Fooble.Persistence
open Fooble.Presentation
open MediatR
open Moq
open Moq.FSharp.Extensions
open NUnit.Framework
open Swensen.Unquote
open System

[<TestFixture>]
module MemberDetailQueryTests =

    type private Result =
        | Success of Guid * string * string * string * string * DateTime * DateTime
        | NotFound of Guid

    let private setupForHandlerTest result =
        let (id, memberData) =
            match result with
            | Success(id, username, email, nickname, avatarData, registeredOn, passwordChangedOn) ->
                  randomPassword 32
                  |> fun x -> Crypto.hash x 100
                  |> fun x ->
                         makeMemberData (Some(id)) (Some(username)) (Some(x)) (Some(email)) (Some(nickname))
                             (Some(avatarData)) (Some(registeredOn)) (Some(passwordChangedOn)) None
                  |> fun x -> (id, Some(x))
            | NotFound(id) -> (id, None)
        let contextMock = Mock<IFoobleContext>()
        contextMock.SetupFunc(fun x -> x.GetMember(id, false)).Returns(memberData).End
        let query = MemberDetailQuery.make id
        let handler = MemberDetailQuery.makeHandler contextMock.Object
        (handler, query, contextMock)

    [<Test>]
    let ``Calling make, with successful parameters, returns expected query`` () =
        let id = Guid.NewGuid()
        let query = MemberDetailQuery.make id
        box query :? IRequest<IMemberDetailQueryResult> =! true
        testMemberDetailQuery query id

    [<Test>]
    let ``Calling make success result, returns expected state`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let registered = DateTime.UtcNow
        let passwordChanged = DateTime.UtcNow
        let queryResult =
            MemberDetailReadModel.make id username email nickname avatarData registered passwordChanged
            |> MemberDetailQuery.makeSuccessResult
        queryResult.IsSuccess =! true
        queryResult.IsNotFound =! false
        testMemberDetailReadModel queryResult.ReadModel id username email nickname avatarData registered
            passwordChanged

    [<Test>]
    let ``Calling not found result, returns expected state`` () =
        let queryResult = MemberDetailQuery.notFoundResult
        queryResult.IsSuccess =! false
        queryResult.IsNotFound =! true
        testInvalidOperationException "Result was not successful" <@ queryResult.ReadModel @>

    [<Test>]
    let ``Calling extension to message display read model, as success result, raises expected exception`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let registered = DateTime.UtcNow
        let passwordChanged = DateTime.UtcNow
        let queryResult =
            MemberDetailReadModel.make id username email nickname avatarData registered passwordChanged
            |> MemberDetailQuery.makeSuccessResult
        testInvalidOperationException "Result was not unsuccessful" <@ queryResult.MapMessageDisplayReadModel() @>

    [<Test>]
    let ``Calling extension to message display read model, as not found result, returns expected read model`` () =
        let heading = "Member"
        let subHeading = "Detail"
        let statusCode = 404
        let severity = MessageDisplayReadModel.warningSeverity
        let message = "No matching member could be found."
        let readModel =
            MemberDetailQuery.notFoundResult
            |> fun x -> x.MapMessageDisplayReadModel()
        testMessageDisplayReadModel readModel heading subHeading statusCode severity message

    [<Test>]
    let ``Calling handler handle, with successful parameters, returns expected result`` () =
        let id = Guid.NewGuid()
        let username = randomString 32
        let email = randomEmail 32
        let nickname = randomString 64
        let avatarData = randomString 32
        let registeredOn = DateTime.UtcNow
        let passwordChangedOn = DateTime.UtcNow
        let (handler, query, contextMock) =
            setupForHandlerTest (Success(id, username, email, nickname, avatarData, registeredOn, passwordChangedOn))
        let queryResult = handler.Handle(query)
        contextMock.VerifyFunc((fun x -> x.GetMember(id, false)), Times.Once())
        queryResult.IsSuccess =! true
        queryResult.IsNotFound =! false
        let readModel = queryResult.ReadModel
        testMemberDetailReadModel readModel id username email nickname avatarData registeredOn passwordChangedOn

    [<Test>]
    let ``Calling handler handle, with id not found in data store, returns expected result`` () =
        let notFoundId = Guid.NewGuid()
        let (handler, query, contextMock) = setupForHandlerTest (NotFound(notFoundId))
        let queryResult = handler.Handle(query)
        contextMock.VerifyFunc((fun x -> x.GetMember(notFoundId, false)), Times.Once())
        queryResult.IsSuccess =! false
        queryResult.IsNotFound =! true
