Imports System.IO
Imports System.Net.Http
imports kliens.SharedElements
imports Newtonsoft.Json.Linq
imports System.Security.Cryptography
imports System.Text
Imports System.Threading.Tasks.Sources

Public MustInherit Class DataResolver
    private shared ReadOnly Client as HttpClient = new HttpClient()
    private shared readonly Schoolspath as string = GetStartupPath() + "schools.json"
    public shared UserAgent as string = ""
    public shared function SearchSchool(query as string) as Task
        dim task as task = task.Run(Sub()
            dim schooldata as string
            if CheckCacheTime(Schoolspath, 300) = False Then
                if SharedElements.checkinternetconnection = true Then
                    cacheSchools().Wait()
                Else
                    Console.WriteLine(GetTranslation("no.internet", lang))
                End If
                schooldata = io.File.ReadAllText(Schoolspath)
            Else
                Console.WriteLine(GetTranslation("file.cached", lang).Replace("%s", GetCacheTime(Schoolspath).ToString()))
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
                    hits += vbcrlf + GetTranslation("search.hit", lang).Replace("%s", hitnum.ToString()) + vbcrlf + name +
                            vbcrlf + instituteCode + " (" + school("city").ToString() + ")"
                End If
            Next
            Console.WriteLine(hits)
            if hitnum > 1 then
                Console.WriteLine(GetTranslation("more.than.1.school", lang).Replace("%s", hitnum.ToString()))
                Console.WriteLine(GetTranslation("be.specific", lang))
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
                Console.WriteLine(GetTranslation("school.get.failed", Lang).Replace("%s", response.StatusCode.ToString()))
            End If
        end sub)
        return task
    End function
    
    public shared function GetUserAgent() as Task
        dim task as task = task.Run(Sub()
            dim version as string
            Client.DefaultRequestHeaders.Add("User-Agent", "Naplika/v1 #UserAgent")
            dim response as HttpResponseMessage = Client.GetAsync("https://api.refilc.hu/v1/public/client/version/latest/name").Result
            if response.IsSuccessStatusCode Then
                version = response.Content.ReadAsStringAsync().Result
            Else
                version = "4.5.1"
            End If
            dim ua = "hu.ekreta.student/$0 $1/$2"
            UserAgent = ua.Replace("$0", version).Replace("$1", "Android").Replace("$2", "0")
            Client.DefaultRequestHeaders.Remove("User-Agent")
            end sub)
        return task
    End function
    
    public shared function Authorize(username as string, password as String, institutecode as String) as Task
        dim task as task = task.Run(Sub()
            dim nonce as string = GenerateNonce()
            if nonce = "fail" Then
                exit Sub
            End If
            dim policyKey as string = GenerateAuthorizationPolicy(username, institutecode, nonce)
            Client.DefaultRequestHeaders.Add("X-Authorizationpolicy-Key", policyKey)
            Client.DefaultRequestHeaders.add("X-Authorizationpolicy-Version", "v2")
            Client.DefaultRequestHeaders.Add("X-Authorizationpolicy-Nonce", nonce)
            Client.DefaultRequestHeaders.Add("User-Agent", UserAgent.ToString())
            dim url as string = "https://idp.e-kreta.hu/connect/token"
            dim clientid as string = "kreta-ellenorzo-mobile-android"
            dim granttype as String = "password"
            dim response as HttpResponseMessage =
                    client.PostAsync(url, New FormUrlEncodedContent(New Dictionary(Of String, String) From {
                                                                       {"userName", username},
                                                                       {"password", password},
                                                                       {"institute_code", institutecode},
                                                                       {"grant_type", granttype},
                                                                       {"client_id", clientid}
                                                                       })).Result
            Client.DefaultRequestHeaders.Remove("X-Authorizationpolicy-Key")
            Client.DefaultRequestHeaders.Remove("X-Authorizationpolicy-Nonce")
            Client.DefaultRequestHeaders.Remove("X-Authorizationpolicy-Version")
            Client.DefaultRequestHeaders.Remove("User-Agent")
            if response.IsSuccessStatusCode Then
                dim content as string = response.Content.ReadAsStringAsync().Result
                dim json as JObject = JObject.Parse(content)
                dim token as string = json("access_token").ToString()
                dim tokenparts as String() = token.Split(".")
                dim signature as string = BitConverter.ToString(Base64UrlDecode(tokenparts(2))).Replace("-", "")
                Console.WriteLine(GetTranslation("auth.signature", Lang) + FuckMyBytes.LengthController(signature, 5))
                dim payload as JObject =
                        JObject.Parse(System.Text.Encoding.UTF8.GetString(SharedElements.Base64UrlDecode(tokenparts(1))))
                Console.WriteLine(GetTranslation("hello.user", Lang).Replace("%s", payload("name").ToString()))
                SaveLogin(content, signature, password, username, institutecode)
            Else
                Console.WriteLine(GetTranslation("auth.failed", Lang) + response.StatusCode.ToString())
            End If
        end sub)
        return task
    End function

    private shared function GenerateNonce() as String
        Client.DefaultRequestHeaders.Remove("User-Agent")
        Client.DefaultRequestHeaders.Add("User-Agent", UserAgent.ToString())
        dim url as string = "https://idp.e-kreta.hu/nonce"
        dim response as HttpResponseMessage = client.GetAsync(url).Result
        Client.DefaultRequestHeaders.Remove("User-Agent")
        if response.IsSuccessStatusCode Then
            return response.Content.ReadAsStringAsync().Result
        Else
            Console.WriteLine(GetTranslation("auth.nonce.failed", Lang) + response.StatusCode.ToString())
            return "fail"
        End If
    End function

    private shared function GenerateAuthorizationPolicy(username as String, instituteCode as String, nonce as string) _
        as string
        dim hmacKey as string = "baSsxOwlU1jM"
        dim keybytes as Byte() = Encoding.UTF8.GetBytes(hmacKey)
        dim text as string = (instituteCode.ToUpper() + nonce + username.ToUpper())
        using hmac as new HMACSHA512(keybytes)
            dim computedHash as Byte() = hmac.ComputeHash(Encoding.UTF8.GetBytes(text))
            return Convert.toBase64String(computedHash)
        End Using
    End function
    
    public shared function Refresh() as Task
        dim task as task = task.Run(Sub()
            if File.Exists(Loginpath) Then
                dim json as JObject = JObject.Parse(DecryptAuth)
                dim refreshToken as string = json("refresh").ToString()
                dim instituteCode as string = GetSettings("school")
                dim clientid as string = "kreta-ellenorzo-mobile-android"
                dim granttype as string = "refresh_token"
                Client.DefaultRequestHeaders.Add("User-Agent", UserAgent.ToString())
                dim url as string = "https://idp.e-kreta.hu/connect/token"
                dim response as HttpResponseMessage =
                        client.PostAsync(url, New FormUrlEncodedContent(New Dictionary(Of String, String) From {
                                                                           {"institute_code", institutecode},
                                                                           {"refresh_token", refreshToken},
                                                                           {"grant_type", granttype},
                                                                           {"client_id", clientid}
                                                                           })).Result
                client.DefaultRequestHeaders.Remove("User-Agent")
                if response.IsSuccessStatusCode Then
                    dim content as string = response.Content.ReadAsStringAsync().Result
                    dim json2 as JObject = JObject.Parse(content)
                    dim token as string = json2("access_token").ToString()
                    dim tokenparts as String() = token.Split(".")
                    dim signature as string = BitConverter.ToString(Base64UrlDecode(tokenparts(2))).Replace("-", "")
                    dim username as string = GetSettings("user")
                    dim password as string = GetSettings("password")
                    if password = "undefined" Then
                        Console.WriteLine(GetTranslation("auth.refresh.relog", Lang))
                        exit sub
                    End If
                    SaveLogin(content, signature, password, username, instituteCode)
                Else 
                    dim username as string = GetSettings("user")
                    dim password as string = GetSettings("password")
                    Authorize(username, password, instituteCode).Wait()
                End If
            End If
            end sub)
        return task
    End function
    
    public shared function DeleteToken() as Task
        dim task as task = task.Run(sub()
            Client.DefaultRequestHeaders.Remove("User-Agent")
            Client.DefaultRequestHeaders.Add("User-Agent", UserAgent.ToString())
            dim url as string = "https://idp.e-kreta.hu/connect/revocation"
            dim clientid as string = "kreta-ellenorzo-mobile-android"
            dim refreshToken as string = JObject.Parse(DecryptAuth)("refresh").ToString()
            dim response as HttpResponseMessage =
                    client.PostAsync(url, new FormUrlEncodedContent(New Dictionary(Of String, String) From {
                                                                       {"token", refreshToken},
                                                                       {"client_id", clientid}
                                                                       })).Result
            if response.IsSuccessStatusCode Then
                Console.WriteLine(GetTranslation("auth.token.deleted", Lang))
            Else
                Console.WriteLine(GetTranslation("auth.token.delete.failed", Lang) + response.StatusCode.ToString())
            End If
        end sub)
        return task
    End function
End Class