Imports System
Imports System.IO
Imports System.Text
imports newtonsoft.json.linq
imports kliens.SharedElements
imports kliens.FuckMyBytes
Imports System.Threading

Module Program
    public Uniquepass as string = SecurityMeasurements.GenUniquePass()
    public lang as String
    public decryptconf as string

    Sub Main(args As String())
        decryptconf = SecurityMeasurements.decryptconfig()
        FirstStartupCheck().Wait()
        lang = GetSettings("language")
        Console.TreatControlCAsInput = true
        Console.Clear()
        Console.WriteLine(GetTranslation("welcome", Lang))
        Commandmode()
    End Sub

    Private function FirstStartupCheck() as task
        dim task as task = task.Run(Sub()
            if File.Exists(Settingspath) Then
                dim jsonString as string
                if decryptconf = "fail" or decryptconf = nothing Then
                    Console.WriteLine(GetTranslation("unencryptedconfig", lang))
                    upgradeconfig()
                    Environment.Exit(0)
                Else
                    jsonString = decryptconf
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

    private function GetSettings(q as string) as String
        dim jsonstring as string
        if decryptconf = "fail" Then
            jsonstring = File.ReadAllText(Settingspath)
        Else
            jsonstring = decryptconf
        End If
        try
            dim jsonobject as jobject = JObject.Parse(jsonstring)
            return jsonobject(q).ToString()
        catch ex as Exception
            Console.WriteLine(GetTranslation("settingloadfailed", lang).Replace("%s", q))
            return false
        end try
    End function

    private sub Commandmode(optional byval lastcommand as string = Nothing)
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
                Console.ForegroundColor = ConsoleColor.Gray
                dim commandarray as string() = command.ToString().Split(" ")
                Commandparser.Parsecommand(commandarray).Wait()
                command.Clear()
                dim last as string
                for i as Integer = 0 to commandarray.Length -1
                    last += commandarray(i) + " "
                Next
                last = last.TrimEnd(" ")
                Commandmode(last)
            elseIf keyInfo.Key = ConsoleKey.C AndAlso keyInfo.Modifiers = ConsoleModifiers.Control Then
                Console.ForegroundColor = ConsoleColor.Gray
                Console.WriteLine()
                Environment.Exit(0)
            ElseIf keyInfo.Key = ConsoleKey.Backspace Then
                If command.Length > 0 Then
                    command.Remove(command.Length - 1, 1)
                    Console.Write(vbBack)
                    Console.Write(" ")
                    Console.Write(vbBack)
                Else
                    Console.Beep()
                End If
                Continue While
                elseif keyInfo.key = consolekey.UpArrow Then
                    if lastcommand = Nothing Then
                        Console.Beep()
                        Else
                            dim length as Integer = command.Length
                            for i as Integer = 0 to Length -1
                                Console.Write(vbBack)
                                Console.Write(" ")
                                Console.Write(vbBack)
                                next
                            command.Clear()
                            Console.Write(lastcommand)
                            command.Append(lastcommand)
                    End If
                    Continue While
            End If
            command.Append(keyInfo.KeyChar)
            Console.Write(keyInfo.KeyChar)
        End While
' ReSharper disable once FunctionNeverReturns
    End sub
End Module
