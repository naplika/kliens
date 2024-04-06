Imports System
Imports System.IO
imports System.Reflection
imports System.Runtime.InteropServices
imports System.Text
imports newtonsoft.json.linq
Module Program
    Sub Main(args As String())
        firstStartupCheck()
    End Sub
    Private Function GetStartupPath() As String
        dim path as string = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            path = path + "\"
        Else
            path = path + "/"
        End if
        Return path
    End Function
    private function FirstStartupCheck()
        dim path as string = GetStartupPath() + "settings.json"
        if File.Exists(path) Then
            dim jsonString as string = file.ReadAllText(path)
            dim jsonObject as JObject = JObject.Parse(jsonString)
            if not jsonObject.ContainsKey("firstStartup") Then
                firstStartup.welcome()
            End If
        Else 
                firstStartup.welcome()
        End If
        return True
    End function
End Module
