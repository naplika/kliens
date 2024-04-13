imports System.io
Imports System.Text.Json.Nodes
imports kliens.SharedElements

Public Module FirstStartup
    friend sub Welcome()
        Console.WriteLine("Welcome to Naplika!")
        Console.WriteLine("Please select a language:")
        Console.WriteLine("1. Hungarian")
        Console.WriteLine("2. English")
        Console.WriteLine("Can't find your language? Check https://github.com/naplika/kliens")
        dim selected as string = console.ReadLine()
        dim jsonobj as JsonObject = new JsonObject()
        jsonobj.Add("configrev", "1")
        jsonobj.Add("firstStartup", "true")
        jsonobj.Add("user", "guest")
        jsonobj.Add("school", "Naplika")
        dim okinput as boolean = false
        dim templang as string
        if not IO.File.Exists(Settingspath) Then
            IO.File.Create(Settingspath).Dispose()
        End If
        if selected = "1" Then
            jsonobj.Add("language", "hu")
            templang = "hu"
            okinput = true
        elseif selected = "2" Then
            jsonobj.Add("language", "en")
            templang = "en"
            okinput = true
        Else
            Console.Clear()
            Console.WriteLine("Invalid input. Please only input the number.")
            Console.WriteLine()
            Welcome()
        End If
        if okinput = true Then
            dim jsonstring as string = jsonobj.ToString()
            jsonstring = FuckMyBytes.FuckString(jsonstring)
            File.WriteAllText(Settingspath, jsonstring)
            Console.WriteLine(GetTranslation("changessaved", templang))
        End If
        Environment.Exit(0)
    End sub
End Module