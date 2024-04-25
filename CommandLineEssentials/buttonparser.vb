Imports System.Runtime.InteropServices.JavaScript
imports kliens.CommandLineEssentials.SharedElements.ConsoleSet

Namespace CommandLineEssentials
    public Class ButtonParser
        public shared sub EnterKey
            Console.WriteLine()
            Console.ForegroundColor = DefaultColor
            dim commandArray as String() = Command.ToString().Split(" ")
            CommandParser.ParseCommand(commandArray).Wait()
            Command.Clear()
            CursorPos = 0
            for i = 0 to CommandArray.Length - 1
                LastCommand += CommandArray(i) + " "
            Next
            LastCommand = LastCommand.TrimEnd(CType(" ", Char))
            Base.CommandMode(LastCommand)
        End sub

        public shared sub CtrlC
            Console.ForegroundColor = DefaultColor
            console.WriteLine()
            Environment.Exit(0)
        End sub

        public shared sub Backspace
            if Command.Length > 0 and cursorPos > 0 Then
                Command.Remove(CursorPos - 1, 1)
                CursorPos -= 1
                Console.SetCursorPosition(Whoami.Length + CursorPos, console.CursorTop)
                Console.Write(" ")
                Console.SetCursorPosition(Whoami.Length + CursorPos, console.CursorTop)
            Else
                Console.Beep()
            End If
        End sub

        public shared sub UpArrow
            if LastCommand = Nothing Then
                Console.Beep()
            Else
                dim length as Integer = Command.Length
                for i = 0 to length - 1
                    Console.Write(vbBack)
                    Console.Write(" ")
                    Console.Write(vbBack)
                Next
                Command.Clear()
                Console.Write(LastCommand)
                Command.Append(LastCommand)
            End If
        End sub
    End Class
End Namespace