namespace Fooble.Core

open System
open System.Security.Cryptography

[<RequireQualifiedAccess>]
module internal Crypto =

    let private subkeyLength = 32
    let private saltSize = 16

    let private saltOffset = 1
    let private subkeyOffset = saltOffset + saltSize
    let private itersOffset = subkeyOffset + subkeyLength
    let private partsLength = itersOffset + sizeof<int>

    let hash password iterations =
        use crypto = new Rfc2898DeriveBytes(password, saltSize, iterations)
        let salt = crypto.Salt
        let bytes = crypto.GetBytes(subkeyLength)

        let iters =
            if BitConverter.IsLittleEndian
                then BitConverter.GetBytes(iterations)
                else BitConverter.GetBytes(iterations) |> Array.rev

        let parts = Array.zeroCreate<byte> partsLength
        parts.[0] <- byte 0 // version of this hashing algorithm
        Buffer.BlockCopy(salt, 0, parts, saltOffset, saltSize)
        Buffer.BlockCopy(bytes, 0, parts, subkeyOffset, subkeyLength)
        Buffer.BlockCopy(iters, 0, parts, itersOffset, sizeof<int>)

        Convert.ToBase64String(parts)

    let verify hashedPassword (password:string) =
        let parts = Convert.FromBase64String(hashedPassword)
        if parts.Length <> partsLength || parts.[0] <> byte 0 then
            false
        else
            let salt = Array.zeroCreate<byte> saltSize
            Buffer.BlockCopy(parts, saltOffset, salt, 0, saltSize)

            let bytes = Array.zeroCreate<byte> subkeyLength
            Buffer.BlockCopy(parts, subkeyOffset, bytes, 0, subkeyLength)

            let iters = Array.zeroCreate<byte> sizeof<int>
            Buffer.BlockCopy(parts, itersOffset, iters, 0, sizeof<int>)

            let iters = if BitConverter.IsLittleEndian then iters else iters |> Array.rev

            let iterations = BitConverter.ToInt32(iters, 0)

            use crypto = new Rfc2898DeriveBytes(password, salt, iterations)
            let challengeBytes = crypto.GetBytes(32)

            match Seq.compareWith (fun x y -> if x = y then 0 else 1) bytes challengeBytes with
            | x when x = 0 -> true
            | _ -> false

    let version hashedPassword = Convert.FromBase64String(hashedPassword).[0]
