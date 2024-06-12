Imports System
Imports System.IO
imports newtonsoft.json.linq
imports kliens.SharedElements
imports kliens.FuckMyBytes
Imports System.Threading
imports Mindmagma.Curses

#Disable Warning BC42016

Module Program
    public Updatedconfig as boolean = False
    public readonly Uniquepass as string = SecurityMeasurements.GenUniquePass()
    public Lang as String
    public DecryptConf as string
    public DecryptAuth as string
    private _internetavail as Boolean = true
    public Screen = NCurses.InitScreen()
    
    Sub Main()
        NCurses.NoDelay(Screen, True)
        NCurses.NoEcho()
        NCurses.Refresh()
        DecryptConf = SecurityMeasurements.decryptconfig()
        DecryptAuth = SecurityMeasurements.DecryptAuth()
        FirstStartupCheck().Wait()
        lang = GetSettings("language")
        DataResolver.GetUserAgent().Wait()
        Console.TreatControlCAsInput = true
        Console.Clear()
        Console.WriteLine(GetTranslation("welcome", Lang))
        if SharedElements.CheckInternetConnection() = false Then
            _internetavail = False
            Console.WriteLine(GetTranslation("no.internet", Lang))
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
            DataResolver.Refresh().Wait()
        End If
        extends.ExMain.InitExtensions()
        CommandLineEssentials.Base.CommandMode()
    End Sub

    Private function FirstStartupCheck() as task
        dim task as task = task.Run(Sub()
            if File.Exists(Settingspath) Then
                dim jsonString = ""
                if DecryptConf = "fail" or DecryptConf = nothing Then
                    Console.WriteLine(GetTranslation("unencrypted.config", lang))
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
            dim regen as string
            if TryToRegen(q, regen) = True Then
                return regen
            Else
                Console.WriteLine(GetTranslation("setting.load.failed", lang).Replace("%s", q))
                return False
            End If
        end try
    End function
    
    public Function TryToRegen(q as String, byref vari as String) As Boolean
        dim confjson as JObject = JObject.Parse(DecryptConf)
        dim value as String
        if q = "language" Then
            value = "en"
        elseif q = "customuser" Then
            value = "guest"
        elseif q = "user" Then
            value = "guest"
        ElseIf q = "school" Then
            value = "Naplika"
        elseif q = "firstStartup" Then
            value = "false"
        elseif q = "configrev" Then
            value = "1"
        elseif q = "password" Then
            value = "undefined"
        End If
        confjson(q) = value
        dim updconf as string = confjson.ToString()
        updconf = FuckMyBytes.FuckString(updconf, program.Uniquepass)
        File.WriteAllText(Settingspath, updconf)
        vari = value
    End Function
End Module
