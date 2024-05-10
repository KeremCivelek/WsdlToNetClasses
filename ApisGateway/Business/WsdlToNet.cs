using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WsdlToNetClasses.Business
{
    public class WsdlToNet
    {
        public void GetNetClasses(string wsdlUrl)
        {
            //Svcutil.exe path for windows11
            string svcutilPath = @"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools\svcutil.exe";

            //Output file path diffrence svcutil path
            string outputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GeneratedClasses.cs");

            //Start process on windows
            Process process = new Process();
            process.StartInfo.FileName = svcutilPath;
            process.StartInfo.Arguments = $"/language:C# /out:{outputFilePath} {wsdlUrl}";
            process.Start();
            process.WaitForExit();

            #region MyRegion
            //string classContent = File.ReadAllText(outputFilePath);
            //Console.WriteLine($"Created Class Path :{outputFilePath}");
            //Console.WriteLine("Created Class Content:");
            //Console.WriteLine(classContent);

            //// Sınıfları ve methodları ayırmak için düzenli ifade kullan
            //string pattern = @"(public(?:\s+partial)?\s+class\s+.*?{(?:[^{}]+|(?<open>{)|(?<-open>}))*})";
            //MatchCollection classMatches = Regex.Matches(classContent, pattern, RegexOptions.Singleline);
            //// Sınıfları ve methodları tutacak bir liste oluştur
            //List<string> classDefinitions = new List<string>();
            //// Her bir eşleşme için sınıf tanımlamasını al ve listeye ekle
            //foreach (Match match in classMatches)
            //{
            //    string classDefinition = match.Groups[1].Value;
            //    classDefinitions.Add(classDefinition);
            //}
            //// Sınıf tanımlamalarını yazdır
            //foreach (var classDef in classDefinitions)
            //{
            //    Console.WriteLine(classDef);
            //}



            //string[] lines = File.ReadAllLines(outputFilePath);
            //// Tanımlama sınıflarını ve endpoint'leri ayırmak için Regex deseni
            //string classPattern = @"public\s+(partial\s+)?class\s+\w+";
            //string methodPattern = @"public\s+(\w+\s+)?\w+\s+\w+\s*\(.+\)";

            //// Tanımlama sınıflarını ve endpoint'leri saklamak için listeler
            //var classMatches = Regex.Matches(string.Join("\n", lines), classPattern);
            //var methodMatches = Regex.Matches(string.Join("\n", lines), methodPattern);

            //// Tanımlama sınıflarını ve endpoint'leri ekrana yazdır
            //Console.WriteLine("Tanımlama Sınıfları:");
            //foreach (Match match in classMatches)
            //{
            //    Console.WriteLine(match.Value);
            //}

            //Console.WriteLine("\nEndpoint'ler:");
            //foreach (Match match in methodMatches)
            //{
            //    Console.WriteLine(match.Value);
            //} 
            #endregion


            // Dosyadaki tüm satırları oku
            string[] lines = File.ReadAllLines(outputFilePath);

            #region MyRegion
            //// Tanımlama sınıflarını ve endpoint'leri ayırmak için Regex deseni
            //string classPattern = @"public\s+(partial\s+)?class\s+(\w+)";
            //string propertyPattern = @"\b(public|private|protected|internal)\s+(\w+)\s+(\w+)\s*\{";

            //// Sınıf adlarını ve property bilgilerini saklamak için sözlükler
            //Dictionary<string, List<string>> classProperties = new Dictionary<string, List<string>>();

            //// Sınıf adlarını bul
            //var classMatches = Regex.Matches(string.Join("\n", lines), classPattern);
            //foreach (Match classMatch in classMatches)
            //{
            //    string className = classMatch.Groups[2].Value;
            //    classProperties[className] = new List<string>();
            //}

            //// Property bilgilerini bul
            //var propertyMatches = Regex.Matches(string.Join("\n", lines), propertyPattern);
            //foreach (Match propertyMatch in propertyMatches)
            //{
            //    string accessModifier = propertyMatch.Groups[1].Value;
            //    string propertyType = propertyMatch.Groups[2].Value;
            //    string propertyName = propertyMatch.Groups[3].Value;

            //    // Property bilgisini ekleyerek sakla
            //    string propertyInfo = $"{accessModifier} {propertyType} {propertyName}";
            //    string className = classMatches
            //        .Cast<Match>()
            //        .FirstOrDefault(m => m.Index < propertyMatch.Index)?
            //        .Groups[2]
            //        .Value;

            //    if (className != null && classProperties.ContainsKey(className))
            //    {
            //        classProperties[className].Add(propertyInfo);
            //    }
            //}

            //// Sınıfların property bilgilerini ekrana yazdır
            //foreach (var kvp in classProperties)
            //{
            //    Console.WriteLine($"Sınıf Adı: {kvp.Key}");
            //    foreach (var propertyInfo in kvp.Value)
            //    {
            //        Console.WriteLine($"    {propertyInfo}");
            //    }
            //}

            #endregion


            // Tanımlama sınıflarını ve endpoint'leri ayırmak için Regex deseni
            string classPattern = @"public\s+(partial\s+)?class\s+(\w+)";
            string propertyPattern = @"\b(public|private|protected|internal)\s+(\w+)\s+(\w+)\s*\{";

            // Sınıf adlarını ve property bilgilerini saklamak için sözlükler
            Dictionary<string, List<string>> classProperties = new Dictionary<string, List<string>>();

            // Sınıf adlarını bul
            var classMatches = Regex.Matches(string.Join("\n", lines), classPattern);
            foreach (Match classMatch in classMatches)
            {
                string className = classMatch.Groups[2].Value;
                classProperties[className] = new List<string>();
            }

            // Property bilgilerini bul
            foreach (var classMatch in classMatches)
            {
                string currentClass = ((Match)classMatch).Groups[2].Value;
                string pattern = $@"{Regex.Escape(currentClass)}\s*{{|((?!{Regex.Escape(currentClass)}).)*\b(public|private|protected|internal)\s+(\w+)\s+(\w+)\s*\{{";
                var propertyMatches = Regex.Matches(string.Join("\n", lines), pattern);

                foreach (Match propertyMatch in propertyMatches)
                {
                    if (propertyMatch.Groups[2].Success)
                    {
                        string accessModifier = propertyMatch.Groups[2].Value;
                        string propertyType = propertyMatch.Groups[3].Value;
                        string propertyName = propertyMatch.Groups[4].Value;

                        string propertyInfo = $"{accessModifier} {propertyType} {propertyName}";
                        classProperties[currentClass].Add(propertyInfo);
                    }
                }
            }

            // Sınıfların property bilgilerini ekrana yazdır
            foreach (var kvp in classProperties)
            {
                Console.WriteLine($"Sınıf Adı: {kvp.Key}");
                foreach (var propertyInfo in kvp.Value)
                {
                    Console.WriteLine($"    {propertyInfo}");
                }
            }


            Console.ReadLine();
        }
    }
}
