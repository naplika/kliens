Namespace CommandLineEssentials
Public Class Base
    public shared sub CommandMode(optional byval lastcommand as string = Nothing)
        SharedElements.UpdateConfig()
        Console.Write(SharedElements.ConsoleSet.Whoami)
        dim KeyInfo as ConsoleKeyInfo = console.ReadKey(True)
        while True
            if KeyInfo.Key = Consolekey.Enter Then
                ButtonParser.EnterKey()
                Continue While
            elseif KeyInfo.Key = consolekey.c AndAlso KeyInfo.Modifiers = ConsoleModifiers.Control Then
                ButtonParser.CtrlC()
                elseif KeyInfo.Key = consolekey.Backspace Then
                    ButtonParser.Backspace()
                    Continue While
                    elseif KeyInfo.Key = ConsoleKey.UpArrow Then
                        ButtonParser.UpArrow()
                        Continue While
            End If
            Command.Append(KeyInfo.KeyChar)
            console.Write(KeyInfo.KeyChar)
            SharedElements.ConsoleSet.CursorPos +=1
            Console.SetCursorPosition(SharedElements.ConsoleSet.Whoami.Length + SharedElements.ConsoleSet.CursorPos, console.CursorTop)
        End While
        
        ' ReSharper disable once FunctionNeverReturns
    End sub
End Class
End Namespace