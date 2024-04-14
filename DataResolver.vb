Imports System.Net.Http
imports kliens.SharedElements
imports Newtonsoft.Json.Linq

Public MustInherit Class DataResolver
    private shared ReadOnly Client as HttpClient = new HttpClient()
    private shared readonly Schoolspath as string = GetStartupPath() + "schools.json"

    public shared function SearchSchool(query as string) as Task
        dim task as task = task.Run(Sub()
            dim schooldata as string
            if CheckCacheTime(Schoolspath, 300) = False Then
                if SharedElements.checkinternetconnection = true Then
                    cacheSchools().Wait()
                Else
                    Console.WriteLine(GetTranslation("nointernet", lang))
                End If
                schooldata = io.File.ReadAllText(Schoolspath)
            Else
                Console.WriteLine(GetTranslation("filecached", lang).Replace("%s", GetCacheTime(Schoolspath).ToString()))
                schooldata = io.File.ReadAllText(Schoolspath)
            End If
            dim schools as JArray = JArray.Parse(schooldata)
            dim hits = ""
            dim hitnum = 0
            for each school as JObject in schools
                dim instituteCode as string = school("instituteCode").Tostring()
                dim name as string = school("name").ToString()
                if FuzzySearch(query, instituteCode) > 90 or FuzzySearch(query, name) > 90 Then
                    hitnum += 1
                    hits += vbcrlf + GetTranslation("searchhit", lang).Replace("%s", hitnum.ToString()) + vbcrlf + name +
                            vbcrlf + instituteCode + " (" + school("city").ToString() + ")"
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

    private shared function CacheSchools() as Task
        dim task as task = task.Run(Sub()
            Client.DefaultRequestHeaders.Add("User-Agent", "Naplika/v1 #SchoolCacher")
            ' thanks kima
            dim response as HttpResponseMessage = Client.GetAsync("https://api.refilc.hu/v3/public/school-list").Result
            Client.DefaultRequestHeaders.Remove("User-Agent")
            if response.IsSuccessStatusCode Then
                dim content as string = response.Content.ReadAsStringAsync().Result
                System.IO.File.WriteAllText(Schoolspath, content)
            Else
                Console.WriteLine(GetTranslation("schoolgetfailed", Lang).Replace("%s", response.StatusCode.ToString()))
            End If
        end sub)
        return task
    End function
End Class