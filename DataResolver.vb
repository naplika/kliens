Imports System.Net.Http
imports kliens.SharedElements
imports Newtonsoft.Json.Linq

Public Class DataResolver
    public shared ReadOnly Client as HttpClient = new HttpClient()
    public shared readonly schoolspath as string = GetStartupPath() + "schools.json"

    public shared function SearchSchool(query as string) as Task
        dim task as task = task.Run(Sub()
            dim schooldata as string
            if checkCacheTime(schoolspath, 300) = False Then
                cacheSchools().Wait()
                schooldata = io.File.ReadAllText(schoolspath)
            Else
                Console.WriteLine(GetTranslation("filecached", lang).Replace("%s", getCacheTime(schoolspath).ToString()))
                schooldata = io.File.ReadAllText(schoolspath)
            End If
            dim schools as JArray = JArray.Parse(schooldata)
            dim hits as string = ""
            dim hitnum as integer = 0
            for each school as JObject in schools
                dim instituteCode as string = school("instituteCode").Tostring()
                dim name as string = school("name").ToString()
                if fuzzySearch(query, instituteCode) > 80 or Fuzzysearch(query, name) > 80 Then
                    hitnum += 1
                   hits += vbcrlf + GetTranslation("searchhit", lang).Replace("%s", hitnum.ToString()) + vbcrlf + name + vbcrlf + instituteCode + " (" + school("city").ToString() + ")" 
                End If
            Next
            Console.WriteLine(hits)
            if hitnum > 1 then
                Console.WriteLine(GetTranslation("morethan1school", lang).Replace("%s", hitnum.ToString()))
                Console.WriteLine(GetTranslation("bespecific", lang))
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