using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Web.Services.Description;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WsdlToStructsAndUse
{
    class Program
    {
        static void Main(string[] args)
        {
            // WSDL URL'si
            string wsdlUrl = "http://webservices.oorsprong.org/websamples.countryinfo/CountryInfoService.wso?WSDL";

            // WSDL dosyasını indirme veya URL'den alma
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(wsdlUrl);

            // ServiceDescription nesnesini oluştur
            XmlSerializer ser = new XmlSerializer(typeof(Definitions));
            Definitions definitions = (Definitions)ser.Deserialize(stream);

            // WSDL'den tip bilgilerini al
            XmlSchemaSet schemas = new XmlSchemaSet();
            XmlSchemaImporter importer = new XmlSchemaImporter(schemas);

            // ServiceContract, OperationContract ve DataContract tanımlarını bul
            foreach (Message message in definitions.Items)
            {
                if (message is PortType portType)
                {
                    foreach (Operation operation in portType.Operation)
                    {
                        Console.WriteLine($"Operation Name: {operation.Name}");
                        foreach (object input in operation.Item)
                        {
                            if (input is Input inputElement)
                            {
                                Console.WriteLine($"Input Message: {inputElement.Message.Name}");
                            }
                        }
                    }
                }
                else if (message is Types types)
                {
                    foreach (object schema in types.Any)
                    {
                        XmlSchema xmlSchema = (XmlSchema)schema;
                        schemas.Add(xmlSchema);
                    }
                }
            }

            // Schemas'ı kullanarak C# kodu oluştur
            ServiceContractGenerator generator = new ServiceContractGenerator();
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";

            // Importer ile CodeDomProvider kullanarak kod birimini derle
            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
            CodeNamespace codeNamespace = new CodeNamespace();
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            codeCompileUnit.Namespaces.Add(codeNamespace);

            generator.GenerateServiceContractType(definitions, codeNamespace, codeCompileUnit);

            // CodeDomProvider kullanarak kodu derle
            StringWriter writer = new StringWriter();
            provider.GenerateCodeFromCompileUnit(codeCompileUnit, writer, options);
            string generatedCode = writer.ToString();

            // Derlenmiş kodu çalışma zamanında yükle
            Assembly assembly = Assembly.Load(generatedCode);

            // Web servis istemci sınıfını al
            Type serviceType = assembly.GetType("CountryInfoServiceSoapType");

            // Web servisi çağır
            MethodInfo[] methods = serviceType.GetMethods();
            object serviceInstance = Activator.CreateInstance(serviceType);
            foreach (var method in methods)
            {
                if (method.Name == "ListOfContinentsByName")
                {
                    var parameters = new object[] { true };
                    var result = method.Invoke(serviceInstance, parameters);
                    Console.WriteLine("Servis çağrısı başarılı. Sonuç: " + result);
                    break;
                }
            }
        }
    }
}
