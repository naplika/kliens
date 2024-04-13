﻿imports kliens.DataResolver

Public MustInherit Class Commandparser
    public shared Function Parsecommand(args as String()) as Task
        dim task as task = task.run(Sub()
            if args.Length > 0 Then
                if args(0) = "exit" Then
                    cmd_exit()
                elseif args(0) = "clear" or args(0) = "cls" Then
                    Console.Clear()
                elseif args(0) = "schools" Then
                    if args.Length <2 Then
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("no")
                        Exit Sub
                    
                    End If
                    searchschool(args(1)).Wait()
                Else
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine(SharedElements.GetTranslation("cmdnotfound", Lang))
                End If
            End If
            End Sub)
return task
    End Function

    private Shared function cmd_exit()
        Console.ForegroundColor = ConsoleColor.Gray
        Environment.Exit(0)
        return true
    End function
End Class