namespace Fooble.Tests.Unit

open Fooble.Core
open Fooble.Tests
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module KeyGeneratorTests =

    [<Test>]
    let ``Calling make, returns key generator`` () =
        let keyGenerator = KeyGenerator.make ()

        test <@ box keyGenerator :? IKeyGenerator @>

    [<Test>]
    let ``Calling generate key, returns newly generated key`` () =
        let keyGenerator = KeyGenerator.make ()

        let key = keyGenerator.GenerateKey()

        test <@ Guid.notIsEmpty key @>
