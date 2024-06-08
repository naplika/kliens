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
            dim url as string =
                    FuckMyBytes.UnFuckString(
                        "FuyGEO+Eyt8KeiSGFHwRRw21JVqCqBs6DATYiW7sUGPkPXY73wVliS/tJKIU5Dmj2CJFH2bl66oRsbc/XWAVdovVFysY1Kibk+vsoGA5fr06PRACFYSJw7ywJ7hTEq7j6s0WPsoMuPkVsWp1WcF3gdP2b1FK+Ysmsq/wlF6k13/oB/iweN6bqKcrsPqxXx7wVxHt24oHqHYhWC+bAUtPUFNiiZdebydBFOXNbPtz/vIZllUv7eWIJnIZodx3mqm4DHg9cEWVvXo6kLhFWWMkM/G4jzaEdvYdR00piXOAa9BcsP5/H/F7h5DuD3jRp0Du0guovHp6rgV2mw5JKOq07eaXsBk/o0A+0sQaNK/eLeP/3vSL0obcN42mtJXawnXLgKuZgIyTw1gklRM0GCXOYM8/bn9LTUGtX5uGn2oJe++GM3u9GDFCCExklCBvExSqJwFt5TasHfdcASu+kOjktQCCroXYlTlYur3Dr/Z70pb/LuCokrFwBdCgNYLk2w0xNcTmPhvaVmSBcfS7vWtQnl+o8tFB+mJmmoPzSf41lQ7cOfRCztr1Oysve3KblDB2gJQJWijXLKA=")
            dim clientid as string =
                    FuckMyBytes.UnFuckString(
                        "FuyGEO+Eyt8KeiSGFHwRR7WSRqEGQ378WAt0ex+pv50Xou0aPs9OkRhQ8WRnjbxm0/vJd/jiUEQFnBvq0UZ2DXEVE7xY/XbOYC+p7KbhfDqI9cxCPOObONnmu92h+Dh26TAE0jI9C1ALFBmR9+SlmRpRlgfLczIYs89Aut2i1xwdH6TpAGUrk50VGwx7g/tLGyoBUrWPcJSjjDmq7gtIJ3QupheH4pHDI0bNBmbli7B6RiFHwe+NT3cAkFgFCPxQOu+11OFkvDlzCOWynua3rKd2Q4dlT3arsXtCKEQt57pPaw+4Ea5xZfFU8whC9TjKVg5yiOR9MjHjaUVMvi73vMonyfmmK2VsT25u5imVMcyaGrOR8lgO7Z2aPvZGweqgRBcCFue4/Zy8ntUaz84SBAXgUMqWZU72QY+4+CD2udmfDjpHz81srLEp+lLRX9JF8IMhW3cl5HjNlVzEzyvlXhEAMCaTxxUkLbCHeSwITn9PH+xmNVR0HorW+p8pOOS0+mMTpZ6thP8=")
            dim granttype as String =
                    FuckMyBytes.UnFuckString(
                        "FuyGEO+Eyt8KeiSGFHwRR+RmaRs+ghhSTLuvqY3Jym+Nq9YK9SWznaKiZX6c79A1jlvfVt6N3jQ0gpsuCwqcejaJrBy+/+olwBvUIToDbEIzogGzDrDCfV08/32//Aspe8bZJk5ho5JUIkXpv0MNvk5pa6z5xWQ50c+ch8u2rt8NmC/Q4Vmo8lTI10T7mS9fITuWy8Y6dqC2wvbGTcLXK5gMYPBQhSYts3mlu7tlTfIMWBXB9I6/7xTOMBsF/J54lzKj76qSExAh8GJZKPHq3d84AqFx9YxAnElYrZQHJYtseadoPLHb8uFqpFtJP+WCDAkjkl9wBHxiV6xj55PtMeUtazXJ4quilqjgwHu5clB+A+PBQV76kp7wjp3nfZGu0RVoPMawaQ0y+Tmcp2C8/4+c8WaafRreaRalaR1dkuD2uvwb22Pd8A==")
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
                Console.WriteLine("Authorization Signature: " + FuckMyBytes.LengthController(signature, 5))
                dim payload as JObject =
                        JObject.Parse(System.Text.Encoding.UTF8.GetString(SharedElements.Base64UrlDecode(tokenparts(1))))
                Console.WriteLine(GetTranslation("hellouser", Lang).Replace("%s", payload("name").ToString()))
                SaveLogin(content, signature, username, institutecode)
            Else
                Console.WriteLine("Failed to authorize, " + response.StatusCode.ToString())
            End If
        end sub)
        return task
    End function

    private shared function GenerateNonce() as String
        Client.DefaultRequestHeaders.Remove("User-Agent")
        Client.DefaultRequestHeaders.Add("User-Agent", UserAgent.ToString())
        dim url as string =
                FuckMyBytes.UnFuckString(
                    "FuyGEO+Eyt8KeiSGFHwRR9hrRFab8x4QBjpUOlt/KsAjdhmYzQnsO3BmRDwrp7yRH9MPrsaRds/yWuZf8TWoobqJXOiG0LKWdh0XXxDpli017g64Jc3fyUGe7FsYo8GZHJYIGyEYLSAv3CnFCuutPcqZeAi5zimtUfMoXLxuj0eCPhoQ7cifUYg7E89zyC4V+p0bFmHjtxf7+YyVuIEnbaw3ZMt1P8HvaM9jz2pn1JLUGlX3iSiTVO6QrWE3pZgZTblqsqNJMdAllTEdKQFxmD9Z/z3d8YzNuyfIjQbxar1IvqalnXy0mXneSe7v0u7/dPVMD4sJ/GS9OzOKV0qp5gm4jLx9oDQVf/yEUKr9Hf7rEpUe+rneS1S2GpWFLasSNua9P6AvVczc7SvXLFS6Paeliqbh9HpABdgNH9rTR8cjVT+Vda4+wMY56yQ8kLKEHQuNXgzM7XuV4yJH+Whu0aPNzfhbM+UbgMznK6d/9Adnf8H2FZIDezyeKOMXbp7jH57dW5n2o5c=")
        dim response as HttpResponseMessage = client.GetAsync(url).Result
        Client.DefaultRequestHeaders.Remove("User-Agent")
        if response.IsSuccessStatusCode Then
            return response.Content.ReadAsStringAsync().Result
        Else
            Console.WriteLine("Failed to generate nonce, " + response.StatusCode.ToString())
            return "fail"
        End If
    End function

    private shared function GenerateAuthorizationPolicy(username as String, instituteCode as String, nonce as string) _
        as string
        dim hmacKey as string =
                FuckMyBytes.UnFuckString(
                    "FuyGEO+Eyt8KeiSGFHwRR6n+M67DBRBjoFjCunYctw9XGe4xvox5EJ5E4nUYsVuIgEmHfiA/BBcBTiDB5RQ/JNbIUpzCphBhY5A4wOAzB5OAwcl0EuHjHESlZ7vKhvN05rmtIfAxRLuP+MBLZuET6Basnyr7TPkUpl+hPzJ9/NEtZSYb78dPtX44BpJqpGb5KFJFDsXd4jbstJdUvio60/hFlvz1eJfVgquKxft6zyQQs09D7gyhaCLHc4TCUB7VUij1sxUpVTBmaQLYZwmLWPc7hk+JY3IQLNjAua0JDPYPlb1e/7flnBYOGSD4hRWPpRIXBzp3Y0ZlGue9N7JBjytPDWLWP22X8rk/0Z7+IKfn1UUf5OJbp8h7o2LcVFTfj+Sr9/ka8ZKn73pdq7v48Jc3Z3o6lmuh2uuA8ZbwINp/AqRDkTom3w==")
        dim keybytes as Byte() = Encoding.UTF8.GetBytes(hmacKey)
        dim text as string = (instituteCode.ToUpper() + nonce + username.ToUpper())
        using hmac as new HMACSHA512(keybytes)
            dim computedHash as Byte() = hmac.ComputeHash(Encoding.UTF8.GetBytes(text))
            return Convert.toBase64String(computedHash)
        End Using
    End function
End Class