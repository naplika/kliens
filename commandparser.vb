imports kliens.DataResolver

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
                    if args.Length < 2 Then
                        Console.ForegroundColor = ConsoleColor.Red
Console.WriteLine("no")
                     Exit Sub   
                    End If
                    searchschool(args(1)).Wait()
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