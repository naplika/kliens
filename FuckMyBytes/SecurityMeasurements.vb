Imports System.IO
imports System.Runtime.InteropServices
imports kliens.SharedElements
imports System.Text

Namespace FuckMyBytes
    Public Module SecurityMeasurements
        Dim ReadOnly _utf8 As Encoding = Encoding.UTF8
        public Function GenUniquePass() as string
            dim machineId as string = ""
            dim diskUuid as String = ""
            dim userId as string = Environment.UserName
            dim cpuId as string = ""
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

        public function FuckString(str as string) as String
            dim step1 as Byte() = _utf8.GetBytes(str)
            dim step2 as string = "I'm nothing like yall" + Convert.ToBase64String(step1)
            step1 = Nothing
            dim step3 as string = AES_Encrypt(step2, program.Uniquepass)
            step2 = nothing
            dim step4 as string = GzipCompress(step3)
            step3 = nothing
            dim step5 as string = DesEncrypt(step4, LengthController(Program.Uniquepass, 8))
            step4 = Nothing
            dim step6 as string = IdeaEncrypt(step5, LengthController(Program.Uniquepass, 128))
            step5 = Nothing
            dim step7 as string = GzipCompress(step6)
            step6 = Nothing
            dim step8 as string = TripleDesEncrypt(step7, Program.Uniquepass)
            step7 = Nothing
            return step8
        End Function

        public function UnFuckString(str as string) as String
            dim step1 as string = TripleDesDecrypt(str, Program.Uniquepass)
            if step1 = "fail" Then
                return "fail"
            End If
            dim step2 as string = GzipDecompress(step1)
            step1 = Nothing
            dim step3 as string = IdeaDecrypt(step2, LengthController(Program.Uniquepass, 128))
            step2 = Nothing
            if step3 = "fail" Then
                return "fail"
            End If
            dim step4 as string = DesDecrypt(step3, LengthController(Program.Uniquepass, 8))
            step3 = Nothing
            if step4 = "fail" Then
                return "fail"
            End If
            dim step5 as string = GzipDecompress(step4)
            step4 = Nothing
            dim step6 as string = AES_Decrypt(step5, program.Uniquepass)
            step5 = Nothing
            if step6 = "fail" Then
                return "fail"
            End If
            dim step7 as string = step6.Remove(0, "I'm nothing like yall".Length)
            step6 = Nothing
            dim step8 as Byte() = Convert.FromBase64String(step7)
            step7 = Nothing
            dim step9 as string = _utf8.GetString(step8)
            step8 = Nothing
            return step9
        End function

        public Function decryptconfig() As String
            if File.Exists(Settingspath) Then
                dim str as string = io.File.ReadAllText(Settingspath)
                str = UnFuckString(str)
                str = str.Replace(vbcrlf, "").Replace(vbLf, "").Replace(vbCr, "").Replace("  ", "")
                return str
            Else
                return "fail"
            End If
        End Function
    End Module
End Namespace