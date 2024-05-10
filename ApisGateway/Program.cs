using System.Diagnostics;
using System.Text.RegularExpressions;
using WsdlToNetClasses.Business;

try
{
    //Wsdl Url
    string wsdlUrl = "http://webservices.oorsprong.org/websamples.countryinfo/CountryInfoService.wso?WSDL";
   
    WsdlToNet toNet = new WsdlToNet();
    toNet.GetNetClasses(wsdlUrl);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}