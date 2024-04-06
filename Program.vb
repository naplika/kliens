Imports System
Imports System.IO
imports newtonsoft.json.linq
imports kliens.SharedElements
Module Program
    dim ReadOnly Lang as string = GetSettings("language")
    Sub Main(args As String())
        Console.Clear()
        firstStartupCheck()
        Console.WriteLine(GetTranslation("welcome", Lang))
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

    private function GetSettings(q as string) as String
        dim jsonstring as string = file.ReadAllText(Settingspath)
        dim jsonobject as jobject = JObject.Parse(jsonstring)
        return jsonobject(q).ToString()
    End function
End Module
