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
        jsonobj.Add("firstStartup", "true")
        if selected = "1" Then
            jsonobj.Add("language", "hu")
            dim jsonstring as string = jsonobj.ToString()
            File.WriteAllText(Settingspath, jsonstring)
            Console.WriteLine(GetTranslation("pleaserestart", "hu"))
        elseif selected = "2" Then
            jsonobj.Add("language", "en")
            dim jsonstring as string = jsonobj.ToString()
            File.WriteAllText(Settingspath, jsonstring)
            Console.WriteLine(GetTranslation("pleaserestart", "en"))
        Else
            Console.Clear()
            Console.WriteLine("Invalid input. Please only input the number.")
            Console.WriteLine()
            Welcome()
        End If
    End sub
End Module