Imports System
Imports System.IO
imports newtonsoft.json.linq
imports kliens.SharedElements
imports kliens.FuckMyBytes
Imports System.Threading
imports kliens.commandlineEssentials
#Disable Warning BC42016

Module Program
    public updatedconfig as boolean = False
    public readonly Uniquepass as string = SecurityMeasurements.GenUniquePass()
    public Lang as String
    public DecryptConf as string
    public DecryptAuth as string
    private _internetavail as Boolean = true
    
    Sub Main()
        DecryptConf = SecurityMeasurements.decryptconfig()
        DecryptAuth = SecurityMeasurements.DecryptAuth()
        FirstStartupCheck().Wait()
        lang = GetSettings("language")
        Console.TreatControlCAsInput = true
        Console.Clear()
        Console.WriteLine(GetTranslation("welcome", Lang))
        if SharedElements.CheckInternetConnection() = false Then
            _internetavail = False
            Console.WriteLine(GetTranslation("nointernet", Lang))
        End If
        if _internetavail = true Then
            dim updatecheck = updateChecker()
            if updateCheck = false Then
                dim fails = 0
                while updatecheck = false
                    Thread.Sleep(3000)
                    updatecheck = updateChecker()
                    fails += 1
                    if fails = 5 Then
                        Exit While
                    End If
                End While
            End If
        End If
        extends.ExMain.InitExtensions()
        Commandmode()
    End Sub

    Private function FirstStartupCheck() as task
        dim task as task = task.Run(Sub()
            if File.Exists(Settingspath) Then
                dim jsonString = ""
                if DecryptConf = "fail" or DecryptConf = nothing Then
                    Console.WriteLine(GetTranslation("unencryptedconfig", lang))
                    upgradeconfig()
                    Environment.Exit(0)
                Else
                    jsonString = DecryptConf
                End If
                dim jsonObject as JObject = JObject.Parse(jsonString)
                if not jsonObject.ContainsKey("firstStartup") Then
                    firstStartup.welcome().Wait()
                End If
            Else
                firstStartup.welcome().Wait()

            End If
        end sub)
        return task
    End function

    public function GetSettings(q as string) as String
        dim jsonstring as string
        if DecryptConf = "fail" Then
            jsonstring = File.ReadAllText(Settingspath)
        Else
            jsonstring = DecryptConf
        End If
        try
            dim jsonobject as jobject = JObject.Parse(jsonstring)
            return jsonobject(q).ToString()
        catch ex as Exception
            Console.WriteLine(GetTranslation("settingloadfailed", lang).Replace("%s", q))
            return False
        end try
    End function
End Module
