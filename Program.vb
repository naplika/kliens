Imports System
imports System.Text
Imports System.IO
imports newtonsoft.json.linq
imports kliens.SharedElements
Module Program
    Sub Main(args As String())
        firstStartupCheck()
    End Sub

    Private function FirstStartupCheck()
        if File.Exists(Settingspath) Then
            dim jsonString as string = file.ReadAllText(Settingspath)
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
