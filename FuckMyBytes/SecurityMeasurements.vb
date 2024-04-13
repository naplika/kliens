imports System.Runtime.InteropServices
imports kliens.SharedElements

Namespace FuckMyBytes
    Public Module SecurityMeasurements
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
            if Program.Debugflag = true Then
' ReSharper disable once LocalizableElement
                Console.WriteLine("==DEBUG UNIQUECRAP==")
                Console.Writeline("hwid " + machineId)
                Console.WriteLine("did " + diskUuid)
                Console.WriteLine("usr " + userId)
                Console.WriteLine("cid " + cpuId)
                Console.WriteLine("uniqueid " + mix)
' ReSharper disable once LocalizableElement
                Console.WriteLine("==END DEBUG==")
            End If
            return mix
        End Function
    End Module
End Namespace