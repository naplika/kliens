imports kliens.DataResolver

#Disable Warning BC42021

Public MustInherit Class Commandparser
    public shared Function Parsecommand(args as String()) as Task
        dim task as task = task.run(Sub()
            if args.Length > 0 Then
                if args(0) = "exit" Then
                    cmd_exit()

                elseif args(0) = "clear" or args(0) = "cls" Then
                    Console.Clear()
                elseif args(0) = "schools" Then
                    if args.Length < 2 Then
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine(SharedElements.GetTranslation("schoolscmd", lang))
                        Exit Sub
                    End If
                    searchschool(args(1)).Wait()
                elseif args(0) = "help" Then
                    SharedElements.help()
                elseif args(0) = "insult" Then
                    Console.WriteLine(InsultGen.Insult())
                elseif args(0) = "debug" Then
                    if args.Length >= 2 AndAlso args(1) = "decryptconfig" Then
                        IO.File.WriteAllText(SharedElements.GetStartupPath() + "settings.json", DecryptConf)
                        Console.WriteLine("Configuration decrypted.")
                        Environment.Exit(0)
                        elseif args.Length = 3 AndAlso args(1) = "fuckstring" Then
                            Console.WriteLine(FuckMyBytes.FuckString(args(2)))
                    elseif args.Length >= 2 AndAlso args(1) = "decryptauth" Then
                        IO.File.WriteAllText(SharedElements.GetStartupPath() + "authorization.json", DecryptAuth)
                        Console.WriteLine("Authorisation dump decrypted.")
                        Environment.Exit(0)
                    End If
                elseif args(0) = "config" then
                    if args.Length < 2 Then
                        SharedElements.printconfigables()
                    Else

                    End If
                    elseif args(0) = "login" Then
                        if args.Length < 4 Then
                            Console.WriteLine("login <username> <password> <institutecode>")
                            Else 
                                DataResolver.Authorize(args(1), args(2), args(3)).Wait()
                        End If
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