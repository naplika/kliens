imports System.Reflection
imports System.Runtime.InteropServices
imports System.resources
imports Microsoft.Win32
imports System.Diagnostics

Friend MustInherit Class SharedElements
    public shared ReadOnly Settingspath as string = GetStartupPath() + "settings.json"

    Public shared Function GetStartupPath() As String
        dim path as string = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            path = path + "\"
        Else
            path = path + "/"
        End if
        Return path
    End Function

    public shared function GetTranslation(query as string, lang as string) as String
        Dim originalCulture As System.Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture
        if lang isnot Nothing Then
            System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(lang)
        Else
            System.Threading.Thread.CurrentThread.CurrentUICulture = originalCulture
        End If
        
        Dim rm As New ResourceManager("kliens.i18n", Assembly.GetExecutingAssembly())
        Dim result As String = rm.GetString(query)
        if result.Length <= 0 Then
            System.Threading.Thread.CurrentThread.CurrentUICulture = originalCulture
            result = rm.GetString(query)
        End If
        return result
    End function

    Public shared Function RunBashCommand(command As String) As String
        Dim processStartInfo As New ProcessStartInfo() With {
                .FileName = "/bin/bash",
                .Arguments = "-c """ + command + """",
                .RedirectStandardOutput = True,
                .UseShellExecute = False,
                .CreateNoWindow = True
                }

        Dim process As New Process() With {
                .StartInfo = processStartInfo
                }
        process.Start()

        Dim output As String = process.StandardOutput.ReadToEnd()
        process.WaitForExit()

        Return output.Trim()
    End Function

    public shared function RunCommand(command as String) as String
        Dim processStartInfo As New ProcessStartInfo() With {
                .FileName = "cmd.exe",
                .Arguments = "/C " + command,
                .RedirectStandardOutput = True,
                .UseShellExecute = False,
                .CreateNoWindow = True
                }

        Dim process As New Process() With {
                .StartInfo = processStartInfo
                }
        process.Start()

        Dim output As String = process.StandardOutput.ReadToEnd()
        process.WaitForExit()

        Return output.Trim()
    End function

    Public shared Function GetWindowsMachineId() As String
        If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            Using key = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Cryptography")
                If key IsNot Nothing Then
                    Return key.GetValue("MachineGuid").ToString()
                Else
                    Throw New Exception("none")
                End If
            End Using
        Else
            Throw New PlatformNotSupportedException("This function is only supported on Windows.")
        End If
    End Function

    public shared Function checkCacheTime(path as String, minutes as Integer) As Boolean
        if Not System.IO.File.Exists(path) Then
            Return False
        End If
        Dim lastWriteTime As DateTime = System.IO.File.GetLastWriteTime(path)
        Dim currentTime As DateTime = DateTime.Now
        Dim diff As TimeSpan = currentTime - lastWriteTime
        if diff.TotalMinutes > minutes andalso not diff.TotalMinutes < 0 then
            Return False
        End If
        Return True
    End Function

    public shared function getCacheTime(path as string) as Integer
        if Not System.IO.File.Exists(path) Then
            Return - 1
        End If
        Dim lastWriteTime As DateTime = System.IO.File.GetLastWriteTime(path)
        Dim currentTime As DateTime = DateTime.Now
        Dim diff As TimeSpan = currentTime - lastWriteTime
        Return diff.TotalMinutes
    End function
End Class