imports System.Reflection
imports System.Runtime.InteropServices
imports System.resources
imports Microsoft.Win32
imports System.Diagnostics
Imports System.IO
Imports System.Net.Http
Imports System.Text.Json.Nodes
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json.Linq

#Disable Warning BC42016
# Disable Warning BC42021

Friend MustInherit Class SharedElements
    public shared ReadOnly Settingspath as string = GetStartupPath() + "settings.json"
    public shared ReadOnly Loginpath as string = GetStartupPath() + "authorization.json"

    Public shared Function GetStartupPath() As String
        dim path as string = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            path = path + "\"
        Else
            path = path + "/"
        End if
        Return path
    End Function

    public shared function GetSlashDirection() as String
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            return "\"
        Else
            return "/"
        End If
    End function

public shared function GetTranslation(query as string, lang as string) as String
    Dim originalCulture As System.Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture
    if lang isnot Nothing Then
        System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(lang)
    Else
        System.Threading.Thread.CurrentThread.CurrentUICulture = originalCulture
    End If

    Dim rm As New ResourceManager("kliens.i18n", Assembly.GetExecutingAssembly())
    Dim result As String
    Try
        result = rm.GetString(query)
        if result Is Nothing OrElse result.Length <= 0 Then
            Throw New Exception("Entry not found")
        End If
    Catch ex As Exception
        System.Threading.Thread.CurrentThread.CurrentUICulture = originalCulture
        Try
            result = rm.GetString(query)
            If result Is Nothing OrElse result.Length <= 0 Then
                result = query
            End If
        Catch ex2 As Exception
            result = query
        End Try
    End Try
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

        Dim process As New Diagnostics.Process() With {.StartInfo = processStartInfo}
        process.Start()

        Dim output As String = process.StandardOutput.ReadToEnd()
        process.WaitForExit()
        if output.Length = 0 Then
            output = "none"
        End If
        
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

        Dim process As New System.Diagnostics.Process() With {.StartInfo = processStartInfo}
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

    public shared Function CheckCacheTime(path as String, minutes as Integer) As Boolean
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

    public shared function GetCacheTime(path as string) as Integer
        if Not System.IO.File.Exists(path) Then
            Return - 1
        End If
        Dim lastWriteTime As DateTime = System.IO.File.GetLastWriteTime(path)
        Dim currentTime As DateTime = DateTime.Now
        Dim diff As TimeSpan = currentTime - lastWriteTime
        Return diff.TotalMinutes
    End function

    public shared function FuzzySearch(q as string, data as string) as Integer
        dim ratio = FuzzySharp.Fuzz.PartialRatio(q, data)
        return ratio
    End function

    Public shared Function CheckInternetConnection() As Boolean
        Try
            Using client = New HttpClient()
                Using stream = client.GetAsync("https://www.google.com").Result
                    if stream.IsSuccessStatusCode Then
                        Return True
                    Else
                        return False
                    End If
                End Using
            End Using
        Catch
            Return False
        End Try
    End Function

    public shared function Help()
        Console.WriteLine(GetTranslation("help.header", Lang))
        Console.WriteLine()
        Console.WriteLine("clear/cls : " + GetTranslation("help-clear", Lang))
        Console.WriteLine(GetTranslation("schools.cmd", Lang) + " : " + GetTranslation("help-schools", Lang))
        Console.WriteLine("help : " + GetTranslation("help-help", Lang))
        Console.WriteLine("insult : " + GetTranslation("help-insult", Lang))
        Console.WriteLine("config : " + GetTranslation("help-config", Lang))
        Console.WriteLine(GetTranslation("login.cmd", Lang) + " : " + GetTranslation("help-login", Lang))
        Console.WriteLine("logout : " + GetTranslation("help-logout", Lang))
        return 0
    End function

    Public shared Function UpdateChecker() as Boolean
        dim localversion as string = GenLocalVer()
        dim client = new HttpClient()
        client.DefaultRequestHeaders.Add("User-Agent", "Naplika/v1 #UpdateChecker")
        dim response as HttpResponseMessage = client.GetAsync("https://naplika.mnus.hu/api/v1/version").Result
        if response.IsSuccessStatusCode Then
            dim remoteversion as string = response.Content.ReadAsStringAsync().Result
            dim json As Newtonsoft.Json.Linq.JObject = Newtonsoft.Json.Linq.JObject.Parse(remoteversion)
            ' check if it's an empty json
            if json.Count = 0 Then
                return False
            Else
                if localversion < json("version").ToString() Then
                    Console.WriteLine(GetTranslation("update.avaiable", lang))
                    Console.Writeline(GetTranslation("update.current", Lang) + localversion)
                    Console.WriteLine(GetTranslation("update.new", Lang) + json("version").ToString())
                elseif localversion > json("version").ToString() Then
                    Console.WriteLine(GetTranslation("local.version.newer", Lang))
                    Console.Writeline(GetTranslation("update.current", Lang) + localversion)
                    Console.WriteLine(GetTranslation("update.new", Lang) + json("version").ToString())
                End If
                return true
            End If
        Else
            Console.WriteLine(GetTranslation("update.failed", Lang) + response.StatusCode.ToString())
            return false
        End If
    End Function

    private Shared Function GenLocalVer() As String
        dim version as string = Assembly.GetExecutingAssembly().GetName().Version.ToString()
        ' let's assume the version it returns is 1.0.0.0
        version = version.Replace(".0", "")
        version = "v" + version
        return version
    End Function

    public shared Function PrintConfigables()
        Console.WriteLine(GetTranslation("config.header", Lang))
        Console.WriteLine(GetTranslation("config.cmd", Lang))
        Console.WriteLine("language: " + GetSettings("language") + " # " + GetTranslation("config-language", Lang))
        Console.WriteLine("customuser: " + GetSettings("customuser") + " # " + GetTranslation("config-customuser", Lang))
        return 0
    End Function

    Public shared Function Base64UrlDecode(base64Url As String) As Byte()
        base64Url = base64Url.Replace("-", "+").Replace("_", "/")
        Select Case base64Url.Length Mod 4
            Case 2
                base64Url &= "=="
            Case 3
                base64Url &= "="
        End Select
        Return Convert.FromBase64String(base64Url)
    End Function

    Public Shared Function IsBase64String(s As String) As Boolean
        s = s.Trim()
        Return (s.Length Mod 4 = 0) AndAlso Regex.IsMatch(s, "^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None)
    End Function

    public shared function SaveLogin(response as String, signature as String, password as String, optional name as string = "guest",
                                     optional school as string = "Naplika")
        dim jsonobj = new JsonObject()
        dim json as JObject = JObject.Parse(response)
        dim token as string = json("access_token").ToString()
        jsonobj.Add("token", token)
        dim refresh as string = json("refresh_token").ToString()
        jsonobj.Add("refresh", refresh)
        ' response gives 1800, maybe its 30 minutes
        dim expiretime as DateTime = DateTime.Now.AddMinutes(30)
        dim unixtime as string = ToUnixTimestamp(expiretime).ToString()
        jsonobj.Add("expire", unixtime)
        jsonobj.Add("signature", FuckMyBytes.LengthController(signature, 10))
        dim jsonstring as string = jsonobj.ToString()
        jsonstring = FuckMyBytes.FuckString(jsonstring, program.Uniquepass)
        File.WriteAllText(loginpath, jsonstring)
        dim confjson as JObject = JObject.Parse(DecryptConf)
        confjson("user") = name
        confjson("school") = school
        ' if you ask it is for reauthenticating in case kreten refresh token expires
        confjson("password") = password
        confjson("customuser") = name
        dim updconf as string = confjson.ToString()
        updconf = FuckMyBytes.FuckString(updconf, program.Uniquepass)
        File.WriteAllText(Settingspath, updconf)
        updatedconfig = true
        return 0
    End function

    public shared function DeleteLogin()
        dim json as JObject = JObject.Parse(DecryptConf)
        json("user") = "guest"
        json("school") = "Naplika"
        json("customuser") = "guest"
        json("password") = "undefined"
        dim jstring as string = json.ToString()
        jstring = FuckMyBytes.FuckString(jstring, program.Uniquepass)
        File.WriteAllText(Settingspath, jstring)
        File.Delete(Loginpath)
        updatedconfig = True
        return 0
    End function

    Private Shared Function ToUnixTimestamp(dateTime As DateTime) As Long
        Dim dateTimeOffset = new DateTimeOffset(dateTime)
        Return dateTimeOffset.ToUnixTimeSeconds()
    End Function
    
    public shared Function ReadPassword() As String
        Dim password As New System.Text.StringBuilder()
        While True
            Dim info As ConsoleKeyInfo = Console.ReadKey(True)
            If info.Key = ConsoleKey.Enter Then
                Console.WriteLine()
                Exit While
            ElseIf info.Key = ConsoleKey.Backspace Then
                If password.Length > 0 Then
                    password.Remove(password.Length - 1, 1)
                    Console.Write(vbBack)
                    Console.Write(" ")
                    Console.Write(vbBack)
                End If
            Else
                password.Append(info.KeyChar)
                Console.Write("*")
            End If
        End While
        Return password.ToString()
    End Function
End Class