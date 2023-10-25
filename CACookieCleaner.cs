using System;
using System.IO;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            //C:\Users\caadmin\Desktop\CyberArk Reporting Issue 1.har
            Console.Write("Enter the full path to the input HAR file: ");
            var filePath = Console.ReadLine();
            var inputFile = new FileInfo(filePath);
            var outputFile = Path.Combine(Environment.CurrentDirectory, $"output_{DateTime.Now.ToString("yyyy-MM-dd")}.har");
            

            if (inputFile.Extension.Equals(".har", StringComparison.InvariantCultureIgnoreCase))
            {
                string harJson = File.ReadAllText(inputFile.FullName);
                JObject harData = JObject.Parse(harJson);

                var entries = harData["log"]["entries"];
                foreach (var entry in entries)
                {
                    ModifyHeaders(entry["request"], "cookie");
                    ModifyHeaders(entry["response"], "set-cookie");
                }

                File.WriteAllText(outputFile, harData.ToString());

                Console.WriteLine("Cookies and tokens cleared successfully");
            }
            else
            {
                Console.WriteLine("Input file must have a .har extension");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static void ModifyHeaders(JToken element, string filter)
    {
        var headers = element["headers"];
        if (headers != null)
        {
            var updatedHeaders = new JArray();
            foreach (var header in headers)
            {
                var name = header["name"];
                if (name != null && !name.ToString().ToLower().Contains(filter))
                {
                    updatedHeaders.Add(header);
                }
            }
            element["headers"] = updatedHeaders;
        }
    }
}
