Imports System.Net.Http
imports kliens.SharedElements

Public Class Kretamechanics
    public shared ReadOnly Client as HttpClient = new HttpClient()
    public shared readonly schoolspath as string = GetStartupPath() + "schools.json"

    public shared function SearchSchool(query as string) as Task
        dim task as task = task.Run(Sub()
            if checkCacheTime(schoolspath, 300) = False Then
                cacheSchools()
            Else
                Console.WriteLine(GetTranslation("filecached", lang).Replace("%s", getCacheTime(schoolspath).ToString()))
            End If
        end sub)
        return task
    End function

    public shared function cacheSchools() as Task
        dim task as task = task.Run(Sub()
            Client.DefaultRequestHeaders.Add("User-Agent", "Naplika/v1 #SchoolCacher")
            ' thanks kima
            dim response as HttpResponseMessage = Client.GetAsync("https://api.refilc.hu/v3/public/school-list").Result
            Client.DefaultRequestHeaders.Remove("User-Agent")
            if response.IsSuccessStatusCode Then
                dim content as string = response.Content.ReadAsStringAsync().Result
                System.IO.File.WriteAllText(schoolspath, content)
            Else
                Console.WriteLine(GetTranslation("schoolgetfailed", Lang).Replace("%s", response.StatusCode.ToString()))
            End If
        end sub)
        return task
    End function
End Class