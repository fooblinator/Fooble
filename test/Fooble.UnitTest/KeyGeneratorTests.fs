namespace Fooble.UnitTest

open Fooble.Common
open Fooble.Core
open NUnit.Framework
open Swensen.Unquote

[<TestFixture>]
module KeyGeneratorTests =

    [<Test>]
    let ``Calling generate key, returns newly generated key`` () =
        let keyGenerator = KeyGenerator.make ()

        let key = keyGenerator.GenerateKey()

        Guid.isEmpty key =! false
