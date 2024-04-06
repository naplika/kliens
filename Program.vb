Imports System
Imports System.IO
Imports System.Text
imports newtonsoft.json.linq
imports kliens.SharedElements
Module Program
    dim ReadOnly Lang as string = GetSettings("language")
    Sub Main(args As String())
        Console.TreatControlCAsInput = true
        Console.Clear()
        firstStartupCheck()
        Console.WriteLine(GetTranslation("welcome", Lang))
        Commandmode()
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

    private sub Commandmode()
        dim username as string = GetSettings("user")
        dim school as string = GetSettings("school")
        Console.ForegroundColor = consolecolor.Gray
        Console.Write("{0}@{1}> ", username, school)
        dim command as new StringBuilder
        console.ForegroundColor = ConsoleColor.Yellow
        While True
            Dim keyInfo As ConsoleKeyInfo = Console.ReadKey(True)
            If keyInfo.Key = ConsoleKey.Enter Then
                console.WriteLine()
                dim commandarray as string() = command.ToString().Split(" ")
                Commandparser.Parsecommand(commandarray)
                command.Clear()
                Commandmode()
            elseIf keyInfo.Key = ConsoleKey.C AndAlso keyInfo.Modifiers = ConsoleModifiers.Control Then
                Console.ForegroundColor = ConsoleColor.Gray
                Environment.Exit(0)
            ElseIf keyInfo.Key = ConsoleKey.Backspace Then
                If command.Length > 0 Then
                    command.Remove(command.Length - 1, 1)
                    Console.Write(vbBack)
                    Console.Write(" ")
                    Console.Write(vbBack)
                End If
                Continue While
            End If
            command.Append(keyInfo.KeyChar)
            Console.Write(keyInfo.KeyChar)
        End While
    End sub
End Module
