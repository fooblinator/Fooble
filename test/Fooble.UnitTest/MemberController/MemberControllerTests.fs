namespace Fooble.UnitTest

open Fooble.Web.Controllers
open Moq.FSharp.Extensions
open NUnit.Framework

[<TestFixture>]
module MemberControllerTests =

    [<Test>]
    let ``Constructing, with null mediator, raises expected exception`` () =
        let expectedParamName = "mediator"
        let expectedMessage = "Mediator is required"

        testArgumentException expectedParamName expectedMessage
            <@ new MemberController(null, (makeTestKeyGenerator None)) @>

    [<Test>]
    let ``Constructing, with null key generator, raises expected exception`` () =
        let expectedParamName = "keyGenerator"
        let expectedMessage = "Key generator is required"

        testArgumentException expectedParamName expectedMessage
            <@ new MemberController(mock (), null) @>

    [<Test>]
    let ``Constructing, with valid parameters, returns expected result`` () =
        let keyGenerator = makeTestKeyGenerator None
        ignore (new MemberController(mock (), keyGenerator))
