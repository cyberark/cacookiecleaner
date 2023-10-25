using System;
using System.IO;
using Newtonsoft.Json.Linq;

class CACookieCleaner
{
    static void Main(string[] args)
    {
        try
        {
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
                    ModifyHeaders(entry["request"], "Cookie");
                    ModifyHeaders(entry["response"], "Set-Cookie");
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
                if (name != null && !name.ToString().Contains(filter) && name.ToString() != filter && name.ToString().ToLower() != filter.ToLower())
                {
                    updatedHeaders.Add(header);
                }
            }
            element["headers"] = updatedHeaders;
        }
    }
}
