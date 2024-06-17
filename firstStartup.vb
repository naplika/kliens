imports System.io
Imports System.Text.Json.Nodes
imports kliens.SharedElements

Public Module FirstStartup
    friend function Welcome() as Task
        dim task as task = task.run(Sub() 
            Console.Write("Welcome to Naplika!" + vbCrLf)
            Console.Write("Please select a language:" + vbCrLf)
            Console.Write("1. Hungarian (Magyar)" + vbCrLf)
            Console.Write("2. English" + vbCrLf)
            Console.Write("3. German (Deutsch)" + vbCrLf)
            Console.Write("Can't find your language? Check https://github.com/naplika/kliens" + vbCrLf)
            Console.Write("(1): ")
            dim selected as string = console.Readline()
            dim jsonobj = new JsonObject()
            jsonobj.Add("configrev", "1")
            jsonobj.Add("firstStartup", "true")
            jsonobj.Add("user", "guest")
            jsonobj.Add("customuser", "guest")
            jsonobj.Add("school", "Naplika")
            dim okinput = false
            dim templang = "en"
            if selected = "1" Then
                jsonobj.Add("language", "hu")
                templang = "hu"
                okinput = true
            elseif selected = "2" Then
                jsonobj.Add("language", "en")
                templang = "en"
                okinput = true
            elseif selected = "3" Then
                jsonobj.Add("language", "de")
                templang = "de"
                okinput = true
            Else
                Console.Clear()
                Console.WriteLine("Invalid input. Please only input the number.")
                Console.WriteLine()
                Welcome()
            End If
            if okinput = true Then
                dim jsonstring as string = jsonobj.ToString()
                dim output as STRING = FuckMyBytes.FuckString(jsonstring, Program.Uniquepass)
                File.WriteAllText(Settingspath, output)
                Console.WriteLine(GetTranslation("changes.saved", templang))
            End If
            Environment.Exit(0)
        end sub)
        return task
    End function
End Module