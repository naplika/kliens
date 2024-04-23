Imports System.Net.Http
imports kliens.SharedElements
imports Newtonsoft.Json.Linq
imports System.Security.Cryptography
imports System.Text
Imports HMac = Org.BouncyCastle.Crypto.Macs.HMac

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
    
    public shared function Authorize(username as string, password as String, institutecode as String) as Task
        dim task as task = task.Run(Sub()
            dim nonce as string = GenerateNonce()
            Console.WriteLine("nonce " + nonce)
            if nonce = "fail" Then
                exit Sub
            End If
            dim policyKey as string = GenerateAuthorizationPolicy(username,institutecode,nonce)
            Console.WriteLine("polkey " + policyKey)
            Client.DefaultRequestHeaders.Add("X-Authorizationpolicy-Key", policyKey)
            Client.DefaultRequestHeaders.add("X-Authorizationpolicy-Version", "v2")
            Client.DefaultRequestHeaders.Add("X-Authorizationpolicy-Nonce", nonce)
            dim response as HttpResponseMessage = client.PostAsync("https://idp.e-kreta.hu/connect/token", New FormUrlEncodedContent(New Dictionary(Of String, String) From {
                {"userName", username},
                {"password", password},
                {"institute_code", institutecode},
                {"grant_type", "password"},
                {"client_id", "kreta-ellenorzo-mobile-android"}
            })).Result
            Client.DefaultRequestHeaders.Remove("X-Authorizationpolicy-Key")
            Client.DefaultRequestHeaders.Remove("X-Authorizationpolicy-Nonce")
            Client.DefaultRequestHeaders.Remove("X-Authorizationpolicy-Version")
            if response.IsSuccessStatusCode Then
                dim content as string = response.Content.ReadAsStringAsync().Result
                Console.WriteLine(content)
            Else
                Console.WriteLine("Failed to authorize, " + response.StatusCode.ToString())
            End If
            end sub)
        return task
    End function
    
    private shared function GenerateNonce() as String
        Client.DefaultRequestHeaders.Remove("User-Agent")
        dim response as HttpResponseMessage = client.GetAsync("https://idp.e-kreta.hu/nonce").Result
        if response.IsSuccessStatusCode Then
              return response.Content.ReadAsStringAsync().Result
            Else 
                Console.WriteLine("Failed to generate nonce, " + response.StatusCode.ToString())
                return "fail"
        End If
    End function
    
    private shared function GenerateAuthorizationPolicy(username as String, instituteCode as String, nonce as string) as string
        dim hmacKey as string = "baSsxOwlU1jM"
        dim keybytes as Byte() = Encoding.UTF8.GetBytes(hmacKey)
        dim text as string = (instituteCode.ToUpper() + nonce + username.ToUpper())
        using hmac as new HMACSHA512(keybytes)
            dim computedHash as Byte() = hmac.ComputeHash(Encoding.UTF8.GetBytes(text))
            return Convert.toBase64String(computedHash)
        End Using
    End function
End Class