imports System.io
Imports System.Text.Json.Nodes
imports kliens.SharedElements
Public Module FirstStartup
    friend sub Welcome()
        Console.Clear()
        Console.WriteLine("Welcome to Naplika!")
        Console.WriteLine("Please select a language:")
        Console.WriteLine("1. Hungarian")
        Console.WriteLine("2. English")
        dim selected as string = console.ReadLine()
        dim jsonobj as JsonObject = new JsonObject()
        jsonobj.Add("firstStartup", "true")
        if selected = "1" Then
            jsonobj.Add("language", "hu")
            dim jsonstring as string = jsonobj.ToString()
            File.WriteAllText(Settingspath, jsonstring)
        elseif selected = "2" Then
            jsonobj.Add("language", "en")
            dim jsonstring as string = jsonobj.ToString()
            File.WriteAllText(Settingspath, jsonstring)
        Else
            Console.Clear()
            Console.WriteLine("Invalid input. Please only input the number.")
            Welcome()
        End If
    End sub
End Module