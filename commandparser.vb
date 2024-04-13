imports kliens.Kretamechanics

Public MustInherit Class Commandparser
    public shared sub Parsecommand(args as String())
        if args.Length > 0 Then
            select case args(0)
                case "exit"
                    cmd_exit()
                case "clear"
                    Console.Clear()
                case "cls"
                    Console.Clear()
                case "schools"
                    searchschool(args(1))
                case Else
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine(SharedElements.GetTranslation("cmdnotfound", Lang))
            End Select
        End If
    End sub

    private Shared function cmd_exit()
        Console.ForegroundColor = ConsoleColor.Gray
        Environment.Exit(0)
        return true
    End function
End Class