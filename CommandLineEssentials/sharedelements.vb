Imports System.Text

Namespace CommandLineEssentials
Public Class SharedElements
    public shared Sub UpdateConfig(force as boolean)
        if Program.updatedconfig = true or force Then
            Program.DecryptConf = FuckMyBytes.SecurityMeasurements.decryptconfig()
            Program.DecryptAuth = FuckMyBytes.SecurityMeasurements.DecryptAuth()
            ConsoleSet.Username = GetSettings("user")
            Consoleset.School = GetSettings("school")
            ConsoleSet.Whoami = ConsoleSet.Username + "@" + ConsoleSet.School + "> "
            Program.updatedconfig = False
        End If
    End Sub
    public structure ConsoleSet
        public shared LastCommand as string
        public shared Command as new StringBuilder
        public Shared CursorPos as Integer = 0
        public Shared Username as String = ""
        public Shared School as string = ""
        public Shared DefaultColor as ConsoleColor = consolecolor.Gray
        public shared CommandColor as ConsoleColor = ConsoleColor.Yellow
        public Shared Whoami as string = Username + "@" + School + "> "
    End structure
End Class
End Namespace