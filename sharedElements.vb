imports System.Reflection
imports System.Runtime.InteropServices
imports System.resources

Friend MustInherit Class SharedElements
    public shared ReadOnly Settingspath as string = GetStartupPath() + "settings.json"

    Private shared Function GetStartupPath() As String
        dim path as string = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            path = path + "\"
        Else
            path = path + "/"
        End if
        Return path
    End Function

    public shared function GetTranslation(query as string, lang as string) as String
        System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(lang)
        Dim rm As New ResourceManager("kliens.i18n", Assembly.GetExecutingAssembly())
        Dim result As String = rm.GetString(query)
        if result = nothing Then
            System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("default")
            result = rm.GetString(query)
        End If
        return result
    End function
End Class