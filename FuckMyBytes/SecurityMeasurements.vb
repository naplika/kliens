Imports System.IO
imports System.Runtime.InteropServices
imports kliens.SharedElements
imports System.Text

#Disable Warning BC42021
#Disable Warning BC42016

Namespace FuckMyBytes
    Public Module SecurityMeasurements
        Dim ReadOnly Utf8 As Encoding = Encoding.UTF8

        public Function GenUniquePass() as string
            dim machineId = ""
            dim diskUuid = ""
            dim userId as string = Environment.UserName
            dim cpuId = ""
            if runtimeinformation.IsOSPlatform(osPlatform.Windows) Then
                machineId = GetWindowsMachineId()
                diskUuid = SharedElements.RunCommand("wmic diskdrive get SerialNumber")
                cpuId = RunCommand("wmic cpu get ProcessorId")
            Elseif Runtimeinformation.IsOSPlatform(OSPlatform.Linux) Then
                machineId = RunBashCommand("cat /etc/machine-id")
                diskUuid = SharedElements.RunBashCommand("blkid -s UUID -o value $(df --output=source / | tail -1)")
                cpuId = RunBashCommand("cat /proc/cpuinfo | grep 'model name' | uniq")
            Elseif RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                machineId = RunBashCommand("system_profiler SPHardwareDataType | awk '/UUID/ { print $3; }'")
                diskUuid = RunBashCommand("diskutil info / | awk '/Volume UUID/ {print $3}'")
                cpuId = RunBashCommand("sysctl -n machdep.cpu.brand_string")
            End If
            dim mix as string = machineId + diskUuid + userId + cpuId
            mix = EncryptionProviders.S512X100(mix)
            return mix
        End Function

        public function FuckString(str as string, Optional pass as string = "Naplika") as String
            dim step1 as Byte() = Utf8.GetBytes(str)
            dim step2 as string = "I'm nothing like yall" + Convert.ToBase64String(step1)

            dim step3 as string = AES_Encrypt(step2, pass)

            dim step4 as string = GzipCompress(step3)

            dim step5 as string = DesEncrypt(step4, LengthController(Pass, 8))

            dim step6 as string = IdeaEncrypt(step5, LengthController(Pass, 128))

            dim step7 as string = GzipCompress(step6)

            dim step8 as string = TripleDesEncrypt(step7, pass)

            return step8
        End Function

        public function UnFuckString(str as string, Optional pass as string = "Naplika") as String
            dim step1 as string = TripleDesDecrypt(str, Pass)
            if step1 = "fail" Then
                return "fail"
            End If
            dim step2 as string = GzipDecompress(step1)

            dim step3 as string = IdeaDecrypt(step2, LengthController(Pass, 128))

            if step3 = "fail" Then
                return "fail"
            End If
            dim step4 as string = DesDecrypt(step3, LengthController(Pass, 8))

            if step4 = "fail" Then
                return "fail"
            End If
            dim step5 as string = GzipDecompress(step4)

            dim step6 as string = AES_Decrypt(step5, pass)

            if step6 = "fail" Then
                return "fail"
            End If
            dim step7 as String
            try
                step7 = step6.Remove(0, "I'm nothing like yall".Length)
            catch
                return "fail"
            end try

            dim step8 as Byte() = Convert.FromBase64String(step7)

            dim step9 as string = Utf8.GetString(step8)

            return step9
        End function

        public Function DecryptConfig() As String
            if File.Exists(Settingspath) Then
                dim str as string = io.File.ReadAllText(Settingspath)
                str = UnFuckString(str, program.Uniquepass)
                str = str.Replace(vbcrlf, "").Replace(vbLf, "").Replace(vbCr, "").Replace("  ", "")
                return str
            Else
                return "fail"
            End If
        End Function

        public function UpgradeConfig()
            dim config as string = io.File.ReadAllText(Settingspath)
            dim output as string = FuckString(config, program.Uniquepass)
            IO.File.WriteAllText(Settingspath, output)
            return true
        End function
    End Module
End Namespace