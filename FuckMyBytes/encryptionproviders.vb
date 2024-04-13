Imports Org.BouncyCastle.Crypto.Digests
Imports Org.BouncyCastle.Utilities.Encoders
Namespace FuckMyBytes
Public Module EncryptionProviders
    private function Sha512(str as string) as String
        Dim digest As New Sha512Digest()
        Dim resBuf As Byte() = New Byte(digest.GetDigestSize() - 1) {}
        Dim inputBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(str)

        digest.BlockUpdate(inputBytes, 0, inputBytes.Length)
        digest.DoFinal(resBuf, 0)

        Return Hex.ToHexString(resBuf)
    End function
    public function S512X100(str as string) as String
        dim i as Integer = 0
        while i < 100
            str = Sha512(str)
            i += 1
        End While
        return str
    End function
End Module
End Namespace