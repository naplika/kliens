Imports System.Net.Http
imports kliens.SharedElements

Public Class Kretamechanics
    dim Client = new HttpClient()
    dim schoolspath as string = GetStartupPath() + "schools.json"

    public function SearchSchool(query as string) as Task
        dim task as task = task.Run(Sub()
        end sub)
        return task
    End function
End Class