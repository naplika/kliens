Imports Org.BouncyCastle.Crypto.Digests
Imports Org.BouncyCastle.Utilities.Encoders
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports Org.BouncyCastle.Crypto
Imports Org.BouncyCastle.Crypto.Parameters
Imports Org.BouncyCastle.Security
Imports System.IO.Compression

#Disable Warning BC40000
#Disable Warning SYSLIB0021
#Disable Warning SYSLIB0022
#Disable Warning BC42021
#Disable Warning BC42016

Namespace FuckMyBytes
    Public Module EncryptionProviders
        private function Sha512(str as string) as String
            Dim digest As New Sha512Digest()
            Dim resBuf = New Byte(digest.GetDigestSize() - 1) {}
            Dim inputBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(str)

            digest.BlockUpdate(inputBytes, 0, inputBytes.Length)
            digest.DoFinal(resBuf, 0)

            Return Hex.ToHexString(resBuf)
        End function

        public function S512X100(str as string) as String
            dim i = 0
            while i < 100
                str = Sha512(str)
                i += 1
            End While
            return str
        End function

        Public Function AES_Encrypt(input As String, pass As String) As String

            Using aes As New RijndaelManaged
                Using hashAes As New MD5CryptoServiceProvider
                    Dim encrypted As String
                    Try
                        Dim hash(31) As Byte
                        Dim temp As Byte() = hashAes.ComputeHash(Encoding.ASCII.GetBytes(pass))
                        Array.Copy(temp, 0, hash, 0, 16)
                        Array.Copy(temp, 0, hash, 15, 16)
                        aes.Key = hash
                        aes.Mode = CipherMode.ECB
                        Dim desEncrypter As ICryptoTransform = aes.CreateEncryptor
                        Dim buffer As Byte() = Encoding.ASCII.GetBytes(input)
                        encrypted = Convert.ToBase64String(desEncrypter.TransformFinalBlock(buffer, 0, buffer.Length))
                        Return encrypted
                    Catch ex As Exception
                        Return ex.Message
                    End Try
                End Using
            End Using
        End Function

        Public Function AES_Decrypt(input As String, pass As String) As String
            Using aes As New RijndaelManaged
                Using hashAes As New MD5CryptoServiceProvider
                    Dim decrypted As String
                    Try
                        Dim hash(31) As Byte
                        Dim temp As Byte() = hashAes.ComputeHash(Encoding.ASCII.GetBytes(pass))
                        Array.Copy(temp, 0, hash, 0, 16)
                        Array.Copy(temp, 0, hash, 15, 16)
                        aes.Key = hash
                        aes.Mode = CipherMode.ECB
                        Dim desDecrypter As ICryptoTransform = aes.CreateDecryptor
                        Dim buffer As Byte() = Convert.FromBase64String(input)
                        decrypted = Encoding.ASCII.GetString(desDecrypter.TransformFinalBlock(buffer, 0, buffer.Length))
                        Return decrypted
                    Catch ex As Exception
                        Return "fail"
                    End Try
                End Using
            End Using
        End Function

        Public Function GzipCompress(input As String) As String
            Using outputStream As New MemoryStream()
                Using gZipStream As New GZipStream(outputStream, CompressionLevel.Optimal)
                    Using writer As New StreamWriter(gZipStream, Encoding.UTF8)
                        writer.Write(input)
                    End Using
                End Using
                Dim output As String = Convert.ToBase64String(outputStream.ToArray())
                Return output
            End Using
        End Function

        Public Function GzipDecompress(input As String) As String
            Dim compressedBytes As Byte() = Convert.FromBase64String(input)
            Using inputStream As New MemoryStream(compressedBytes)
                Using gZipStream As New GZipStream(inputStream, CompressionMode.Decompress)
                    Using reader As New StreamReader(gZipStream, Encoding.UTF8)
                        Dim output As String = reader.ReadToEnd()
                        Return output
                    End Using
                End Using
            End Using
        End Function

        Public Function DesEncrypt(plainText As String, password As String) As String
            Dim keyBytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim ivBytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim output As String
            Try
                Using desAlg = New DESCryptoServiceProvider()
                    desAlg.Key = keyBytes
                    desAlg.IV = ivBytes
                    Using encryptor As ICryptoTransform = desAlg.CreateEncryptor(keyBytes, ivBytes)
                        Using msEncrypt As New MemoryStream()
                            Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                                Using swEncrypt As New StreamWriter(csEncrypt)
                                    swEncrypt.Write(plainText)
                                End Using
                            End Using
                            output = Convert.ToBase64String(msEncrypt.ToArray())
                        End Using
                    End Using
                End Using
                Return output
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Public Function DesDecrypt(encryptedText As String, password As String) As String
            Dim keyBytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim ivBytes As Byte() = Encoding.UTF8.GetBytes(password)
            Dim encryptedBytes As Byte() = Nothing
            Dim errors As Boolean
            Try
                encryptedBytes = Convert.FromBase64String(encryptedText)
            Catch ex As Exception
                errors = True
            End Try
            Dim output As String
            If errors = False Then
                Using desAlg = New DESCryptoServiceProvider()
                    desAlg.Key = keyBytes
                    desAlg.IV = ivBytes
                    Using decryptor As ICryptoTransform = desAlg.CreateDecryptor(keyBytes, ivBytes)
                        Using msDecrypt As New MemoryStream(encryptedBytes)
                            Using csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)
                                Using srDecrypt As New StreamReader(csDecrypt)
                                    Try
                                        output = srDecrypt.ReadToEnd()
                                    Catch ex As Exception
                                        output = "fail"
                                    End Try
                                End Using
                            End Using
                        End Using
                    End Using
                End Using
            Else
                output = "fail"
            End If
            Return output
        End Function

        Public Function LengthController(hashValue As String, length As Integer) As String
            Dim shortenedHash As String
            If hashValue.Length < length Then
                Dim charactersToAdd As Integer = length - hashValue.Length
                shortenedHash = hashValue & New String("X"c, charactersToAdd)
            Else
                shortenedHash = hashValue.Substring(0, length)
            End If
            Return shortenedHash
        End Function

        Public Function IdeaEncrypt(data As String, password As String)
            Dim key As Byte() = Encoding.UTF8.GetBytes(password)
            If key.Length < 16 Then
            End If
            Dim cipher As IBufferedCipher = CipherUtilities.GetCipher("IDEA/ECB/PKCS7Padding")
            cipher.Init(True, New KeyParameter(key))
            Dim dataToEncrypt As Byte() = Encoding.UTF8.GetBytes(data)
            Dim encryptedData As Byte() = cipher.DoFinal(dataToEncrypt)
            Dim encryptedText As String = Convert.ToBase64String(encryptedData)
            Return encryptedText
        End Function

        Public Function IdeaDecrypt(encryptedText As String, password As String)
            Dim key As Byte() = Encoding.UTF8.GetBytes(password)
            Dim errors As Boolean
            If key.Length < 16 Then
            End If
            Dim cipher As IBufferedCipher = CipherUtilities.GetCipher("IDEA/ECB/PKCS7Padding")
            cipher.Init(False, New KeyParameter(key))
            Dim encryptedData As Byte() = Convert.FromBase64String(encryptedText)
            Dim decryptedData As Byte() = Nothing
            Try
                decryptedData = cipher.DoFinal(encryptedData)
            Catch ex As Exception
                errors = True
            End Try
            If errors = False Then
                Dim decryptedText As String = Encoding.UTF8.GetString(decryptedData)
                Return decryptedText
            Else
                Const decryptedtext = "fail"
                Return decryptedtext
            End If
        End Function

        Public Function TripleDesEncrypt(data As String, password As String) As String
            Dim tripleDesAlg As New TripleDESCryptoServiceProvider()
            Dim key As Byte() = TdesGen(password)
            Dim iv = New Byte() {0, 0, 0, 0, 0, 0, 0, 0}
            tripleDesAlg.Key = key
            tripleDesAlg.IV = iv
            Dim encryptor As ICryptoTransform = tripleDesAlg.CreateEncryptor()
            Dim dataBytes As Byte() = Encoding.UTF8.GetBytes(data)
            Dim encryptedData As Byte() = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length)
            Dim output As String = Convert.ToBase64String(encryptedData)
            Return output
        End Function

        Public Function TripleDesDecrypt(encryptedData As String, password As String) As String
            Dim tripleDesAlg As New TripleDESCryptoServiceProvider()
            Dim errors As Boolean
            Dim key As Byte() = TdesGen(password)
            Dim iv = New Byte() {0, 0, 0, 0, 0, 0, 0, 0}
            tripleDesAlg.Key = key
            tripleDesAlg.IV = iv
            Dim decryptor As ICryptoTransform = tripleDesAlg.CreateDecryptor()
            Dim encryptedBytes As Byte()
            try
                encryptedBytes = Convert.FromBase64String(encryptedData)
            Catch
                return "fail"
            end try
            Dim decryptedBytes As Byte() = Nothing
            Try
                decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length)
            Catch ex As Exception
                errors = True
            End Try
            If errors = False Then
                Dim output As String = Encoding.UTF8.GetString(decryptedBytes)
                Return output
            Else
                Const output = "fail"
                Return output
            End If
        End Function

        Private Function TdesGen(password As String) As Byte()
            Dim sha1 As New SHA1CryptoServiceProvider()
            Dim keyBytes As Byte() = sha1.ComputeHash(Encoding.UTF8.GetBytes(password))
            Array.Resize(keyBytes, 24)
            Return keyBytes
        End Function
    End Module
End Namespace