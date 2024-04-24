Imports System.Text

Namespace CommandLineEssentials
Public Class SharedElements
    public shared Sub UpdateConfig()
        if Program.updatedconfig = true Then
            DecryptConf = FuckMyBytes.SecurityMeasurements.decryptconfig()
            DecryptAuth = FuckMyBytes.SecurityMeasurements.DecryptAuth()
            ConsoleSet.Username = GetSettings("user")
            Consoleset.School = GetSettings("school")
            Program.updatedconfig = False
        End If
    End Sub
    public structure ConsoleSet
        public shared LastCommand as string
        public shared Command as new StringBuilder
        public Shared CursorPos as Integer = 0
        public Shared Username as String = Program.GetSettings("user")
        public Shared School as string = Program.GetSettings("school")
        public Shared DefaultColor as ConsoleColor = consolecolor.Gray
        public shared CommandColor as ConsoleColor = ConsoleColor.Yellow
        public Shared Whoami as string = Username + "@" + School + "> "
    End structure
End Class
End Namespace