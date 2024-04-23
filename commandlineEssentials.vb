imports kliens.FuckMyBytes
Imports System.Text

Public Class commandlineEssentials
    Public shared sub Commandmode(optional byval lastcommand as string = Nothing)
        ' shitty config updater, but it will do
        if updatedconfig = true Then
            DecryptConf = SecurityMeasurements.decryptconfig()
            DecryptAuth = SecurityMeasurements.DecryptAuth()
            updatedconfig = False
        End If
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
                dim last = ""
                for i = 0 to commandarray.Length - 1
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
                    for i = 0 to Length - 1
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
End Class