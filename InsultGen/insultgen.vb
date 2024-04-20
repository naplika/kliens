imports System.Xml.Linq
imports System.Linq
Imports System.Runtime.InteropServices.JavaScript
Imports Org.BouncyCastle.Asn1.Cmp

Namespace InsultGen
    Public Module Main
        private ReadOnly dirtywords as string = SharedElements.GetStartupPath() + "dirtywords.xml"

        public Function Insult() As String
            dim doc as XDocument = XDocument.Load(dirtywords)
            dim nouns as List(Of String) =
                    doc.Descendants("Word").Where(Function(x) x.Attribute("type").Value = "f").Select(
                        Function(x) x.Value.ToLower()).ToList()
            dim adjectives as List(Of String) =
                    doc.Descendants("Word").Where(Function(x) x.Attribute("type").Value = "m").Select(
                        Function(x) x.Value.ToLower()).ToList()
            dim random as new Random
            dim chooseInsultType as Integer = random.Next(1, 7)
            dim insu as string
            select case chooseInsultType
                case 1 : insu = nouns(random.Next(nouns.Count))
                case 2 : insu = adjectives(random.Next(adjectives.Count))
                case 3 : insu = adjectives(random.Next(adjectives.Count)) + " " + nouns(random.Next(nouns.Count))
                case 4 _
                    : insu = adjectives(random.Next(adjectives.Count)) + ", " + adjectives(random.Next(adjectives.Count)) +
                             " " + nouns(random.Next(nouns.Count))
                case 5 : insu = nouns(random.Next(nouns.Count)) + " " + nouns(random.Next(nouns.Count))
                case 6 _
                    : insu = adjectives(random.Next(adjectives.Count)) + ", " + adjectives(random.Next(adjectives.Count)) +
                             ", " + adjectives(random.Next(adjectives.Count)) + " " + nouns(random.Next(nouns.Count))
            End Select
            dim asd as integer = random.Next(1,3)
            if asd = 1 Then
                insu = "Hogy rohadnál meg, te " + insu + "!"
                else
                    insu = "Te " + insu + "!"
            End If
return insu
        End Function
    End Module
End Namespace