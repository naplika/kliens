imports System.Runtime.InteropServices
imports kliens.SharedElements
Namespace FuckMyBytes
Public Module SecurityMeasurements
    public Function GenUniquePass() as string
        dim machineId as string
        dim DiskUUID as String
        if runtimeinformation.IsOSPlatform(osPlatform.Windows) Then
               machineId = GetWindowsMachineId()
            DiskUUID = SharedElements.RunCommand("wmic diskdrive get SerialNumber")
        Elseif Runtimeinformation.IsOSPlatform(OSPlatform.Linux) Then
            machineId = RunBashCommand("cat /etc/machine-id")
            DiskUUID = SharedElements.RunBashCommand("blkid -s UUID -o value $(df --output=source / | tail -1)")
        Elseif RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            machineId = RunBashCommand("system_profiler SPHardwareDataType | awk '/UUID/ { print $3; }'")
            DiskUUID = RunBashCommand("diskutil info / | awk '/Volume UUID/ {print $3}'")
        End If
        Console.WriteLine("==DEBUG UNIQUECRAP==")
        Console.Writeline("hwid " + machineId)
        Console.WriteLine("did " + DiskUUID)
        Console.WriteLine("==END DEBUG==")
        return true
    End Function
End Module
End Namespace