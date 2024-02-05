using System.Diagnostics;

try
{
    //Wsdl Url
    string wsdlUrl = "http://www.dneonline.com/calculator.asmx?wsdl";
    
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

    string classContent = File.ReadAllText(outputFilePath);
    Console.WriteLine($"Created Class Path :{outputFilePath}");
    Console.WriteLine("Created Class Content:");
    Console.WriteLine(classContent);
    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}